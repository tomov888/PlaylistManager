﻿using System;
using System.Text;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PlaylistManager.Api.Serverless.AzureFunctions.Middleware;
using PlaylistManager.Core.Contracts.Models.Authentication;
using PlaylistManager.Core.Contracts.Repository;
using PlaylistManager.Core.Contracts.UseCase;
using PlaylistManager.Core.Services.Authentication;
using PlaylistManager.Core.Services.Authentication.JsonWebToken;
using PlaylistManager.Core.Services.UseCases;
using PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions;
using PlaylistManager.Infrastructure.Api.Service;
using PlaylistManager.Infrastructure.Repository.AzureTableStorage.EntityRepository;
using TokenHandler = PlaylistManager.Core.Services.Authentication.JsonWebToken.TokenHandler;

[assembly: FunctionsStartup(typeof(Startup))]

namespace PlaylistManager.Infrastructure.Api.Serverless.AzureFunctions;

public class Startup : FunctionsStartup
{
	public override void Configure(IFunctionsHostBuilder builder)
	{
		var configuration = builder.GetContext().Configuration;
		
		var authConfig = new AuthenticationConfiguration
		{
			Issuer = "PlaylistManager",
			Secret = "super-secret-super-secret-super-secret-super-secret-super-secret",
			TokenLifetime = TimeSpan.FromMinutes(60)
		};
		builder.Services.AddSingleton(authConfig);
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("super-secret-super-secret-super-secret-super-secret-super-secret")),
			ValidateIssuer = false,
			ValidateAudience = false,
			RequireExpirationTime = false,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};

		builder.Services.AddSingleton(tokenValidationParameters);
		
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<ITrackRepository, TrackRepository>();
		builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
		builder.Services.AddScoped<IUserLoginService, UserLoginService>();
		builder.Services.AddScoped<IGetTracksUseCase, GetTracksUseCase>();
		
		builder.Services.AddScoped<IJwtService, JwtService>();
		builder.Services.AddScoped<ITokenHandler, TokenHandler>();
		builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
		
		builder.Services.AddScoped<AzureFunctionsHttpMiddlewarePipelineFactory>();

		builder.Services.AddAzureClients(azureClientsBuilder =>
		{
			azureClientsBuilder.AddClient<TableServiceClient, TableClientOptions>((options, _, _) =>
			{
				return new TableServiceClient("UseDevelopmentStorage=true", new TableClientOptions { Retry = { MaxRetries = 5 } });
			});

			// azureClientsBuilder.AddClient<QueueClient, QueueClientOptions>((options, _, _) =>
			// {
			// 	options.Diagnostics.IsLoggingEnabled = false;
			// 	options.MessageEncoding = QueueMessageEncoding.Base64;
			//
			// 	var queueClient = new QueueClient(azureStorageQueueSettings.AzureStorageAccount, azureStorageQueueSettings.QueueName, options);
			//
			// 	queueClient.CreateIfNotExists();
			//
			// 	return queueClient;
			// });
		});
	}
}