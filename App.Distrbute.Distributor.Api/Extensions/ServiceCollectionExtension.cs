using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Hosting;
using App.Distrbute.Api.Common.Authentication;
using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Api.Common.Services.Providers;
using App.Distrbute.Common;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using App.Distrbute.Distributor.Api.Services.Providers;
using Ledgr.Sdk.Extensions;
using Logged.Sdk.Extensions;
using Messaging.Sdk.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ObjectStorage.Sdk.Extensions;
using Paystack.Sdk.Extensions;
using Pipeline.Sdk.Extensions;
using Redis.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBearerAuthAndOAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Action<BearerTokenConfig> bearerTokenConfigAction = bearerTokenConfig =>
            configuration.GetRequiredSection(nameof(BearerTokenConfig)).Bind(bearerTokenConfig);
        var bearerConfig = new BearerTokenConfig();
        bearerTokenConfigAction.Invoke(bearerConfig);

        services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = bearerConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerConfig.SigningKey)),
                    ValidAudience = bearerConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = true
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authorization = context.Request.Headers.Authorization.ToString();
                        context.Request.Cookies.TryGetValue(CommonConstants.AUTHORIZATION_COOKIE,
                            out var cookieAuthValue);

                        string? token = null;
                        if (!string.IsNullOrEmpty(authorization))
                        {
                            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                token = authorization["Bearer ".Length..];
                            else
                                token = authorization;
                        }

                        if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(cookieAuthValue))
                        {
                            if (cookieAuthValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                token = cookieAuthValue["Bearer ".Length..];
                            else
                                token = cookieAuthValue;
                        }

                        if (!string.IsNullOrEmpty(token)) context.Token = token;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = JwtIdentityPrincipalExtensions.AuthenticateJwtUserIdentity()
                };
            });


        return services;
    }

    public static IServiceCollection AddDistributorServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<BearerAuthHandler>();

        // Configurations
        // this is important, dont remove no matter what
        services.Configure<BearerTokenConfig>(c => configuration.GetRequiredSection(nameof(BearerTokenConfig)).Bind(c));
        
        services.Configure<SavingsProductConfig>(c =>
            configuration.GetRequiredSection(nameof(SavingsProductConfig)).Bind(c));
        services.Configure<MailTemplateConfig>(c =>
            configuration.GetRequiredSection(nameof(MailTemplateConfig)).Bind(c));
        
        services.AddLoggedScopedService<IAuthenticationService, AuthenticationService>();
        services.AddLoggedScopedService<IDepositToWalletService, DepositToWalletService>();
        services.AddLoggedScopedService<IDistributorService, DistributorService>();
        services.AddLoggedScopedService<IPipelineProvider, PipelineProvider>();
        services.AddLoggedScopedService<IWalletService, WalletService>();

        return services;
    }

    public static IServiceCollection AddDistributorSdks(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Logger
        services.AddLoggedSdk(configuration);

        // Add ledger
        services.AddLedgerSdk(configuration);

        // Add Paystack Sdk
        services.AddPaystackSdk(configuration);

        // Add Redis
        services.AddRedisSdk(configuration);

        // Add ElasticSearch
        // services.AddElasticSearchSdk(configuration);

        // Add scheduelr
        // services.AddSchedulerSdk(configuration);

        // Add object storage sdk
        // services.AddLocalObjectStorageSdk(configuration);
        services.AddS3ObjectStorage(configuration);

        // Add task pipeline
        services.AddPipelineSdk(configuration);

        // Add messaging
        services.AddGmailSmtpMessaging(configuration, MailTemplate.GetTemplate);

        return services;
    }

    public static IServiceCollection AddActorSystem(
        this IServiceCollection services)
    {
        var actorSystemName = Regex.Replace(Assembly.GetExecutingAssembly().GetName().Name ?? "ActorSystemName",
            @"[^a-zA-Z\s]+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        services.AddAkka(actorSystemName, builder =>
        {
            builder.WithActors((system, registry, resolver) =>
            {
                var defaultStrategy = new OneForOneStrategy(
                    3, TimeSpan.FromSeconds(3), ex =>
                    {
                        if (ex is not ActorInitializationException)
                            return Directive.Resume;

                        system?.Terminate().Wait(1000);

                        return Directive.Stop;
                    });

                // var processWalletDepositActorProps = resolver
                //     .Props<ProcessPayoutActor>()
                //     .WithSupervisorStrategy(defaultStrategy);
                //
                // var processWalletDepositActor =
                //     system.ActorOf(processWalletDepositActorProps, nameof(ProcessPayoutActor));
                // registry.Register<ProcessPayoutActor>(processWalletDepositActor);
                //
                // var processPostAutoApprovalActorProps = resolver
                //     .Props<ProcessPostAutoApprovalActor>()
                //     .WithSupervisorStrategy(defaultStrategy);
                //
                // var processPostAutoApprovalActor =
                //     system.ActorOf(processPostAutoApprovalActorProps, nameof(ProcessPostAutoApprovalActor));
                // registry.Register<ProcessPostAutoApprovalActor>(processPostAutoApprovalActor);

                system.RegisterPipelineSdkActors(registry, resolver);
            });
        });

        return services;
    }
}