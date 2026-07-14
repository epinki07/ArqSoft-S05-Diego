using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Repositories
{
    public class PostgresMedicoRepository : IMedicoRepository
    {
        private readonly CitasDbContext _context;

        public PostgresMedicoRepository(CitasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Medico> ObtenerTodos()
        {
            return _context.Medicos
                .AsNoTracking()
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToList();
        }

        public Medico ObtenerPorId(int id)
        {
            return _context.Medicos
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id) ?? new Medico();
        }

        public void Agregar(Medico medico)
        {
            _context.Medicos.Add(medico);
            _context.SaveChanges();
        }

        public void Editar(Medico medico)
        {
            _context.Medicos.Update(medico);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var medico = _context.Medicos.Find(id);
            if (medico == null)
                return;

            _context.Medicos.Remove(medico);
            _context.SaveChanges();
        }
    }
}
