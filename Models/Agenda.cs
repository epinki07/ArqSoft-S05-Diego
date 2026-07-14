namespace CitasApp.Models
{
    public class Agenda
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string MedicoNombre { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente";
    }
}
