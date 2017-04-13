using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;

namespace ZumoCommunity.IdentityAPI.SDK.MasterApiKey
{
	public class MasterApiKeyAuthentication : Attribute, IAuthenticationFilter
	{
		public bool AllowMultiple => false;
		protected AccessLevel AccessLevel { get; private set; }
		protected IConfigurationProvider ConfigurationProvider { get; private set; }
		protected string MasterApiKeyName { get; private set; }

		public MasterApiKeyAuthentication(
			IConfigurationProvider configurationProvider,
			AccessLevel accessLevel = AccessLevel.Master,
			string masterApiKeyName = "MasterApiKey")
		{
			AccessLevel = accessLevel;
			ConfigurationProvider = configurationProvider;
			MasterApiKeyName = masterApiKeyName;
		}

		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			if (AccessLevel == AccessLevel.Anonymous)
			{
				return;
			}

			var request = context.Request;
			var queryParameters = request.RequestUri.ParseQueryString();
			var apiKey = queryParameters.Get("api_key");

			if (string.IsNullOrEmpty(apiKey))
			{
				context.ErrorResult = new AuthenticationFailureResult("Missing api key", request);
				return;
			}

			var masterKeyExpected = await ConfigurationProvider.GetConfigValueAsync(MasterApiKeyName);
			if (apiKey != masterKeyExpected)
			{
				context.ErrorResult = new AuthenticationFailureResult("Master authorization key is not valid", request);
				return;
			}

			context.Principal = new ClaimsPrincipal();
		}

		public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			var challenge = new AuthenticationHeaderValue("MasterApiKey");
			context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
		}
	}
}