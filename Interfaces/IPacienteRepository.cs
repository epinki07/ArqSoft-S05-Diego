using ArqSoft_S05_Diego.Models;

namespace ArqSoft_S05_Diego.Interfaces
{
    public interface IPacienteRepository
    {
        IEnumerable<Paciente> ObtenerTodos();
        Paciente? ObtenerPorId(int id);
    }
}
