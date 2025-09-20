using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Services.Interfaces;
using Logged.Sdk.Core;
using Microsoft.AspNetCore.Mvc;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
public class WebhooksController : WebhooksControllerBase
{
    public WebhooksController(IPipelineProvider pipelineProvider, ICoolLogger<WebhooksControllerBase> logger) : base(pipelineProvider, logger)
    {
    }
}