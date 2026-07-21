using CitasApp.Services;

namespace CitasApp.Tests;

public class PasswordHashServiceTests
{
    [Fact]
    public void Verify_CuandoPasswordEsCorrecto_RegresaTrue()
    {
        // Arrange
        var password = "Password123!";
        var hash = PasswordHashService.Hash(password);

        // Act
        var resultado = PasswordHashService.Verify(password, hash);

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public void Verify_CuandoPasswordEsIncorrecto_RegresaFalse()
    {
        // Arrange
        var hash = PasswordHashService.Hash("Password123!");

        // Act
        var resultado = PasswordHashService.Verify("OtraPassword123!", hash);

        // Assert
        Assert.False(resultado);
    }
}
