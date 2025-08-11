using Microsoft.Extensions.Configuration;
using OmniSuite.Infrastructure.Services.FlowpagService.Request;
using OmniSuite.Infrastructure.Services.FlowpagService.Responses;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OmniSuite.Infrastructure.Services.FlowpagService
{
    public interface IFlowpagService
    {
        Task<PixDepositResponse> CreatePixDepositAsync(PixDepositRequest request);

        Task<WithdrawPixResponse> CreatePixWithdrawalAsync(WithdrawPixRequest request);
    }
    public class FlowpagService : IFlowpagService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FlowpagService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var clientId = _configuration["Flowpag:ClientId"];
            var clientSecret = _configuration["Flowpag:ClientSecret"];

            var requestBody = new
            {
                client_id = clientId,
                client_secret = clientSecret
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.flowpag.com/auth", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao autenticar na FlowPag: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<FlowpagAuthResponse>();
            return result.AccessToken;
        }

        public async Task<PixDepositResponse> CreatePixDepositAsync(PixDepositRequest request)
        {
            var token = await GetAccessTokenAsync();

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.flowpag.com/deposit-pix")
            {
                Content = JsonContent.Create(request, options: options)
            };

            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar cobrança PIX: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<PixDepositResponse>();

            return result;
        }

        public async Task<WithdrawPixResponse> CreatePixWithdrawalAsync(WithdrawPixRequest request)
        {
            var token = await GetAccessTokenAsync();

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.flowpag.com/withdraw")
            {
                Content = JsonContent.Create(request, options: options)
            };

            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao realizar retirada PIX: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<WithdrawPixResponse>();

            return result;
        }
    }
}
