using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GuardaImagenNet6.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PruebasDBContext context;

    public HomeController(ILogger<HomeController> logger, PruebasDBContext _context)
    {
        _logger = logger;
        context = _context;
    }

    public IActionResult Index()
    {
        var list = context.Usuarios.ToList();
        return View(list);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioVM usuario)
    {
        if (usuario == null)
            return BadRequest("Error usuario no valido");
        if (usuario.FotoByte == null || usuario.FotoByte.Length == 0)
            return BadRequest("Imagen no seleccionada");

        string photoName = Path.GetFileName(usuario.FotoByte.FileName);
        string contentType = usuario.FotoByte.ContentType;

        using (var streamPhoto = new MemoryStream())
        {
            usuario.FotoByte.CopyToAsync(streamPhoto);
            Usuario user = new Usuario();
            user.UserName = usuario.NombreUsuario;
            user.Password = usuario.Contrasenya;
            user.FotoBd = streamPhoto.ToArray();
            user.Estatus = usuario.Activo;

            context.Usuarios.Add(user);
            context.SaveChanges();
        }
        return View();
    }

    public IActionResult Actualizar()
    {
        return View();
    }

    [HttpPut]
    public IActionResult Actualizar(UsuarioVM usuario)
    {
        if (usuario == null)
            return BadRequest("Error usuario no valido");
        if (usuario.FotoByte == null || usuario.FotoByte.Length == 0)
            return BadRequest("Imagen no seleccionada");

        string photoName = Path.GetFileName(usuario.FotoByte.FileName);
        string contentType = usuario.FotoByte.ContentType;

        using (var streamPhoto = new MemoryStream())
        {
            usuario.FotoByte.CopyToAsync(streamPhoto);
            Usuario user = new Usuario();
            user.UserName = usuario.NombreUsuario;
            user.Password = usuario.Contrasenya;
            user.FotoBd = streamPhoto.ToArray();
            user.Estatus = usuario.Activo;

            context.Usuarios.Add(user);
            context.SaveChanges();
        }
        return View();
    }


    public IActionResult Detalles()
    {
        return View();
    }

    public IActionResult Eliminar()
    {
        return View();
    }


    public IActionResult Contacto()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
