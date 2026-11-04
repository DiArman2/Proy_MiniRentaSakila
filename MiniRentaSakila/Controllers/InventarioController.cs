using Microsoft.AspNetCore.Mvc;
using MiniRentaSakila.Data;
using MiniRentaSakila.Filters;
using MiniRentaSakila.Models;

namespace MiniRentaSakila.Controllers
{
    [Autorizar("Administrador")]
    public class InventarioController : Controller
    {
        private readonly InventarioRepository _inventarioRepository;
        private readonly PeliculaRepository _peliculaRepository;

        public InventarioController(
            InventarioRepository inventarioRepository,
            PeliculaRepository peliculaRepository)
        {
            _inventarioRepository = inventarioRepository;
            _peliculaRepository = peliculaRepository;
        }

        public IActionResult Index(int peliculaId)
        {
            var pelicula = _peliculaRepository.ObtenerPorId(peliculaId);

            if (pelicula == null)
            {
                return NotFound();
            }

            var modelo = new InventarioPeliculaViewModel
            {
                Pelicula = pelicula,
                Copias = _inventarioRepository.ObtenerCopias(peliculaId)
            };

            return View(modelo);
        }

        [HttpPost]
        public IActionResult Agregar(int peliculaId)
        {
            if (_peliculaRepository.ObtenerPorId(peliculaId) == null)
            {
                return NotFound();
            }

            _inventarioRepository.AgregarCopia(peliculaId);
            TempData["Mensaje"] = "Copia agregada correctamente.";
            return RedirectToAction("Index", new { peliculaId });
        }

        [HttpPost]
        public IActionResult Eliminar(int inventarioId, int peliculaId)
        {
            bool eliminada = _inventarioRepository.EliminarCopiaSinHistorial(inventarioId);

            if (eliminada)
            {
                TempData["Mensaje"] = "Copia eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar una copia que tiene historial de alquileres.";
            }

            return RedirectToAction("Index", new { peliculaId });
        }
    }
}