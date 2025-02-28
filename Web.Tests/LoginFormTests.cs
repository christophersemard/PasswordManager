using Bunit;
using Moq;
using Xunit;
using Web.Client.Components.Auth;
using Web.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services; // Ajout MudBlazor

public class LoginFormTests : TestContext
{
    public LoginFormTests()
    {
        // 🔹 Ajouter MudBlazor Services pour éviter l’erreur `InternalMudLocalizer`
        Services.AddMudServices();
    }

    [Fact]
    public void LoginForm_ShouldRenderWithoutErrors()
    {
        // Arrange - Mock de IAuthService et NavigationManager
        var mockAuthService = new Mock<IAuthService>();
        var mockNavigationManager = new Mock<NavigationManager>();

        Services.AddSingleton(mockAuthService.Object);
        Services.AddSingleton(mockNavigationManager.Object);

        // Act
        var cut = RenderComponent<LoginForm>();

        // Assert
        Assert.NotNull(cut.Markup);
    }
}