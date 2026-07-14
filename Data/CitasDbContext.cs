using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Data
{
    public class CitasDbContext : DbContext
    {
        public CitasDbContext(DbContextOptions<CitasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Agenda> Agenda => Set<Agenda>();
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Medico> Medicos => Set<Medico>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Motivo).HasMaxLength(500);
                entity.Property(c => c.Estado).HasMaxLength(50).HasDefaultValue("Pendiente");
            });

            modelBuilder.Entity<Agenda>(entity =>
            {
                entity.ToTable("Agenda");
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.CitaId).IsUnique();
                entity.Property(a => a.PacienteNombre).HasMaxLength(220).IsRequired();
                entity.Property(a => a.MedicoNombre).HasMaxLength(220).IsRequired();
                entity.Property(a => a.Motivo).HasMaxLength(500).IsRequired();
                entity.Property(a => a.Estado).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Apellido).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Email).HasMaxLength(150).IsRequired();
                entity.Property(p => p.Telefono).HasMaxLength(30).IsRequired();
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Apellido).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Especialidad).HasMaxLength(120).IsRequired();
                entity.Property(m => m.NumeroLicencia).HasMaxLength(80).IsRequired();
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
                entity.Property(u => u.NombreUsuario).HasMaxLength(80).IsRequired();
                entity.Property(u => u.NombreCompleto).HasMaxLength(160).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Rol).HasMaxLength(30).IsRequired();
            });
        }
    }
}
