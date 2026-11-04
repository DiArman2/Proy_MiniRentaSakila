using Microsoft.Data.SqlClient;
using MiniRentaSakila.Models;

namespace MiniRentaSakila.Data
{
    public class AlquilerRepository
    {
        private const int EmpleadoPorDefecto = 1;

        private const string SelectBase = @"
            SELECT r.rental_id, r.customer_id,
                   c.first_name + ' ' + c.last_name AS cliente,
                   f.title, r.rental_date, r.return_date
            FROM rental r
            INNER JOIN inventory i ON i.inventory_id = r.inventory_id
            INNER JOIN film f ON f.film_id = i.film_id
            INNER JOIN customer c ON c.customer_id = r.customer_id";

        private readonly string _connectionString;

        public AlquilerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sakila");
        }

        public bool Alquilar(int peliculaId, int clienteId)
        {
            const string sql = @"
                INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
                SELECT TOP 1 GETDATE(), i.inventory_id, @clienteId, @empleadoId
                FROM inventory i
                INNER JOIN film f ON f.film_id = i.film_id
                WHERE i.film_id = @peliculaId
                  AND f.activo = 1
                  AND NOT EXISTS (SELECT 1
                                  FROM rental r
                                  WHERE r.inventory_id = i.inventory_id
                                    AND r.return_date IS NULL)
                ORDER BY i.inventory_id;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@clienteId", clienteId);
            command.Parameters.AddWithValue("@empleadoId", EmpleadoPorDefecto);
            command.Parameters.AddWithValue("@peliculaId", peliculaId);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public Alquiler ObtenerPorId(int alquilerId)
        {
            string sql = SelectBase + " WHERE r.rental_id = @alquilerId;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@alquilerId", alquilerId);

            connection.Open();
            using var reader = command.ExecuteReader();

            return reader.Read() ? MapearAlquiler(reader) : null;
        }

        public List<Alquiler> ObtenerPorCliente(int clienteId)
        {
            string sql = SelectBase + @"
                WHERE r.customer_id = @clienteId
                ORDER BY CASE WHEN r.return_date IS NULL THEN 0 ELSE 1 END, r.rental_date DESC;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@clienteId", clienteId);

            return LeerAlquileres(connection, command);
        }

        public List<Alquiler> ObtenerRecientes(int cantidad)
        {
            string sql = SelectBase.Replace("SELECT ", "SELECT TOP (@cantidad) ")
                         + " ORDER BY r.rental_date DESC;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cantidad", cantidad);

            return LeerAlquileres(connection, command);
        }

        public bool Devolver(int alquilerId, int clienteId)
        {
            const string sql = @"
                UPDATE rental
                SET return_date = GETDATE()
                WHERE rental_id = @alquilerId
                  AND customer_id = @clienteId
                  AND return_date IS NULL;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@alquilerId", alquilerId);
            command.Parameters.AddWithValue("@clienteId", clienteId);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        private static List<Alquiler> LeerAlquileres(SqlConnection connection, SqlCommand command)
        {
            var alquileres = new List<Alquiler>();

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                alquileres.Add(MapearAlquiler(reader));
            }

            return alquileres;
        }

        private static Alquiler MapearAlquiler(SqlDataReader reader)
        {
            return new Alquiler
            {
                AlquilerId = Convert.ToInt32(reader["rental_id"]),
                ClienteId = Convert.ToInt32(reader["customer_id"]),
                NombreCliente = reader["cliente"].ToString(),
                TituloPelicula = reader["title"].ToString(),
                FechaAlquiler = Convert.ToDateTime(reader["rental_date"]),
                FechaDevolucion = reader["return_date"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["return_date"])
            };
        }
    }
}