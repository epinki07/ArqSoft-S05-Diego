using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class PacienteService
{
    private readonly IPacienteRepository _pacienteRepo;

    public PacienteService(IPacienteRepository pacienteRepo)
    {
        _pacienteRepo = pacienteRepo;
    }

    public IEnumerable<Paciente> ObtenerTodos() => _pacienteRepo.ObtenerTodos();

    public Paciente? ObtenerPorId(int id)
    {
        var paciente = _pacienteRepo.ObtenerPorId(id);
        return paciente.Id == 0 ? null : paciente;
    }
}
