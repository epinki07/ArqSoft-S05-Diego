using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface IPacienteRepository
    {
        IEnumerable<Paciente> ObtenerTodos();
        Paciente ObtenerPorId(int id);
        void Agregar(Paciente paciente);
        void Editar(Paciente paciente);
        void Eliminar(int id);
    }
}
