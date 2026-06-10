using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.Data.Sqlite;

namespace CitasApp.Repositories
{
    public class SqliteMedicoRepository : IMedicoRepository
    {
        private readonly string _connectionString;

        public SqliteMedicoRepository(IWebHostEnvironment env)
            : this(Path.Combine(env.ContentRootPath, "Data", "citasapp.db"))
        {
        }

        public SqliteMedicoRepository(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
            _connectionString = $"Data Source={dbPath}";
            InicializarTabla();
        }

        public IEnumerable<Medico> ObtenerTodos()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Especialidad, NumeroLicencia FROM Medicos;";

            var lista = new List<Medico>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(LeerFila(reader));

            return lista;
        }

        public Medico ObtenerPorId(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Especialidad, NumeroLicencia FROM Medicos WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? LeerFila(reader) : new Medico();
        }

        public void Agregar(Medico medico)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Medicos (Nombre, Apellido, Especialidad, NumeroLicencia)
                VALUES ($nombre, $apellido, $especialidad, $numeroLicencia);";
            cmd.Parameters.AddWithValue("$nombre", medico.Nombre);
            cmd.Parameters.AddWithValue("$apellido", medico.Apellido);
            cmd.Parameters.AddWithValue("$especialidad", medico.Especialidad);
            cmd.Parameters.AddWithValue("$numeroLicencia", medico.NumeroLicencia);
            cmd.ExecuteNonQuery();
        }

        public void Editar(Medico medico)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Medicos
                SET Nombre = $nombre,
                    Apellido = $apellido,
                    Especialidad = $especialidad,
                    NumeroLicencia = $numeroLicencia
                WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", medico.Id);
            cmd.Parameters.AddWithValue("$nombre", medico.Nombre);
            cmd.Parameters.AddWithValue("$apellido", medico.Apellido);
            cmd.Parameters.AddWithValue("$especialidad", medico.Especialidad);
            cmd.Parameters.AddWithValue("$numeroLicencia", medico.NumeroLicencia);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Medicos WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private void InicializarTabla()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Medicos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Especialidad TEXT,
                    NumeroLicencia TEXT
                );";
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection Conectar()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private static Medico LeerFila(SqliteDataReader reader) => new()
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Apellido = reader.GetString(2),
            Especialidad = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            NumeroLicencia = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
        };
    }
}
