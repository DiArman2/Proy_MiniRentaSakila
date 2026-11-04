namespace MiniRentaSakila.Data
{
    using Microsoft.Data.SqlClient;
    using MiniRentaSakila.Models;

    public class ReporteRepository
    {
        private readonly string _connectionString;

        public ReporteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sakila");
        }

        public List<PeliculaAlquilerViewModel> ObtenerPeliculasMasAlquiladas(int cantidad)
        {
            var resultado = new List<PeliculaAlquilerViewModel>();

            const string sql = @"
        SELECT TOP (@cantidad)
            f.title,
            COUNT(r.rental_id) AS total_alquileres
        FROM film f
        INNER JOIN inventory i ON i.film_id = f.film_id
        INNER JOIN rental r ON r.inventory_id = i.inventory_id
        GROUP BY f.film_id, f.title
        ORDER BY total_alquileres DESC;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cantidad", cantidad);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                resultado.Add(new PeliculaAlquilerViewModel
                {
                    Titulo = reader.GetString(0),
                    CantidadAlquileres = reader.GetInt32(1)
                });
            }

            return resultado;
        }

    }
}
