using System.Reflection;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Common;
using Logged.Sdk.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace App.Distrbute.Api.Common.Extensions;

public static class WebApplicationExtensions
{
    public static void UseCustomSwagger(this WebApplication app, string? prefix = null)
    {
        var apiDocsConfig = app.Services.GetRequiredService<IOptions<ApiDocsConfig>>().Value;
        var apiVersionDescription = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        var effectivePrefix = prefix ?? app.Environment.ApplicationName;

        if (apiDocsConfig.ShowSwaggerUi)
        {
            app.UseSwagger(c => { c.RouteTemplate = $"/{effectivePrefix}/swagger/{{documentName}}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                var projName = Assembly.GetCallingAssembly().GetName().Name;
                foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
                    c.SwaggerEndpoint($"/{effectivePrefix}/swagger/{description.GroupName}/swagger.json",
                        $"{projName} - {description.GroupName}");

                if (apiDocsConfig.EnableSwaggerTryIt && app.Environment.IsDevelopment())
                    c.SupportedSubmitMethods(SubmitMethod.Post, SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Patch,
                        SubmitMethod.Delete);
            });
        }

        if (apiDocsConfig.ShowRedocUi)
            foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
                app.UseReDoc(options =>
                {
                    options.DocumentTitle = Assembly.GetCallingAssembly().GetName().Name;
                    options.RoutePrefix = $"api-docs-{description.GroupName}";
                    options.SpecUrl = $"/{effectivePrefix}/swagger/{description.GroupName}/swagger.json";
                });
    }

    public static async Task RunMigrationsAsync<T>(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var logger = app.Services.GetRequiredService<ICoolLogger<T>>();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                // db migrations
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                var count = pendingMigrations.Count();
                if (count > 0)
                {
                    logger.LogInformation(nameof(RunMigrationsAsync),
                        $"found {count} pending migrations to apply. will proceed to apply them");
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation(nameof(RunMigrationsAsync), "done applying pending migrations");
                }
                else
                {
                    logger.LogInformation(nameof(RunMigrationsAsync), "no pending migrations found! :)");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(RunMigrationsAsync), "An error occurred while performing migration.");
                throw;
            }
        }
    }
}