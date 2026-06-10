using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.Data.Sqlite;

namespace CitasApp.Repositories
{
    public class SqlitePacienteRepository : IPacienteRepository
    {
        private readonly string _connectionString;

        public SqlitePacienteRepository(IWebHostEnvironment env)
            : this(Path.Combine(env.ContentRootPath, "Data", "citasapp.db"))
        {
        }

        public SqlitePacienteRepository(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
            _connectionString = $"Data Source={dbPath}";
            InicializarTabla();
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Email, Telefono FROM Pacientes;";

            var lista = new List<Paciente>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(LeerFila(reader));

            return lista;
        }

        public Paciente ObtenerPorId(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nombre, Apellido, Email, Telefono FROM Pacientes WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? LeerFila(reader) : new Paciente();
        }

        public void Agregar(Paciente paciente)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Pacientes (Nombre, Apellido, Email, Telefono)
                VALUES ($nombre, $apellido, $email, $telefono);";
            cmd.Parameters.AddWithValue("$nombre", paciente.Nombre);
            cmd.Parameters.AddWithValue("$apellido", paciente.Apellido);
            cmd.Parameters.AddWithValue("$email", paciente.Email);
            cmd.Parameters.AddWithValue("$telefono", paciente.Telefono);
            cmd.ExecuteNonQuery();
        }

        public void Editar(Paciente paciente)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Pacientes
                SET Nombre = $nombre, Apellido = $apellido, Email = $email, Telefono = $telefono
                WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", paciente.Id);
            cmd.Parameters.AddWithValue("$nombre", paciente.Nombre);
            cmd.Parameters.AddWithValue("$apellido", paciente.Apellido);
            cmd.Parameters.AddWithValue("$email", paciente.Email);
            cmd.Parameters.AddWithValue("$telefono", paciente.Telefono);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Pacientes WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private void InicializarTabla()
        {
            using var conn = Conectar();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Pacientes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    Email TEXT,
                    Telefono TEXT
                );";
            cmd.ExecuteNonQuery();
        }

        private SqliteConnection Conectar()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private static Paciente LeerFila(SqliteDataReader reader) => new()
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Apellido = reader.GetString(2),
            Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            Telefono = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
        };
    }
}
