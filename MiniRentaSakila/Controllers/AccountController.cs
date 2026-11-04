using Microsoft.AspNetCore.Mvc;
using MiniRentaSakila.Helpers;
using MiniRentaSakila.Models;

public class AccountController : Controller
{
    private readonly UsuarioRepository _usuarioRepository;

    public AccountController(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Usuario usuario = _usuarioRepository.ObtenerPorNombre(model.NombreUsuario);

        if (usuario == null || !PasswordHasher.Verify(model.Password, usuario.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeys.UsuarioId, usuario.UsuarioId);
        HttpContext.Session.SetString(SessionKeys.NombreUsuario, usuario.NombreUsuario);
        HttpContext.Session.SetString(SessionKeys.Rol, usuario.Rol);

        if (usuario.CustomerId.HasValue)
        {
            HttpContext.Session.SetInt32(SessionKeys.CustomerId, usuario.CustomerId.Value);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }
}