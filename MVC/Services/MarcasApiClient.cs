using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuntoDeVenta.Entities.Contracts;
using PuntoDeVenta.MVC.Models;

namespace PuntoDeVenta.MVC.Services
{
    public class MarcasApiClient
    {
        private readonly string _baseUri;

        public MarcasApiClient()
        {
            _baseUri = ConfigurationManager.AppSettings["BaseUriWebApi"];
        }

        public Task<ModelResponse<List<MarcaViewModel>>> ListarAsync(string bearerToken)
        {
            return SendAsync<List<MarcaViewModel>>(bearerToken, HttpMethod.Get, "api/marcas", null);
        }

        public Task<ModelResponse<MarcaViewModel>> ConsultarAsync(string bearerToken, long id)
        {
            return SendAsync<MarcaViewModel>(bearerToken, HttpMethod.Get, $"api/marcas/{id}", null);
        }

        public Task<ModelResponse<MarcaViewModel>> CrearAsync(string bearerToken, MarcaUpsertViewModel model)
        {
            var payload = new
            {
                nombre = model?.Nombre,
                descripcion = model?.Descripcion
            };

            return SendAsync<MarcaViewModel>(bearerToken, HttpMethod.Post, "api/marcas", payload);
        }

        public Task<ModelResponse<MarcaViewModel>> ActualizarAsync(string bearerToken, MarcaUpsertViewModel model)
        {
            var payload = new
            {
                nombre = model?.Nombre,
                descripcion = model?.Descripcion
            };

            return SendAsync<MarcaViewModel>(bearerToken, HttpMethod.Put, $"api/marcas/{model?.Id}", payload);
        }

        public Task<ModelResponse> EliminarLogicoAsync(string bearerToken, long id)
        {
            return SendNoGenericAsync(bearerToken, HttpMethod.Delete, $"api/marcas/{id}");
        }

        private async Task<ModelResponse<T>> SendAsync<T>(string bearerToken, HttpMethod method, string relativeUrl, object payload)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return new ModelResponse<T>
                {
                    Success = false,
                    Message = "La sesión del servidor no tiene bearer válido.",
                    Errors = new List<string> { "Bearer faltante en sesión de servidor." }
                };
            }

            using (var request = new HttpRequestMessage(method, relativeUrl))
            using (var client = CrearCliente(bearerToken))
            {
                if (payload != null)
                {
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json");
                }

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        return new ModelResponse<T>
                        {
                            Success = false,
                            Message = "La API devolvió una respuesta vacía.",
                            Errors = new List<string> { "Respuesta vacía de API." }
                        };
                    }

                    ModelResponse<T> parsed;
                    try
                    {
                        parsed = JsonConvert.DeserializeObject<ModelResponse<T>>(body);
                    }
                    catch (Exception)
                    {
                        return new ModelResponse<T>
                        {
                            Success = false,
                            Message = "No fue posible interpretar la respuesta de la API.",
                            Errors = new List<string> { "Respuesta no válida de API." }
                        };
                    }

                    if (parsed == null)
                    {
                        return new ModelResponse<T>
                        {
                            Success = false,
                            Message = "No fue posible interpretar la respuesta de la API.",
                            Errors = new List<string> { "Respuesta no válida de API." }
                        };
                    }

                    return parsed;
                }
            }
        }

        private async Task<ModelResponse> SendNoGenericAsync(string bearerToken, HttpMethod method, string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return new ModelResponse
                {
                    Success = false,
                    Message = "La sesión del servidor no tiene bearer válido.",
                    Errors = new List<string> { "Bearer faltante en sesión de servidor." }
                };
            }

            using (var request = new HttpRequestMessage(method, relativeUrl))
            using (var client = CrearCliente(bearerToken))
            {
                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        return new ModelResponse
                        {
                            Success = false,
                            Message = "La API devolvió una respuesta vacía.",
                            Errors = new List<string> { "Respuesta vacía de API." }
                        };
                    }

                    try
                    {
                        var parsed = JsonConvert.DeserializeObject<ModelResponse>(body);
                        return parsed ?? new ModelResponse
                        {
                            Success = false,
                            Message = "No fue posible interpretar la respuesta de la API.",
                            Errors = new List<string> { "Respuesta no válida de API." }
                        };
                    }
                    catch (Exception)
                    {
                        return new ModelResponse
                        {
                            Success = false,
                            Message = "No fue posible interpretar la respuesta de la API.",
                            Errors = new List<string> { "Respuesta no válida de API." }
                        };
                    }
                }
            }
        }

        private HttpClient CrearCliente(string bearerToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_baseUri),
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
