using System.Threading.Tasks;
using Web.Client.Services;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync(string email, string password);
    Task<(bool, string)> RegisterAsync(string email, string password);
    Task LogoutAsync();
}