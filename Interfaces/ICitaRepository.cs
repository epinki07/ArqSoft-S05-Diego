using ArqSoft_S05_Diego.Models;

namespace ArqSoft_S05_Diego.Interfaces
{
    public interface ICitaRepository
    {
        IEnumerable<Cita> ObtenerTodos();
        IEnumerable<Cita> ObtenerPorPaciente(int pacienteId);
        Cita ObtenerPorId(int id);
        void Agregar(Cita cita);
        void Editar(Cita cita);
        void Eliminar(int id);
    }
}
