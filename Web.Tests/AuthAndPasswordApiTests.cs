using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class PasswordApiTests
{
    private readonly HttpClient _client;

    public PasswordApiTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:5269") }; // Adapter l'URL de l'API
    }

    [Fact]
    public async Task GetPasswords_ShouldReturnList()
    {
        // Act
        var response = await _client.GetAsync("/api/password-entries");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(content)); // Vérifie que l'API renvoie bien une réponse
    }
}