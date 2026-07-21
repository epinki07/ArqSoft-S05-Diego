using CitasApp.Interfaces;
using CitasApp.Models;
using CitasApp.Services;

namespace CitasApp.Tests;

public class CitaServiceTests
{
    [Fact]
    public void ConfirmarCita_CuandoExisteYPendiente_CambiaEstadoYNotifica()
    {
        // Arrange
        var cita = new Cita
        {
            Id = 1,
            PacienteId = 10,
            MedicoId = 20,
            Fecha = new DateOnly(2026, 7, 21),
            Hora = new TimeOnly(10, 0),
            Motivo = "Consulta",
            Estado = "Pendiente"
        };
        var citaRepository = new FakeCitaRepository([cita]);
        var observer = new FakeCitaObserver();
        var service = new CitaService(
            citaRepository,
            new FakePacienteRepository(),
            new FakeMedicoRepository(),
            [observer]);

        // Act
        var resultado = service.ConfirmarCita(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Confirmada", resultado.Estado);
        Assert.Single(citaRepository.CitasEditadas);
        Assert.Single(observer.CitasConfirmadas);
        Assert.Equal(1, observer.CitasConfirmadas[0].Id);
    }

    [Fact]
    public void ConfirmarCita_CuandoNoExiste_RegresaNullYNoEdita()
    {
        // Arrange
        var citaRepository = new FakeCitaRepository([]);
        var observer = new FakeCitaObserver();
        var service = new CitaService(
            citaRepository,
            new FakePacienteRepository(),
            new FakeMedicoRepository(),
            [observer]);

        // Act
        var resultado = service.ConfirmarCita(99);

        // Assert
        Assert.Null(resultado);
        Assert.Empty(citaRepository.CitasEditadas);
        Assert.Empty(observer.CitasConfirmadas);
    }

    private sealed class FakeCitaRepository : ICitaRepository
    {
        private readonly List<Cita> _citas;

        public FakeCitaRepository(IEnumerable<Cita> citas)
        {
            _citas = citas.ToList();
        }

        public List<Cita> CitasEditadas { get; } = [];

        public IEnumerable<Cita> ObtenerTodos() => _citas;

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId) =>
            _citas.Where(cita => cita.PacienteId == pacienteId);

        public Cita ObtenerPorId(int id) =>
            _citas.FirstOrDefault(cita => cita.Id == id) ?? new Cita();

        public void Agregar(Cita cita) => _citas.Add(cita);

        public void Editar(Cita cita) => CitasEditadas.Add(cita);

        public void Eliminar(int id) => _citas.RemoveAll(cita => cita.Id == id);
    }

    private sealed class FakePacienteRepository : IPacienteRepository
    {
        public IEnumerable<Paciente> ObtenerTodos() => [];

        public Paciente ObtenerPorId(int id) => new();

        public void Agregar(Paciente paciente)
        {
        }

        public void Editar(Paciente paciente)
        {
        }

        public void Eliminar(int id)
        {
        }
    }

    private sealed class FakeMedicoRepository : IMedicoRepository
    {
        public IEnumerable<Medico> ObtenerTodos() => [];

        public Medico ObtenerPorId(int id) => new();

        public void Agregar(Medico medico)
        {
        }

        public void Editar(Medico medico)
        {
        }

        public void Eliminar(int id)
        {
        }
    }

    private sealed class FakeCitaObserver : ICitaObserver
    {
        public List<Cita> CitasConfirmadas { get; } = [];

        public void OnCitaConfirmada(Cita cita) => CitasConfirmadas.Add(cita);
    }
}
