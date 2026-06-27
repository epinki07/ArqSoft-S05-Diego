using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Repositories
{
    public class LoggingPacienteRepository : IPacienteRepository
    {
        private readonly IPacienteRepository _inner;

        public LoggingPacienteRepository(IPacienteRepository inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            const string operacion = "ObtenerTodos";
            Log($"{operacion} — inicio");

            try
            {
                var resultado = _inner.ObtenerTodos().ToList();
                Log($"{operacion} — {resultado.Count} registros");
                return resultado;
            }
            catch (Exception ex)
            {
                Log($"{operacion} — error: {ex.Message}");
                throw;
            }
        }

        public Paciente ObtenerPorId(int id)
        {
            var operacion = $"ObtenerPorId({id})";
            Log($"{operacion} — inicio");

            try
            {
                var resultado = _inner.ObtenerPorId(id);
                Log($"{operacion} — {(resultado.Id == 0 ? "sin resultados" : "encontrado")}");
                return resultado;
            }
            catch (Exception ex)
            {
                Log($"{operacion} — error: {ex.Message}");
                throw;
            }
        }

        public void Agregar(Paciente paciente)
        {
            Ejecutar("Agregar", () => _inner.Agregar(paciente));
        }

        public void Editar(Paciente paciente)
        {
            Ejecutar($"Editar({paciente.Id})", () => _inner.Editar(paciente));
        }

        public void Eliminar(int id)
        {
            Ejecutar($"Eliminar({id})", () => _inner.Eliminar(id));
        }

        private static void Ejecutar(string operacion, Action accion)
        {
            Log($"{operacion} — inicio");

            try
            {
                accion();
                Log($"{operacion} — completado");
            }
            catch (Exception ex)
            {
                Log($"{operacion} — error: {ex.Message}");
                throw;
            }
        }

        private static void Log(string mensaje)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}");
        }
    }
}
