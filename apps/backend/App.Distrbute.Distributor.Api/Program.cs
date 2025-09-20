using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Api.Common.Handlers;
using App.Distrbute.Common;
using App.Distrbute.Distributor.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Sdk.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var corsPolicyName = "App.Distrbute.Distributor.Api.PolicyName";

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

services.AddDistributorServices(config);
services.AddDistributorSdks(config);
services.AddPriceConfig(config);

services.AddPersistenceSdk<ApplicationDbContext>(config);

services.AddValidatorsFromAssemblyContaining<Program>();

services.AddBearerWithBasicAuthAndOAuth(config);

services.AddSwaggerGen(config, CommonConstants.AuthScheme.BEARER);

services.AddHealthChecks();

services.AddCors(options => options
    .AddPolicy(corsPolicyName, policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

services.AddCustomControllers();

services.AddApiVersioning(1);

services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddActorSystem();

var app = builder.Build();

await app.RunMigrationsAsync<Program>();

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedProto });

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseCustomSwagger("api/distributor");
app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();

app.UseAuthorization();


app.UseExceptionHandler();

app.MapHealthChecks("/health");
app.MapControllers();


await app.RunAsync();