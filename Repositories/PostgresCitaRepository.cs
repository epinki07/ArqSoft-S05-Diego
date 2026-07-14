using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Repositories
{
    public class PostgresCitaRepository : ICitaRepository
    {
        private readonly CitasDbContext _context;

        public PostgresCitaRepository(CitasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Cita> ObtenerTodos()
        {
            return _context.Citas
                .AsNoTracking()
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToList();
        }

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId)
        {
            return _context.Citas
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Hora)
                .ToList();
        }

        public Cita ObtenerPorId(int id)
        {
            return _context.Citas
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id) ?? new Cita();
        }

        public void Agregar(Cita cita)
        {
            _context.Citas.Add(cita);
            _context.SaveChanges();
            SincronizarAgenda(cita);
        }

        public void Editar(Cita cita)
        {
            _context.Citas.Update(cita);
            _context.SaveChanges();
            SincronizarAgenda(cita);
        }

        public void Eliminar(int id)
        {
            var cita = _context.Citas.Find(id);
            if (cita == null)
                return;

            var agenda = _context.Agenda.FirstOrDefault(a => a.CitaId == id);
            if (agenda != null)
                _context.Agenda.Remove(agenda);

            _context.Citas.Remove(cita);
            _context.SaveChanges();
        }

        private void SincronizarAgenda(Cita cita)
        {
            var paciente = _context.Pacientes
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == cita.PacienteId);
            var medico = _context.Medicos
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == cita.MedicoId);

            var agenda = _context.Agenda.FirstOrDefault(a => a.CitaId == cita.Id);
            if (agenda == null)
            {
                agenda = new Agenda { CitaId = cita.Id };
                _context.Agenda.Add(agenda);
            }

            agenda.PacienteId = cita.PacienteId;
            agenda.MedicoId = cita.MedicoId;
            agenda.PacienteNombre = paciente == null
                ? string.Empty
                : $"{paciente.Nombre} {paciente.Apellido}";
            agenda.MedicoNombre = medico == null
                ? string.Empty
                : $"{medico.Nombre} {medico.Apellido}";
            agenda.Fecha = cita.Fecha;
            agenda.Hora = cita.Hora;
            agenda.Motivo = cita.Motivo;
            agenda.Estado = cita.Estado;

            _context.SaveChanges();
        }
    }
}
