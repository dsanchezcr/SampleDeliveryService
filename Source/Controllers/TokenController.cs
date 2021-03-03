using System;
using Azure.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Services.AppAuthentication;

namespace SampleDeliveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("SessionToken")]
    public class TokenController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [EnableCors("LocalAzure")]
        public async Task<IActionResult> GetTokenAsync()
        {
            var client = new SecretClient(new Uri("Key Vault URL"), new DefaultAzureCredential());
            var secret = await client.GetSecretAsync("Maps Connection String Secret");

            AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider(secret.Value.Value);

            string accessToken = await tokenProvider.GetAccessTokenAsync("https://atlas.microsoft.com/", cancellationToken: HttpContext.RequestAborted);
            return Ok(accessToken);
        }
    }
}