using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Web.Client.Services;

namespace Web.Client.Providers
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthService _authService;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage, AuthService authService)
        {
            _localStorage = localStorage;
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (string.IsNullOrWhiteSpace(token))
            {
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var user = _authService.GetUserFromToken(token);
            _currentUser = user;

            return new AuthenticationState(_currentUser);
        }

      

        public async Task NotifyUserLogout()
        {
            await _localStorage.RemoveItemAsync("authToken");

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }


    }
}
