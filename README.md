\# Mini sistema de renta de películas con Sakila



Aplicación web en ASP.NET Core MVC con SQL Server (ADO.NET) sobre la base de datos Sakila.



\## Funcionalidades

\- Inicio y cierre de sesión con contraseñas protegidas (PBKDF2).

\- Roles Administrador y Cliente, con autorización mediante un filtro propio y Session.

\- Catálogo de películas con disponibilidad de copias.

\- Alquiler y devolución de películas.

\- CRUD de películas, gestión de copias del inventario y eliminación lógica.

\- Reporte de las 10 películas más alquiladas.



\## Cómo ejecutarlo

1\. Restaura la base de datos Sakila en SQL Server.

2\. Ejecuta `Database/01\_cambios\_sakila.sql` sobre esa base.

3\. Ajusta la cadena de conexión en `appsettings.json`.

4\. Ejecuta el proyecto desde Visual Studio o con `dotnet run`.



\## Usuarios de prueba

| Usuario | Contraseña | Rol |

|---|---|---|

| admin | admin123 | Administrador |

| mary | cliente123 | Cliente |

