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
            AuthorizationServerAnswer authorizationServerToken;
            JwtSecurityToken jwtSecurityToken;

            string tokenResult = GetHttpResponseAsync(new Uri("http://localhost:50151/connect/token"), "ClientIdThatCanOnlyRead", "scope.readaccess", "secret1")
                .GetAwaiter()
                .GetResult();
            authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(tokenResult);


            var jwtTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityToken = jwtTokenHandler.ReadJwtToken(authorizationServerToken.access_token);

            Console.WriteLine("Token acquired from Authorization Server:");
            Console.WriteLine(tokenResult);

            HttpResponseMessage responseMessage;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationServerToken.access_token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:50150/api/values");
                responseMessage = httpClient.SendAsync(request).GetAwaiter().GetResult();
            }

            Console.WriteLine("Reponse received:");
            string task = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Console.WriteLine(task);
            Console.ReadKey();
        }

        private static async Task<string> GetHttpResponseAsync(Uri uriAuthorizationServer, string clientId, string scope, string clientSecret)
        {
            HttpClient client = new HttpClient();
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

            HttpResponseMessage postResponse = await client.PostAsync(uriAuthorizationServer, httpContent);

            return await postResponse.Content.ReadAsStringAsync();
        }

        private class AuthorizationServerAnswer
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }

        }
    }
}