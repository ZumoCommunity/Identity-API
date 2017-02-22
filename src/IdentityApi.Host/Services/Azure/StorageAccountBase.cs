using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Korzh.WindowsAzure.Storage;

namespace Korzh.WindowsAzure.Storage
{

    public static class AzureStorageConfig {
        public static string ConnectionString = ""; //"UseDevelopmentStorage=true";
    }

	public abstract class StorageServiceBase
	{
		protected readonly CloudStorageAccount Account;

        protected StorageServiceBase() : this(AzureStorageConfig.ConnectionString) {
        }

        protected StorageServiceBase(string connectionString) {
			Account = CloudStorageAccount.Parse(connectionString);
		}

		protected StorageServiceBase(string name, string key, bool isHttps = true)
		{
			var credentials = new StorageCredentials(name, key);

			Account = new CloudStorageAccount(credentials, isHttps);
		}
	}
}
