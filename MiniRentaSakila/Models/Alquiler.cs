namespace MiniRentaSakila.Models
{
    public class Alquiler
    {
        public int AlquilerId { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public string TituloPelicula { get; set; }
        public DateTime FechaAlquiler { get; set; }
        public DateTime? FechaDevolucion { get; set; }

        public bool Devuelto => FechaDevolucion.HasValue;
    }
}