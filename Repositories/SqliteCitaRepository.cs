using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.Data.Sqlite;

namespace CitasApp.Repositories
{
    public class SqliteCitaRepository : ICitaRepository
    {
        private readonly string _connectionString;

        public SqliteCitaRepository(IWebHostEnvironment env)
            : this(Path.Combine(env.ContentRootPath, "Data", "citasapp.db"))
        {
        }

        public SqliteCitaRepository(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
            _connectionString = $"Data Source={dbPath}";
            InicializarTabla();
        }

        public IEnumerable<Cita> ObtenerTodos()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas;";

            var lista = new List<Cita>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(LeerFila(reader));

            return lista;
        }

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas WHERE PacienteId = $pacienteId;";
            cmd.Parameters.AddWithValue("$pacienteId", pacienteId);

            var lista = new List<Cita>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(LeerFila(reader));

            return lista;
        }

        public Cita ObtenerPorId(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado FROM Citas WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? LeerFila(reader) : new Cita();
        }

        public void Agregar(Cita cita)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Citas (PacienteId, MedicoId, Fecha, Hora, Motivo, Estado)
                VALUES ($pacienteId, $medicoId, $fecha, $hora, $motivo, $estado);";
            AsignarParametros(cmd, cita);
            cmd.ExecuteNonQuery();
        }

        public void Editar(Cita cita)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Citas
                SET PacienteId = $pacienteId,
                    MedicoId = $medicoId,
                    Fecha = $fecha,
                    Hora = $hora,
                    Motivo = $motivo,
                    Estado = $estado
                WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", cita.Id);
            AsignarParametros(cmd, cita);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Citas WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private void InicializarTabla()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Citas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PacienteId INTEGER NOT NULL,
                    MedicoId INTEGER NOT NULL,
                    Fecha TEXT NOT NULL,
                    Hora TEXT NOT NULL,
                    Motivo TEXT,
                    Estado TEXT NOT NULL DEFAULT 'Pendiente'
                );";
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection Conectar()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private static void AsignarParametros(SqliteCommand cmd, Cita cita)
        {
            cmd.Parameters.AddWithValue("$pacienteId", cita.PacienteId);
            cmd.Parameters.AddWithValue("$medicoId", cita.MedicoId);
            cmd.Parameters.AddWithValue("$fecha", cita.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$hora", cita.Hora.ToString("HH:mm"));
            cmd.Parameters.AddWithValue("$motivo", cita.Motivo);
            cmd.Parameters.AddWithValue("$estado", cita.Estado);
        }

        private static Cita LeerFila(SqliteDataReader reader) => new()
        {
            Id = reader.GetInt32(0),
            PacienteId = reader.GetInt32(1),
            MedicoId = reader.GetInt32(2),
            Fecha = DateOnly.ParseExact(reader.GetString(3), "yyyy-MM-dd"),
            Hora = TimeOnly.ParseExact(reader.GetString(4), "HH:mm"),
            Motivo = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
            Estado = reader.GetString(6)
        };
    }
}
