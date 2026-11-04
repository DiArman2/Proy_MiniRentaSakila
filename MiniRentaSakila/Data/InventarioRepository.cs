using Microsoft.Data.SqlClient;
using MiniRentaSakila.Models;

namespace MiniRentaSakila.Data
{
    public class InventarioRepository
    {
        private const int TiendaPorDefecto = 1;

        private readonly string _connectionString;

        public InventarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sakila");
        }

        public List<CopiaInventario> ObtenerCopias(int peliculaId)
        {
            const string sql = @"
                SELECT i.inventory_id, i.film_id, i.store_id,
                       CASE WHEN EXISTS (SELECT 1
                                         FROM rental r
                                         WHERE r.inventory_id = i.inventory_id
                                           AND r.return_date IS NULL)
                            THEN 1 ELSE 0 END AS alquilada
                FROM inventory i
                WHERE i.film_id = @peliculaId
                ORDER BY i.inventory_id;";

            var copias = new List<CopiaInventario>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@peliculaId", peliculaId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                copias.Add(new CopiaInventario
                {
                    InventarioId = Convert.ToInt32(reader["inventory_id"]),
                    PeliculaId = Convert.ToInt32(reader["film_id"]),
                    TiendaId = Convert.ToInt32(reader["store_id"]),
                    Alquilada = Convert.ToInt32(reader["alquilada"]) == 1
                });
            }

            return copias;
        }

        public void AgregarCopia(int peliculaId)
        {
            const string sql = "INSERT INTO inventory (film_id, store_id) VALUES (@peliculaId, @tiendaId);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@peliculaId", peliculaId);
            command.Parameters.AddWithValue("@tiendaId", TiendaPorDefecto);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public bool EliminarCopiaSinHistorial(int inventarioId)
        {
            const string sql = @"
                DELETE FROM inventory
                WHERE inventory_id = @inventarioId
                  AND NOT EXISTS (SELECT 1
                                  FROM rental r
                                  WHERE r.inventory_id = @inventarioId);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@inventarioId", inventarioId);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }
    }
}