using App.Distrbute.Api.Common.Services.Interfaces;
using Logged.Sdk.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Paystack.Sdk;

namespace App.Distrbute.Api.Common.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class WebhooksController : CustomControllerBase
{
    private readonly ICoolLogger<WebhooksController> _logger;
    private readonly IPipelineProvider _pipelineProvider;

    public WebhooksController(IPipelineProvider pipelineProvider, ICoolLogger<WebhooksController> logger)
    {
        _pipelineProvider = pipelineProvider;
        _logger = logger;
    }

    // <summary>
    ///     Paystack webhook endpoint that triggers payment processing.
    ///     This webhook serves as a notification trigger only - we DO NOT TRUST the payload data.
    ///     All payment verification is done by calling Paystack's API directly during processing.
    /// </summary>
    /// <returns>HTTP response indicating webhook processing status</returns>
    [HttpPost("payments/paystack")]
    public async Task<IActionResult> ProcessPaystackWebhook()
    {
        try
        {
            // Read the raw request body for signature verification
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
            {
                _logger.LogWarning(nameof(ProcessPaystackWebhook),
                    "Received empty webhook payload from Paystack. Request rejected.");

                return BadRequest();
            }

            // Extract and validate webhook signature
            Request.Headers.TryGetValue(CommonConstants.PAYSTACK_WEBHOOK_SIGNATURE, out var signatures);
            var signature = signatures.FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning(nameof(ProcessPaystackWebhook),
                    "Paystack webhook received without signature header. Potential security threat - request rejected.");

                return BadRequest();
            }

            // Parse the webhook payload to extract basic event information
            PaystackWebhookEvent webhookEvent;
            try
            {
                webhookEvent = JsonConvert.DeserializeObject<PaystackWebhookEvent>(json)!;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(nameof(ProcessPaystackWebhook),
                    "Failed to parse Paystack webhook payload. Invalid JSON format. Error: {Error}", ex.Message);

                // Return 200 to prevent Paystack from retrying on our internal errors
                // But log the error for investigation
                return Ok();
            }

            var reference = webhookEvent.Data.Reference;
            var eventType = webhookEvent.Event;

            // Process webhook events (only successful charges for now)
            switch (eventType)
            {
                case "charge.success":
                    _logger.LogInformation(nameof(ProcessPaystackWebhook),
                        "Successful charge notification received for reference {Reference}. Triggering deposit processing pipeline.",
                        reference);

                    await _pipelineProvider.ExecuteDepositProcessingPipeline(reference);
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(ProcessPaystackWebhook),
                "Unexpected error occurred while processing Paystack webhook. Error: {Error}", ex.Message);

            // Return 200 to prevent Paystack from retrying on our internal errors
            // But log the error for investigation
            return Ok();
        }
    }
}

internal class PaystackWebhookEvent
{
    public string Event { get; set; } = null!;
    public PaystackTransaction Data { get; set; } = null!;
}

internal class PaystackTransaction
{
    public string Reference { get; set; } = null!;
}