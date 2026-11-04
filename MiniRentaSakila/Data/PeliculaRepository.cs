using Microsoft.Data.SqlClient;
using MiniRentaSakila.Models;

namespace MiniRentaSakila.Data
{
    public class PeliculaRepository
    {
        private readonly string _connectionString;

        private const string SelectBase = @"
            SELECT f.film_id, f.title, f.description, f.release_year,
                   f.rental_rate, f.length, f.rating,
                   (SELECT COUNT(*)
                    FROM inventory i
                    WHERE i.film_id = f.film_id) AS copias_totales,
                   (SELECT COUNT(*)
                    FROM inventory i
                    WHERE i.film_id = f.film_id
                      AND NOT EXISTS (SELECT 1
                                      FROM rental r
                                      WHERE r.inventory_id = i.inventory_id
                                        AND r.return_date IS NULL)) AS copias_disponibles
            FROM film f
            WHERE f.activo = 1";

        public PeliculaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sakila");
        }

        public List<Pelicula> ObtenerCatalogo(string titulo)
        {
            string sql = SelectBase + " AND f.title LIKE @titulo ORDER BY f.title;";

            var peliculas = new List<Pelicula>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@titulo", "%" + titulo + "%");

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                peliculas.Add(MapearPelicula(reader));
            }

            return peliculas;
        }

        public Pelicula ObtenerPorId(int peliculaId)
        {
            string sql = SelectBase + " AND f.film_id = @peliculaId;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@peliculaId", peliculaId);

            connection.Open();
            using var reader = command.ExecuteReader();

            return reader.Read() ? MapearPelicula(reader) : null;
        }

        public void Crear(Pelicula pelicula)
        {
            const string sql = @"
        INSERT INTO film (title, description, release_year, language_id, rental_duration,
                          rental_rate, length, replacement_cost, rating, activo)
        VALUES (@titulo, @descripcion, @anio, 1, 3,
                @tarifa, @duracion, 19.99, @clasificacion, 1);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            AgregarParametros(command, pelicula);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Actualizar(Pelicula pelicula)
        {
            const string sql = @"
        UPDATE film
        SET title = @titulo,
            description = @descripcion,
            release_year = @anio,
            rental_rate = @tarifa,
            length = @duracion,
            rating = @clasificacion
        WHERE film_id = @peliculaId AND activo = 1;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            AgregarParametros(command, pelicula);
            command.Parameters.AddWithValue("@peliculaId", pelicula.PeliculaId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Desactivar(int peliculaId)
        {
            const string sql = "UPDATE film SET activo = 0 WHERE film_id = @peliculaId;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@peliculaId", peliculaId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static void AgregarParametros(SqlCommand command, Pelicula pelicula)
        {
            command.Parameters.AddWithValue("@titulo", pelicula.Titulo);
            command.Parameters.AddWithValue("@descripcion", (object?)pelicula.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@anio", (object?)pelicula.AnioLanzamiento ?? DBNull.Value);
            command.Parameters.AddWithValue("@tarifa", pelicula.TarifaAlquiler);
            command.Parameters.AddWithValue("@duracion", (object?)pelicula.Duracion ?? DBNull.Value);
            command.Parameters.AddWithValue("@clasificacion", pelicula.Clasificacion);
        }

        private static Pelicula MapearPelicula(SqlDataReader reader)
        {
            return new Pelicula
            {
                PeliculaId = Convert.ToInt32(reader["film_id"]),
                Titulo = reader["title"].ToString(),
                Descripcion = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
                AnioLanzamiento = reader["release_year"] == DBNull.Value ? null : Convert.ToInt32(reader["release_year"]),
                TarifaAlquiler = Convert.ToDecimal(reader["rental_rate"]),
                Duracion = reader["length"] == DBNull.Value ? null : Convert.ToInt32(reader["length"]),
                Clasificacion = reader["rating"] == DBNull.Value ? null : reader["rating"].ToString(),
                CopiasTotales = Convert.ToInt32(reader["copias_totales"]),
                CopiasDisponibles = Convert.ToInt32(reader["copias_disponibles"])
            };
        }
    }
}