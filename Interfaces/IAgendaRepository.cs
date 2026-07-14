using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface IAgendaRepository
    {
        IEnumerable<Agenda> ObtenerTodos();
        IEnumerable<Agenda> ObtenerPorPaciente(int pacienteId);
    }
}
