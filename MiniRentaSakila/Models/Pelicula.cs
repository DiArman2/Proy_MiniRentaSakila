using System.ComponentModel.DataAnnotations;

namespace MiniRentaSakila.Models
{
    public class Pelicula
    {
        public int PeliculaId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(128, ErrorMessage = "El título no puede superar 128 caracteres.")]
        public string Titulo { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100.")]
        public int? AnioLanzamiento { get; set; }

        [Range(0.01, 99.99, ErrorMessage = "La tarifa debe estar entre 0.01 y 99.99.")]
        public decimal TarifaAlquiler { get; set; }

        [Range(1, 1000, ErrorMessage = "La duración debe estar entre 1 y 1000 minutos.")]
        public int? Duracion { get; set; }

        [Required(ErrorMessage = "La clasificación es obligatoria.")]
        [RegularExpression("^(G|PG|PG-13|R|NC-17)$", ErrorMessage = "Clasificación no válida.")]
        public string Clasificacion { get; set; }

        public int CopiasTotales { get; set; }
        public int CopiasDisponibles { get; set; }
    }
}