using System.Net.Http.Json;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;

namespace Web.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly NavigationManager _navigationManager;
        private byte[] _encryptionKey;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, ISessionStorageService sessionStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
        }

        public async Task<LoginResult?> LoginAsync(string email, string password)
        {
            var loginData = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Échec de la connexion");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (result?.Token == null) return null;

            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("userSalt", result.Salt);

            // 🔑 Dériver la clé AES avec PBKDF2 (mot de passe + salt)
            _encryptionKey = await DeriveKeyFromPasswordAsync(password, result.Salt);
            await _sessionStorage.SetItemAsync("encryptionKey", Convert.ToBase64String(_encryptionKey));
            Console.WriteLine("✅ Clé AES stockée en SessionStorage.");

            return result;
        }
        
        public async Task<(bool, string)> RegisterAsync(string email, string password)
        {
            var registerData = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);

            if (response.IsSuccessStatusCode) return (true, "");

            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return error != null && error.ContainsKey("message")
                ? (false, error["message"])
                : (false, "Une erreur inconnue s'est produite.");
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _encryptionKey = null; // 🔐 Supprime la clé AES de la mémoire
        }

        
        public async Task LogoutAndRedirect()
        {
            Console.WriteLine("🔹 Déconnexion forcée : Clé AES introuvable.");
        
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("userSalt");
            await _sessionStorage.RemoveItemAsync("encryptionKey");

            _encryptionKey = null;

            _navigationManager.NavigateTo("/connexion", forceLoad: true);
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

        public byte[] GetEncryptionKey() => _encryptionKey; // 🔥 Permet à `PasswordService` d’accéder à la clé

        // 🔹 Dérivation de la clé AES avec PBKDF2
        private static async Task<byte[]> DeriveKeyFromPasswordAsync(string password, string base64Salt)
        {
            byte[] salt = Convert.FromBase64String(base64Salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // Génère une clé de 256 bits pour AES
        }
        
        public async Task<bool> RestoreEncryptionKey()
        {
            var keyBase64 = await _sessionStorage.GetItemAsync<string>("encryptionKey");

            if (!string.IsNullOrWhiteSpace(keyBase64))
            {
                _encryptionKey = Convert.FromBase64String(keyBase64);
                Console.WriteLine("✅ Clé AES restaurée depuis SessionStorage.");
                return true;
            }

            Console.WriteLine("❌ Clé AES introuvable dans SessionStorage, l’utilisateur doit se reconnecter.");
            return false;
        }


    }

    
    
    public class LoginResult
    {
        public string Token { get; set; }
        public string Salt { get; set; }
    }

}
