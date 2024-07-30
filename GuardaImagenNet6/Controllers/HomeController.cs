using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics;

namespace GuardaImagenNet6.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PruebasDBContext context;
    private readonly IWebHostEnvironment env;

    public HomeController(IWebHostEnvironment _env, ILogger<HomeController> logger, PruebasDBContext _context)
    {
        _logger = logger;
        env = _env;
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
    //  revisar
    public IActionResult Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioVM usuario)
    {
        if (usuario == null)
            return BadRequest("Error usuario no valido");
        if (usuario.FotoByte == null || usuario.FotoByte.Length == 0)
            return BadRequest("Imagen no seleccionada");

        string photoName = Path.GetFileName(usuario.FotoByte.FileName);
        string extPhoto = Path.GetExtension(usuario.FotoByte.FileName);

        ImagenesUtility imagenTools = new ImagenesUtility();

        if (!imagenTools.ExtensionsFotosValid(extPhoto))
            return BadRequest("El archivo no es una imagen valida");

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


    public async Task<IActionResult> Detalles(int id)
    {
        if (id <= 0)
            return View("Index");

        Usuario userDB = await context.Usuarios.FindAsync(id);

        if (userDB == null)
            return View("Index");

        string imagenSrc = string.Empty;
        if (userDB.FotoBd != null)
        {
            string imgBase64 = Convert.ToBase64String(userDB.FotoBd);
            imagenSrc = string.Format("data:imagen/*;base64,{0}", imgBase64);
        }
        else
        {
            var foldername = "image";
            var filename = "user-azul.jpg";
            var path1 = Path.Combine(env.WebRootPath, foldername, filename);
            var path2 = Path.Combine("\\", foldername, filename);
            Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");
            imagenSrc = location.AbsoluteUri;
        }


        UsuarioVM userFound = new UsuarioVM
        {
            ID = userDB.Id,
            NombreUsuario = userDB.UserName,
            Contrasenya = userDB.Password,
            FechaAlta = userDB.FechaAlta,
            FotoByteSrc = imagenSrc,
            Activo = userDB.Estatus ?? false
        };
        return View(userFound);
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
