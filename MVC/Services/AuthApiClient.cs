using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuntoDeVenta.Entities.Contracts;
using PuntoDeVenta.MVC.Models;

namespace PuntoDeVenta.MVC.Services
{
    public class AuthApiClient
    {
        private readonly string _baseUri;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AuthApiClient()
        {
            _baseUri = ConfigurationManager.AppSettings["BaseUriWebApi"];
            _clientId = ConfigurationManager.AppSettings["client_id"];
            _clientSecret = ConfigurationManager.AppSettings["client_secret"];
        }

        public async Task<TokenResponse> RequestTokenAsync(string userName, string password)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(_baseUri), Timeout = TimeSpan.FromSeconds(30) })
            {
                var form = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret)
                };

                using (var content = new FormUrlEncodedContent(form))
                using (var response = await httpClient.PostAsync("oauth/token", content).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TokenResponse>(payload);
                }
            }
        }

        public async Task<ModelResponse<AuthenticatedContextDto>> GetAuthenticatedContextAsync(string bearerToken)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(_baseUri), Timeout = TimeSpan.FromSeconds(30) })
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                using (var response = await httpClient.GetAsync("api/auth-context").ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ModelResponse<AuthenticatedContextDto>
                        {
                            Success = false,
                            Message = "No fue posible obtener el contexto autenticado."
                        };
                    }

                    var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<ModelResponse<AuthenticatedContextDto>>(payload);
                }
            }
        }
    }
}
