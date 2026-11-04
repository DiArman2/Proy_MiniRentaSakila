namespace MiniRentaSakila.Models
{
    public class InventarioPeliculaViewModel
    {
        public Pelicula Pelicula { get; set; }
        public List<CopiaInventario> Copias { get; set; }
    }
}