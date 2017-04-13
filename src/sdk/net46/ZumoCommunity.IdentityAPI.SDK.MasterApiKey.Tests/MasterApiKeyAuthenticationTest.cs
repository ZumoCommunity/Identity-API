﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NUnit.Framework;
using ZumoCommunity.ConfigurationAPI.Infrastructure.Services;
using ZumoCommunity.ConfigurationAPI.Provider;
using ZumoCommunity.ConfigurationAPI.Readers.Common;

namespace ZumoCommunity.IdentityAPI.SDK.MasterApiKey.Tests
{
	[TestFixture]
	public class MasterApiKeyAuthenticationTest
	{
		protected static IConfigurationProvider EmptyConfigurationProvider = new ConfigurationProvider();
		protected static IConfigurationProvider ValidConfigurationProvider = new ConfigurationProvider();
		protected static string MasterApiKeyName = "MasterApiKeyName";
		protected static string MasterApiKeyValue = "1234";
		protected static string MasterApiKeyUrlParameter = "api_key";

		[OneTimeSetUp]
		public async Task InitializeAsync()
		{
			var reader = new InMemoryReader();
			await reader.SetConfigValueAsync("MasterApiKeyName", MasterApiKeyValue);

			await ValidConfigurationProvider.AddConfigurationReaderAsync(reader);
		}

		protected static IEnumerable<TestCaseData> ValidAuthenticationAsync_TestData
		{
			get
			{
				yield return new TestCaseData(ValidConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, MasterApiKeyName, MasterApiKeyValue);
				yield return new TestCaseData(ValidConfigurationProvider, AccessLevel.Anonymous, string.Empty, string.Empty, string.Empty);
				yield return new TestCaseData(EmptyConfigurationProvider, AccessLevel.Anonymous, string.Empty, string.Empty, string.Empty);
			}
		}

		[Test]
		[TestCaseSource(nameof(ValidAuthenticationAsync_TestData))]
		public async Task ValidAuthenticationAsync_Test(
			IConfigurationProvider configurationProvider,
			AccessLevel accessLevel,
			string urlParameter,
			string keyName,
			string keyValue)
		{
			var httpAuthenticationContext = SetupHttpAuthenticationContext(urlParameter, keyValue);

			var auth = new MasterApiKeyAuthentication(configurationProvider, accessLevel, keyName);
			await auth.AuthenticateAsync(httpAuthenticationContext, new CancellationToken());

			Assert.IsNull(httpAuthenticationContext.ErrorResult);
		}

		protected static IEnumerable<TestCaseData> InvalidAuthenticationAsync_TestData
		{
			get
			{
				yield return new TestCaseData(ValidConfigurationProvider, AccessLevel.Master, string.Empty, MasterApiKeyName, MasterApiKeyValue);
				yield return new TestCaseData(ValidConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, string.Empty, MasterApiKeyValue);
				yield return new TestCaseData(ValidConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, MasterApiKeyName, string.Empty);
				yield return new TestCaseData(EmptyConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, MasterApiKeyName, MasterApiKeyValue);
				yield return new TestCaseData(EmptyConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, string.Empty, MasterApiKeyValue);
				yield return new TestCaseData(EmptyConfigurationProvider, AccessLevel.Master, MasterApiKeyUrlParameter, MasterApiKeyName, string.Empty);
			}
		}

		[Test]
		[TestCaseSource(nameof(InvalidAuthenticationAsync_TestData))]
		public async Task InvalidAuthenticationAsync_Test(
			IConfigurationProvider configurationProvider,
			AccessLevel accessLevel,
			string urlParameter,
			string keyName,
			string keyValue)
		{
			var httpAuthenticationContext = SetupHttpAuthenticationContext(urlParameter, keyValue);

			var auth = new MasterApiKeyAuthentication(configurationProvider, accessLevel, keyName);
			await auth.AuthenticateAsync(httpAuthenticationContext, new CancellationToken());

			Assert.IsNotNull(httpAuthenticationContext.ErrorResult);
			Assert.IsNull(httpAuthenticationContext.Principal);
		}

		private HttpAuthenticationContext SetupHttpAuthenticationContext(string urlParameter, string keyValue)
		{
			var httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.RequestUri = new Uri($"http://localhost/?{urlParameter}={keyValue}");

			var httpControllerContext = new HttpControllerContext {Request = httpRequestMessage};

			var httpAuthenticationContext =
				new HttpAuthenticationContext(new HttpActionContext(httpControllerContext, new ReflectedHttpActionDescriptor()), null);
			return httpAuthenticationContext;
		}
	}
}