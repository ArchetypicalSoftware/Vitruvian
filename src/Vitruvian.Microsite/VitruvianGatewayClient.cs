using Archetypical.Software.Vitruvian.Common.Models;
using Archetypical.Software.Vitruvian.Common.Models.Commands;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public class VitruvianGatewayClient : IVitruvianGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly MicrositeConfiguration _configuration;

        public VitruvianGatewayClient(HttpClient httpClient, MicrositeConfiguration micrositeConfiguration)
        {
            _httpClient = httpClient;
            _configuration = micrositeConfiguration;
        }

        public async Task SendCommand(BaseCommand command)
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration.VitruvianGatewayUri, "/vitruvian/admin"),
                Content = new StringContent(JsonConvert.SerializeObject(command,
                    SerializerSettings.CommandJsonSerializerSettings), System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(req);
            response.EnsureSuccessStatusCode();
        }
    }
}