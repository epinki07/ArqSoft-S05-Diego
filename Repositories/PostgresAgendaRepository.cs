using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Repositories
{
    public class PostgresAgendaRepository : IAgendaRepository
    {
        private readonly CitasDbContext _context;

        public PostgresAgendaRepository(CitasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Agenda> ObtenerTodos()
        {
            return _context.Agenda
                .AsNoTracking()
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Hora)
                .ToList();
        }

        public IEnumerable<Agenda> ObtenerPorPaciente(int pacienteId)
        {
            return _context.Agenda
                .AsNoTracking()
                .Where(a => a.PacienteId == pacienteId)
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Hora)
                .ToList();
        }
    }
}
