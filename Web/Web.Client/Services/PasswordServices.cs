using System.Net.Http.Json;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Web.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<LoginResult?> LoginAsync(string email, string password)
        {
            var loginData = new { email, password };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (result?.Token != null)
                {
                    await _localStorage.SetItemAsync("authToken", result.Token);
            
                    return result; 
                }
            }
            return null;
        }
        
        public async Task<(bool, string)> RegisterAsync(string email, string password)
        {
            var registerData = new { email, password };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);
            if (response.IsSuccessStatusCode)
            {
                return (true, "");
            }
            else
            {
                // Convertir la réponse en JSON et renvoyer la clé message
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (error.ContainsKey("message"))
                {
                    Console.WriteLine(error["message"]);
                    return (false, error["message"]);
                }
                else
                {
                    return (false, "Une erreur inconnue s'est produite.");
                }
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            
            
        }


        public ClaimsPrincipal GetUserFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return new ClaimsPrincipal(new ClaimsIdentity());

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

    }
    


    public class LoginResult
    {
        public string Token { get; set; }
    }
}
