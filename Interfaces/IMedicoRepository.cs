using ArqSoft_S05_Diego.Models;

namespace ArqSoft_S05_Diego.Interfaces
{
    public interface IMedicoRepository
    {
        IEnumerable<Medico> ObtenerTodos();
        Medico? ObtenerPorId(int id);
    }
}
