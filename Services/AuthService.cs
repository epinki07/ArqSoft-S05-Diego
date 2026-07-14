using CitasApp.Data;
using CitasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Services
{
    public class AuthService
    {
        private readonly CitasDbContext _context;

        public AuthService(CitasDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string password)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (usuario == null)
                return null;

            return PasswordHashService.Verify(password, usuario.PasswordHash)
                ? usuario
                : null;
        }
    }
}
