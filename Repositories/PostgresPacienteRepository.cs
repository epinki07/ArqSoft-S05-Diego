using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Repositories
{
    public class PostgresPacienteRepository : IPacienteRepository
    {
        private readonly CitasDbContext _context;

        public PostgresPacienteRepository(CitasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            return _context.Pacientes
                .AsNoTracking()
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToList();
        }

        public Paciente ObtenerPorId(int id)
        {
            return _context.Pacientes
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id) ?? new Paciente();
        }

        public void Agregar(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            _context.SaveChanges();
        }

        public void Editar(Paciente paciente)
        {
            _context.Pacientes.Update(paciente);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var paciente = _context.Pacientes.Find(id);
            if (paciente == null)
                return;

            _context.Pacientes.Remove(paciente);
            _context.SaveChanges();
        }
    }
}
