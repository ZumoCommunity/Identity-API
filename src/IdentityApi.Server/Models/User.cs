using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace IdentityApi.Models {
    public class User : TableEntity {
        public static string DefaultPartitionKey = "Users";

        public User() {         
            PartitionKey = DefaultPartitionKey;
            RowKey = Guid.NewGuid().ToString("N");
            DateCreated = DateTime.UtcNow;
            FullName = "New User";
            Roles = "";
        }

        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Roles { get; set; }

        public DateTime DateCreated { get; set; }

        [IgnoreProperty]
        public string Id {
            get { return this.RowKey; }
            set { this.RowKey = value; }
        }


    }
}