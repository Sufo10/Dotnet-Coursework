using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coursework.Presentation.Data.Helper
{
    public static class HttpClientExtension
    {
        

        private static HttpContent Serialize(object data)
        {
            return new StringContent(System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
                Encoding.UTF8, "application/json");
        }

        public static async Task<T> AuthGetAsync<T>(this HttpClient httpClient, string requestUri, string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }


        public static async Task<T> AuthDeleteAsync<T>(this HttpClient httpClient, string requestUri, string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        public static async Task<TResponse> AuthPostAsync<TResponse>(HttpClient httpClient, string url, HttpContent content, string token)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content,
                Headers =
        {
            Authorization = new AuthenticationHeaderValue("Bearer", token),
        },
            };

            using var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
            }
            else
            {
                throw new HttpRequestException($"Failed to send request: {response.ReasonPhrase}");
            }
        }

        public static async Task<TResponse> AuthPatchAsync<TResponse>(HttpClient httpClient, string url, HttpContent content, string token)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = content,
                Headers =
        {
            Authorization = new AuthenticationHeaderValue("Bearer", token),
        },
            };

            using var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
            }
            else
            {
                throw new HttpRequestException($"Failed to send request: {response.ReasonPhrase}");
            }
        }

        public static async Task<T>  AuthPostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data, string bearerToken) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            request.Content = Serialize(data);
            httpClient.SendAsync(request);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
