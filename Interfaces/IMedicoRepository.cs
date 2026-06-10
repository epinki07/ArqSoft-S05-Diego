using CitasApp.Models;

namespace CitasApp.Interfaces
{
    public interface IMedicoRepository
    {
        IEnumerable<Medico> ObtenerTodos();
        Medico ObtenerPorId(int id);
        void Agregar(Medico medico);
        void Editar(Medico medico);
        void Eliminar(int id);
    }
}
