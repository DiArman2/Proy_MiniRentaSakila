namespace MiniRentaSakila.Models
{
    public class CopiaInventario
    {
        public int InventarioId { get; set; }
        public int PeliculaId { get; set; }
        public int TiendaId { get; set; }
        public bool Alquilada { get; set; }
    }
}