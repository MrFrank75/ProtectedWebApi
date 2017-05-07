using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string rawJwtToken = RequestTokenToAuthorizationServer(new Uri("http://localhost:50151/connect/token"), "ClientIdThatCanOnlyRead", "scope.readaccess", "secret1")
                .GetAwaiter()
                .GetResult();

            AuthorizationServerAnswer authorizationServerToken;
            authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(rawJwtToken);

            Console.WriteLine("Token acquired from Authorization Server:");
            Console.WriteLine(authorizationServerToken.access_token);

            string response = RequestValuesToSecuredWebApi(authorizationServerToken)
                .GetAwaiter()
                .GetResult();

            Console.WriteLine("Response received from WebAPI:");
            Console.WriteLine(response);
            Console.ReadKey();
        }

        private static async Task<string> RequestValuesToSecuredWebApi(AuthorizationServerAnswer authorizationServerToken)
        {
            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationServerToken.access_token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:56087/api/values");
                responseMessage = await httpClient.SendAsync(request);
            }

            return await responseMessage.Content.ReadAsStringAsync();
        }

        private static async Task<string> RequestTokenToAuthorizationServer(Uri uriAuthorizationServer, string clientId, string scope, string clientSecret)
        {
            HttpResponseMessage responseMessage;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = uriAuthorizationServer;
                client.DefaultRequestHeaders
                    .Accept
                    .Clear();
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("scope", scope),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                    });

                responseMessage = await client.PostAsync(uriAuthorizationServer, httpContent);
            }            
            return await responseMessage.Content.ReadAsStringAsync();
        }

        private class AuthorizationServerAnswer
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }

        }
    }
}