using Microsoft.Data.SqlClient;
using MiniRentaSakila.Models;

public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Sakila");
    }

    public Usuario ObtenerPorNombre(string nombreUsuario)
    {
        const string sql = @"
        SELECT usuario_id, nombre_usuario, password_hash, rol, customer_id, staff_id
        FROM Usuarios
        WHERE nombre_usuario = @nombreUsuario AND activo = 1;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);

        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Usuario
        {
            UsuarioId = reader.GetInt32(0),
            NombreUsuario = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Rol = reader.GetString(3),
            CustomerId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
            StaffId = reader.IsDBNull(5) ? null : reader.GetByte(5)
        };
    }

}