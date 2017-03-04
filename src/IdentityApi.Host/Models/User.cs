using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace IdentityApi.Models {
    public class User : TableEntity {
        public static string DefaultPartitionKey = "Users";

        public User() {         
            PartitionKey = DefaultPartitionKey;
            RowKey = Guid.NewGuid().ToString("N");
            DateCreated = DateTime.UtcNow;
            FirstName = "New";
            LastName = "User";
            Roles = "";
        }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public string Roles { get; set; }

        public DateTime DateCreated { get; set; }

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


        [IgnoreProperty]
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



        [IgnoreProperty]
        public string Id {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }
    }

    public enum UserGender {
        Undefined = 0,
        Mail = 1,
        Female = 2
    }
}