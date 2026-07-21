using CitasApp.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace CitasApp.Tests;

public class RepositoryFactoryTests
{
    [Fact]
    public void CrearPacienteRepository_CuandoEsProduccion_RegresaRepositorioEnMemoria()
    {
        // Arrange
        var env = new FakeWebHostEnvironment();

        // Act
        var repository = RepositoryFactory.CrearPacienteRepository(Environments.Production, env);

        // Assert
        Assert.IsType<MemoriaPacienteRepository>(repository);
    }

    [Fact]
    public void CrearPacienteRepository_CuandoNoEsProduccion_RegresaRepositorioJson()
    {
        // Arrange
        var env = new FakeWebHostEnvironment();

        // Act
        var repository = RepositoryFactory.CrearPacienteRepository(Environments.Development, env);

        // Assert
        Assert.IsType<JsonPacienteRepository>(repository);
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "CitasApp.Tests";

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();

        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();

        public string EnvironmentName { get; set; } = Environments.Development;

        public string WebRootPath { get; set; } = Directory.GetCurrentDirectory();

        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
