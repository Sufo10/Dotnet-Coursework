using System;
using System.Text;
using System.Text.Json;

namespace Coursework.Presentation.Data.Helper
{
	public static class HttpClientExtension
	{

        private static readonly string baseURL = "https://localhost:7190/";
        private static HttpContent Serialize(object data) => new StringContent(
        JsonSerializer.Serialize(data, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8,
        "application/json");


        public static Task<HttpResponseMessage> AuthGetAsync(this HttpClient httpClient, string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, baseURL+requestUri);
            return httpClient.SendAsync(request);
        }

        public static Task<HttpResponseMessage> AuthPostAsync(this HttpClient httpClient, string requestUri, HttpContent? content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseURL+requestUri);
            request.Content = content;
            return httpClient.SendAsync(request);
        }


        public static Task<HttpResponseMessage> AuthPostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = Serialize(data);
            return httpClient.SendAsync(request);
        }
    }
}

