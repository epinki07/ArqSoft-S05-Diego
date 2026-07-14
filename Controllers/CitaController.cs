using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitasApp.Controllers
{
    [Authorize]
    public class CitaController : Controller
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IPacienteRepository _pacienteRepo;
        private readonly IMedicoRepository _medicoRepo;

        public CitaController(ICitaRepository citaRepo,
                              IPacienteRepository pacienteRepo,
                              IMedicoRepository medicoRepo)
        {
            _citaRepo = citaRepo;
            _pacienteRepo = pacienteRepo;
            _medicoRepo = medicoRepo;
        }

        public IActionResult Index()
        {
            var citas = _citaRepo.ObtenerTodos();
            if (User.IsInRole("Medico"))
            {
                var medicoId = ObtenerClaimEntero("MedicoId");
                citas = citas.Where(c => c.MedicoId == medicoId);
            }
            else if (User.IsInRole("Paciente"))
            {
                var pacienteId = ObtenerClaimEntero("PacienteId");
                citas = citas.Where(c => c.PacienteId == pacienteId);
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            ViewBag.Rol = User.FindFirstValue(ClaimTypes.Role);
            ViewBag.PuedeAdministrar = User.IsInRole("Administrador");
            return View(citas);
        }

        public IActionResult PorPaciente(int pacienteId)
        {
            if (User.IsInRole("Paciente") && ObtenerClaimEntero("PacienteId") != pacienteId)
                return Forbid();

            var citas = _citaRepo.ObtenerPorPaciente(pacienteId);
            if (User.IsInRole("Medico"))
            {
                var medicoId = ObtenerClaimEntero("MedicoId");
                citas = citas.Where(c => c.MedicoId == medicoId);
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(citas);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Cita cita)
        {
            if (ModelState.IsValid)
            {
                _citaRepo.Agregar(cita);
                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            var cita = _citaRepo.ObtenerPorId(id);
            if (cita.Id == 0)
                return NotFound();

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(Cita cita)
        {
            if (ModelState.IsValid)
            {
                _citaRepo.Editar(cita);
                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            _citaRepo.Eliminar(id);
            return RedirectToAction("Index");
        }

        private int ObtenerClaimEntero(string claimType)
        {
            var valor = User.FindFirstValue(claimType);
            return int.TryParse(valor, out var id) ? id : 0;
        }
    }
}
