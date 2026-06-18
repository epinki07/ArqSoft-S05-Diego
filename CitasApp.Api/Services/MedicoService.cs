using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services;

public class MedicoService
{
    private readonly IMedicoRepository _medicoRepo;

    public MedicoService(IMedicoRepository medicoRepo)
    {
        _medicoRepo = medicoRepo;
    }

    public IEnumerable<Medico> ObtenerTodos() => _medicoRepo.ObtenerTodos();

    public Medico? ObtenerPorId(int id)
    {
        var medico = _medicoRepo.ObtenerPorId(id);
        return medico.Id == 0 ? null : medico;
    }
}
