using System;
using System.Text;
using Xunit;
using Web.Client.Services; // Adapter selon l'emplacement de EncryptionService

public class EncryptionServiceTests
{
    [Fact]
    public void Encrypt_And_Decrypt_ShouldReturnOriginalText()
    {
        // Arrange
        string originalText = "MonSuperMotDePasse123!";
        byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); // Clé AES de 16 bytes

        // Act
        string encrypted = EncryptionService.Encrypt(originalText, key);
        byte[] encryptedBytes = Convert.FromBase64String(encrypted);
        string decrypted = EncryptionService.Decrypt(encryptedBytes, key);

        // Assert
        Assert.Equal(originalText, decrypted);
    }
}