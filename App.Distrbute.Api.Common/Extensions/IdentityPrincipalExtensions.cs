using System.Security.Claims;
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Common;
using App.Distrbute.Common.Exceptions;
using App.Distrbute.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Persistence.Sdk.Core.Interfaces;
using Redis.Sdk.Repositories.Interfaces;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Extensions;

public static class JwtIdentityPrincipalExtensions
{
    public static Email GetAccount(this ClaimsPrincipal principal)
    {
        var claimsIdentity =
            principal.Identities.FirstOrDefault(i => i.AuthenticationType == CommonConstants.AuthScheme.BEARER);
        var claim = claimsIdentity?.FindFirst(ClaimTypes.Thumbprint);

        if (claim == null) throw new Unauthorized("Failed to identify authentication principal");

        var authUser = JsonConvert.DeserializeObject<Email>(claim.Value)!;

        return authUser;
    }

    public static Func<TokenValidatedContext, Task> AuthenticateJwtUserIdentity()
    {
        return async ctx =>
        {
            var dbRepository = ctx.HttpContext.RequestServices.GetService<IDbRepository>();
            var redisRepository = ctx.HttpContext.RequestServices.GetService<IRedisRepository>();
            if (dbRepository == null)
            {
                ctx.Fail("Unable to resolve DB context");
                return;
            }

            var id = ctx.Principal!.FindFirst(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            if (string.IsNullOrWhiteSpace(id))
            {
                ctx.Fail("ID not found in token");
                return;
            }

            // Check cache, if exists, return here. A disabled account will be removed from cache
            var cacheKey = $"{CommonConstants.RedisKeys.UserDetailCacheKey}:{id}";
            ClaimsIdentity? identity;
            if (redisRepository != null)
            {
                var cachedUser = await redisRepository
                    .GetAsync<Email>(cacheKey)
                    .IgnoreAndDefault<Exception, Email>();

                if (cachedUser != null)
                {
                    identity = GetIdentityFrom(cachedUser);
                    ctx.Principal.AddIdentity(identity);

                    return;
                }
            }

            // if we're here, the details did not exist in cache
            var dbUser = await dbRepository
                .GetAsync<Email>(e => !e.IsDeleted && e.Address.Equals(id))
                .IgnoreAndDefault<NotFound, Email>();

            if (dbUser == null)
            {
                ctx.Fail("User not found");
                return;
            }

            // cache retrieved user
            if (redisRepository != null)
                await redisRepository
                    .SetAsync(cacheKey, dbUser)
                    .Ignore<Exception>();

            identity = GetIdentityFrom(dbUser);
            ctx.Principal.AddIdentity(identity);
        };
    }

    private static ClaimsIdentity GetIdentityFrom(Email email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Thumbprint, JsonConvert.SerializeObject(email)),
            new(ClaimTypes.Authentication, CommonConstants.AuthScheme.BEARER)
        };
        var identity = new ClaimsIdentity(claims, CommonConstants.AuthScheme.BEARER);

        return identity;
    }
}