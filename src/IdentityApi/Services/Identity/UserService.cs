using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Korzh.WindowsAzure.Storage;

using IdentityApi.Models;


namespace IdentityApi.Services
{
    public interface IUserService {
        Task<IEnumerable<User>> GetAllUsersAsync();
    }


    public class UserService : IUserRoleStore<User>, IUserPasswordStore<User>, IUserService //, IUserEmailStore<User>
    {
        private TableStorageService<User> userTable = new TableStorageService<User>("IDAPIUsers");
        private ILookupNormalizer _normalizer;

        public UserService(ILookupNormalizer lookupNormalizer) {
            this._normalizer = lookupNormalizer;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync() {
            return userTable.GetEntitiesByFilterAsync();
        }


        //------------ IUserRoleStore -------------
        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) {
            return await Task.FromResult(user.RowKey);
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) {
            return await Task.FromResult(user.Email);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken)) {
            return userTable.GetEntityByKeysAsync(User.DefaultPartitionKey, userId);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            var users = await userTable.ListEntitiesByFilterAsync(new Dictionary<string, object>() {
                {nameof(User.Email), normalizedUserName}
            });

            return users.FirstOrDefault();
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) {
            return await Task.FromResult(_normalizer.Normalize(user.Email));
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default(CancellationToken)) {
            try {
                await userTable.InsertOrMergeEntityAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex) {
                return IdentityResult.Failed(new IdentityError {
                    Code = ex.GetType().Name,
                    Description = ex.Message
                });
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = default(CancellationToken)) {
            try {
                await userTable.DeleteEntityAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex) {
                return IdentityResult.Failed( new IdentityError {
                        Code = ex.GetType().Name,
                        Description = ex.Message
                    });
            }
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) {
            //user.NormalizedUserName = normalizedName;
            //await userService.SaveAsync(user);
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) {
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) {
            try {
                await userTable.InsertOrMergeEntityAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex) {
                return IdentityResult.Failed(new IdentityError { Code = ex.GetType().Name, Description = ex.Message });
            }
        }


        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(roleName))
                return ;
            if (string.IsNullOrEmpty(user.UserRoles))
                user.UserRoles = roleName;
            else if (!user.UserRoles.Contains(roleName))
                user.UserRoles += "," + roleName;

            await userTable.InsertOrMergeEntityAsync(user);
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult<IList<string>>(user.UserRoles?.Split(',').ToList());
        }


        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            return (await userTable.GetEntitiesByFilterAsync(""))
                .Where(u => u.UserRoles.IndexOf(roleName) >= 0)
                .ToList();
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            return Task.FromResult(!string.IsNullOrEmpty(user.UserRoles) && user.UserRoles.Contains(roleName));
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken) {
            if (!string.IsNullOrEmpty(roleName) && user.UserRoles.Contains(roleName)) {
                var roles = user.UserRoles.Split(',').ToList();
                roles.Remove(roleName);
                user.UserRoles = string.Join(",", roles);
                await userTable.InsertOrMergeEntityAsync(user);
            }
        }

        public void Dispose() {
        }


        // --------------- IUserPasswordStore -------------
        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            await userTable.InsertOrMergeEntityAsync(user);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        //--------------- IUserEmailStore -------------
        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken)) {
            var users = await userTable.ListEntitiesByFilterAsync(new Dictionary<string, object>() {
                { "Email", normalizedEmail}
            });

            return users.FirstOrDefault();
        }

        //public async Task SetEmailAsync(User user, string email, CancellationToken cancellationToken) {
        //    user.Email = email;
        //    await userService.SaveAsync(user);
        //}

        //public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken) {
        //    return Task.FromResult(user.Email);
        //}

        //public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken) {
        //    return Task.FromResult(user.EmailConfirmed);
        //}

        //public async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken) {
        //    user.EmailConfirmed = confirmed;
        //    await userService.SaveAsync(user);
        //}


        //public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken) {
        //    return Task.FromResult(user.NormalizedEmail);
        //}

        //public async Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken) {
        //    user.NormalizedEmail = normalizedEmail;
        //    await userService.SaveAsync(user);
        //}
    }

   
}