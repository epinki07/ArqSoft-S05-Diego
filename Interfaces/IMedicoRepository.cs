using ArqSoft_S05_Diego.Models;

namespace ArqSoft_S05_Diego.Interfaces
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
