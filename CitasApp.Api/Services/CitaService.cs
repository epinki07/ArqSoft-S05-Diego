using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class CitaService
{
    private readonly ICitaRepository _citaRepo;
    private readonly IPacienteRepository _pacienteRepo;
    private readonly IMedicoRepository _medicoRepo;

    public CitaService(
        ICitaRepository citaRepo,
        IPacienteRepository pacienteRepo,
        IMedicoRepository medicoRepo)
    {
        _citaRepo = citaRepo;
        _pacienteRepo = pacienteRepo;
        _medicoRepo = medicoRepo;
    }

    public (List<Cita> citas, List<Paciente> pacientes, List<Medico> medicos) ObtenerTodasLasCitas()
    {
        return (
            _citaRepo.ObtenerTodos().ToList(),
            _pacienteRepo.ObtenerTodos().ToList(),
            _medicoRepo.ObtenerTodos().ToList());
    }

    public (List<Cita> citas, List<Paciente> pacientes, List<Medico> medicos) ObtenerCitasPorPaciente(int pacienteId)
    {
        return (
            _citaRepo.ObtenerPorPaciente(pacienteId).ToList(),
            _pacienteRepo.ObtenerTodos().ToList(),
            _medicoRepo.ObtenerTodos().ToList());
    }
}
