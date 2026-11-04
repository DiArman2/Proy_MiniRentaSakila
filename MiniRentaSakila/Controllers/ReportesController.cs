namespace MiniRentaSakila.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MiniRentaSakila.Data;
    using MiniRentaSakila.Filters;

    [Autorizar("Administrador")]

    public class ReportesController : Controller
    {
        private readonly ReporteRepository _reporteRepository;

        public ReportesController(ReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public IActionResult PeliculasMasAlquiladas()
        {
            var peliculas = _reporteRepository.ObtenerPeliculasMasAlquiladas(10);
            return View(peliculas);
        }

    }
}
