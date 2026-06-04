using ArqSoft_S05_Diego.Models;

namespace ArqSoft_S05_Diego.Interfaces
{
    public interface ICitaRepository
    {
        IEnumerable<Cita> ObtenerTodos();
        IEnumerable<Cita> ObtenerPorPaciente(int pacienteId);
    }
}
