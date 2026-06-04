using ArqSoft_S05_Diego.Interfaces;
using ArqSoft_S05_Diego.Models;
using System.Text.Json;

namespace ArqSoft_S05_Diego.Repositories
{
    public class JsonMedicoRepository : IMedicoRepository
    {
        private readonly string _filePath;

        public JsonMedicoRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "Medicos.json");
        }

        public IEnumerable<Medico> ObtenerTodos()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Enumerable.Empty<Medico>();
                }

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Medico>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Medico>();
            }
            catch
            {
                return Enumerable.Empty<Medico>();
            }
        }

        public Medico ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(m => m.Id == id) ?? new Medico();
        }
    }
}
