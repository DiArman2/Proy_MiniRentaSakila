using Microsoft.AspNetCore.Mvc;
using MiniRentaSakila.Data;
using MiniRentaSakila.Filters;
using MiniRentaSakila.Helpers;

namespace MiniRentaSakila.Controllers
{
    [Autorizar]
    public class AlquileresController : Controller
    {
        private readonly AlquilerRepository _alquilerRepository;
        private readonly PeliculaRepository _peliculaRepository;

        public AlquileresController(
            AlquilerRepository alquilerRepository,
            PeliculaRepository peliculaRepository)
        {
            _alquilerRepository = alquilerRepository;
            _peliculaRepository = peliculaRepository;
        }

        [Autorizar("Cliente")]
        public IActionResult MisAlquileres()
        {
            int? clienteId = HttpContext.Session.GetInt32(SessionKeys.CustomerId);

            if (clienteId == null)
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            var alquileres = _alquilerRepository.ObtenerPorCliente(clienteId.Value);
            return View(alquileres);
        }

        [Autorizar("Cliente")]
        [HttpPost]
        public IActionResult Alquilar(int peliculaId)
        {
            int? clienteId = HttpContext.Session.GetInt32(SessionKeys.CustomerId);

            if (clienteId == null)
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            if (_peliculaRepository.ObtenerPorId(peliculaId) == null)
            {
                return NotFound();
            }

            if (_alquilerRepository.Alquilar(peliculaId, clienteId.Value))
            {
                TempData["Mensaje"] = "Película alquilada correctamente.";
                return RedirectToAction("MisAlquileres");
            }

            TempData["Error"] = "No hay copias disponibles de esta película.";
            return RedirectToAction("Detalle", "Peliculas", new { id = peliculaId });
        }

        [Autorizar("Cliente")]
        [HttpPost]
        public IActionResult Devolver(int alquilerId)
        {
            int? clienteId = HttpContext.Session.GetInt32(SessionKeys.CustomerId);

            if (clienteId == null)
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            var alquiler = _alquilerRepository.ObtenerPorId(alquilerId);

            if (alquiler == null)
            {
                return NotFound();
            }

            if (alquiler.ClienteId != clienteId.Value)
            {
                TempData["Error"] = "Solo puedes devolver tus propios alquileres.";
                return RedirectToAction("MisAlquileres");
            }

            if (alquiler.Devuelto)
            {
                TempData["Error"] = "Este alquiler ya fue devuelto.";
                return RedirectToAction("MisAlquileres");
            }

            _alquilerRepository.Devolver(alquilerId, clienteId.Value);
            TempData["Mensaje"] = "Película devuelta correctamente.";
            return RedirectToAction("MisAlquileres");
        }

        [Autorizar("Administrador")]
        public IActionResult Todos()
        {
            var alquileres = _alquilerRepository.ObtenerRecientes(100);
            return View(alquileres);
        }
    }
}