using System.Security.Claims;
using CitasApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombreUsuario, string password, string? returnUrl = null)
        {
            var usuario = await _authService.AutenticarAsync(nombreUsuario, password);
            if (usuario == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, string.IsNullOrWhiteSpace(usuario.NombreCompleto)
                    ? usuario.NombreUsuario
                    : usuario.NombreCompleto),
                new(ClaimTypes.Role, usuario.Rol)
            };

            if (usuario.PacienteId.HasValue)
                claims.Add(new Claim("PacienteId", usuario.PacienteId.Value.ToString()));

            if (usuario.MedicoId.HasValue)
                claims.Add(new Claim("MedicoId", usuario.MedicoId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Cita");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
