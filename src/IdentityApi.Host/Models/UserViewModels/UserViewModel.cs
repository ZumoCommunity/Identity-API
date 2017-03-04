using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityApi.Models;

namespace IdentityApi.Models.UserViewModels
{
    public class UserViewModel
    {
        public UserViewModel() {

        }

        public UserViewModel(User user) {
            this.CopyFrom(user);
        }

        public string Id { get; set; }

        public string Email { get; set; }

        public UserGender Gender { get; set; } = UserGender.Undefined;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Bio { get; set; }

        public string PhotoUrl { get; set; }

        public string FaceBookUrl { get; set; }

        public string GitHubUrl { get; set; }

        public string LinkedInUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string YouTubeUrl { get; set; }

        public string MvpUrl { get; set; }

        public string WebSiteUrl { get; set; }


        public void CopyFrom(User user) {
            this.Id = user.Id;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Gender = user.Gender;
            this.FaceBookUrl = user.FaceBookUrl;
            this.GitHubUrl = user.GitHubUrl;
            this.LinkedInUrl = user.LinkedInUrl;
            this.MvpUrl = user.MvpUrl;
            this.PhotoUrl = user.PhotoUrl;
            this.TwitterUrl = user.TwitterUrl;
            this.WebSiteUrl = user.WebSiteUrl;
            this.YouTubeUrl = user.YouTubeUrl;
        }

        public void CopyTo(User user) {
            user.Email = this.Email;
            user.FirstName = this.FirstName;
            user.LastName = this.LastName;
            user.Gender = this.Gender;
            user.FaceBookUrl = this.FaceBookUrl;
            user.GitHubUrl = this.GitHubUrl;
            user.LinkedInUrl = this.LinkedInUrl;
            user.MvpUrl = this.MvpUrl;
            user.PhotoUrl = this.PhotoUrl;
            user.TwitterUrl = this.TwitterUrl;
            user.WebSiteUrl = this.WebSiteUrl;
            user.YouTubeUrl = this.YouTubeUrl;
        }

        public string FullName {
            get {
                string res = this.FirstName ?? "";
                if (!string.IsNullOrEmpty(res)) {
                    res += " ";
                }
                res += (this.LastName ?? "");
                return res;
            }
        }

    }
}
