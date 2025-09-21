using System.Reflection;
using System.Text;
using System.Text.Json;
using App.Distrbute.Api.Common.Authentication;
using App.Distrbute.Api.Common.Middlewares;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Api.Common.Services.Providers;
using App.Distrbute.Common;
using Logged.Sdk.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Api.Common.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBearerWithBasicAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Action<BearerTokenConfig> bearerTokenConfigAction = bearerTokenConfig =>
            configuration.GetRequiredSection(nameof(BearerTokenConfig)).Bind(bearerTokenConfig);
        var bearerConfig = new BearerTokenConfig();
        bearerTokenConfigAction.Invoke(bearerConfig);
        
        services.Configure<BasicAuthConfig>(c =>
            configuration.GetRequiredSection(nameof(BasicAuthConfig)).Bind(c));

        services.AddAuthentication()
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
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(CommonConstants.AuthScheme.BASIC, null);

        return services;
    }

    public static IServiceCollection AddSwaggerGen(
        this IServiceCollection services,
        IConfiguration configuration,
        string schemeName)
    {
        services.Configure<ApiDocsConfig>(c => configuration.GetRequiredSection(nameof(ApiDocsConfig)).Bind(c));

        services.ConfigureOptions<ConfigureSwaggerOptions>();

        var projName = Assembly.GetCallingAssembly().GetName().Name;

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
            {
                Description = $@"Enter '[schemeName]' [space] and then your token in the text input below.<br/>
                      Example: '{schemeName} 12345abcdef'",
                Name = CommonConstants.AUTHORIZATION,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = schemeName
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeName
                        },
                        Scheme = "oauth2",
                        Name = schemeName,
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });

            c.DocumentFilter<AdditionalParametersDocumentFilter>();

            c.ResolveConflictingActions(descriptions => { return descriptions.First(); });

            var xmlFile = $"{projName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddApiVersioning(this IServiceCollection services, int version)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(version, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                options.Filters.Add<DtoValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        return services;
    }
    
    public static IServiceCollection AddPriceConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PriceConfig>(c => configuration.GetRequiredSection(nameof(PriceConfig)).Bind(c));

        return services;
    }
    
    public static IServiceCollection AddPostTracking(this IServiceCollection services)
    {
        services.AddLoggedScopedService<IPostMetricService, PostMetricService>();
        services.AddLoggedScopedService<IPostValuationService, PostValuationService>();
        services.AddLoggedScopedService<IPostValuationWriter, PostValuationWriter>();
        services.AddLoggedScopedService<ISocialAccountValuationService, SocialAccountValuationService>();

        return services;
    }
}