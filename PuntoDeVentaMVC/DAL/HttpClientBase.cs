using Newtonsoft.Json;
using PuntoDeVentaEntities.Seguridad;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PuntoDeVentaMVC.DAL
{
    /// <summary>
    /// Motor HTTP genérico del front. Nunca lanza excepciones hacia el llamador:
    /// ante errores HTTP devuelve un <see cref="ModelResponse{T}"/> con IsSuccess=false.
    /// </summary>
    public class HttpClientBase
    {
        private readonly HttpClient httpClient;
        private readonly string BaseUri;

        public HttpClientBase(string baseUrl)
        {
            BaseUri = string.IsNullOrEmpty(baseUrl)
                ? ConfigurationManager.AppSettings["BaseUriWebApi"]
                : baseUrl;

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUri),
                Timeout = TimeSpan.FromMinutes(5)
            };
        }

        /// <summary>Obtiene un token OAuth (form-urlencoded). Devuelve default(T) si falla.</summary>
        public async Task<T> TokenAsync<T>(string endPoint, IEnumerable<KeyValuePair<string, string>> content, string contentType = "application/json")
        {
            SetParametersHttpCliente(contentType, string.Empty);

            using (var httpResponseMessage = await httpClient.PostAsync(endPoint, new FormUrlEncodedContent(content)))
            {
                var body = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Log.Information("TokenAsync OK: Endpoint={Endpoint}, StatusCode={StatusCode}", endPoint, (int)httpResponseMessage.StatusCode);
                    return JsonConvert.DeserializeObject<T>(body);
                }

                Log.Warning("TokenAsync FALLO: Endpoint={Endpoint}, StatusCode={StatusCode}, Body={Body}", endPoint, (int)httpResponseMessage.StatusCode, body);
                return default(T);
            }
        }

        /// <summary>Petición cruda con mapper: el llamador decide cómo interpretar el body.</summary>
        public async Task<T> RequestAsync<T>(string endPoint, HttpMethod method, T content, Func<string, T> func, string token = "", string contentType = "application/json") where T : class
        {
            SetParametersHttpCliente(contentType, token);

            using (var r = new HttpRequestMessage
            {
                Content = content != null ? new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, contentType) : null,
                Method = method,
                RequestUri = new Uri(httpClient.BaseAddress, endPoint)
            })
            using (var responseMessage = await httpClient.SendAsync(r))
            {
                var stringContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    return func?.Invoke(stringContent);
                }

                var error = new
                {
                    IsSuccess = false,
                    Message = $"Error {(int)responseMessage.StatusCode} ({responseMessage.ReasonPhrase}) al consumir {endPoint}.",
                    Response = (object)null
                };
                return func?.Invoke(JsonConvert.SerializeObject(error));
            }
        }

        /// <summary>Petición tipada estándar: deserializa la respuesta como ModelResponse&lt;TResponse&gt;.</summary>
        public async Task<ModelResponse<TResponse>> RequestAsync<TResponse>(string endPoint, HttpMethod method, object content, string token = "", string contentType = "application/json")
        {
            SetParametersHttpCliente(contentType, token);

            using (var r = new HttpRequestMessage
            {
                Content = content != null ? new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, contentType) : null,
                Method = method,
                RequestUri = new Uri(httpClient.BaseAddress, endPoint)
            })
            using (var responseMessage = await httpClient.SendAsync(r))
            {
                var stringContent = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ModelResponse<TResponse>>(stringContent);
                }

                return new ModelResponse<TResponse>
                {
                    IsSuccess = false,
                    Message = $"Error {(int)responseMessage.StatusCode} ({responseMessage.ReasonPhrase}) al consumir {endPoint}.",
                    Response = default(TResponse)
                };
            }
        }

        /// <summary>Descarga de contenido binario. Devuelve null si la respuesta no es exitosa.</summary>
        public async Task<byte[]> RequestAsyncByteArray(string endPoint, HttpMethod method, object content, string token = "", string contentType = "application/json")
        {
            SetParametersHttpCliente(contentType, token);

            using (var r = new HttpRequestMessage
            {
                Content = content != null ? new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, contentType) : null,
                Method = method,
                RequestUri = new Uri(httpClient.BaseAddress, endPoint)
            })
            using (var responseMessage = await httpClient.SendAsync(r))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
        }

        /// <summary>Envía contenido multipart y deserializa la respuesta como ModelResponse&lt;T&gt;.</summary>
        public async Task<ModelResponse<T>> SendMultipartAsync<T>(string endPoint, MultipartFormDataContent content, string token = "")
        {
            SetParametersHttpCliente("application/json", token);

            try
            {
                using (var responseMessage = await httpClient.PostAsync(endPoint, content))
                {
                    var stringContent = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ModelResponse<T>>(stringContent);
                    }

                    Log.Warning("SendMultipartAsync FALLO: Endpoint={Endpoint}, StatusCode={StatusCode}, Body={Body}", endPoint, (int)responseMessage.StatusCode, stringContent);
                    return new ModelResponse<T>
                    {
                        IsSuccess = false,
                        Message = $"Error {(int)responseMessage.StatusCode} ({responseMessage.ReasonPhrase}) al consumir {endPoint}.",
                        Response = default(T)
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SendMultipartAsync EXCEPCION: Endpoint={Endpoint}", endPoint);
                return new ModelResponse<T>
                {
                    IsSuccess = false,
                    Message = $"No se pudo enviar la solicitud a {endPoint}: {ex.Message}",
                    Response = default(T)
                };
            }
        }

        private void SetParametersHttpCliente(string contentType, string token)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
