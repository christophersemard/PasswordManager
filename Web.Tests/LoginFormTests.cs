using Bunit;
using Xunit;
using Web.Client.Pages.Auth; // Adapter selon l'emplacement de LoginForm

public class LoginFormTests : TestContext
{
    [Fact]
    public void LoginForm_ShouldRenderWithoutErrors()
    {
        // Act - Rendu du composant
        var cut = RenderComponent<LoginForm>();

        // Assert - Vérifie que le composant s'affiche bien
        Assert.NotNull(cut.Markup);
    }

    [Fact]
    public void LoginForm_ShouldHaveEmailAndPasswordFields()
    {
        // Act
        var cut = RenderComponent<LoginForm>();

        // Assert - Vérifie que les champs email et mot de passe existent
        var emailField = cut.Find("input[type='text']");
        var passwordField = cut.Find("input[type='password']");

        Assert.NotNull(emailField);
        Assert.NotNull(passwordField);
    }

    [Fact]
    public void LoginForm_ShouldHaveLoginButton()
    {
        // Act
        var cut = RenderComponent<LoginForm>();

        // Assert - Vérifie que le bouton de connexion est présent
        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Contains("Se connecter", button.TextContent);
    }
}