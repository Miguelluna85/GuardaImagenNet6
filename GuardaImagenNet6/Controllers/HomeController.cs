using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System;
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

    public async Task<IActionResult> Index()
    {
        IEnumerable<Usuario> listUserBD = await context.Usuarios.ToListAsync();
        List<UsuarioVM> listUserVM = new List<UsuarioVM>();

        foreach (Usuario userDB in listUserBD)
        {
            UsuarioVM usrVM = new UsuarioVM
            {
                ID = userDB.Id,
                NombreUsuario = userDB.UserName,
                Contrasenya = userDB.Password,
                FotoSrc = ImageBdToURL(userDB.FotoBd),
                Activo = userDB.Estatus ?? false,
                FechaAlta = userDB.FechaAlta
            };
            listUserVM.Add(usrVM);
        }

        return View(listUserVM);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //  revisar
    public async Task<IActionResult> Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioVM usuario)
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

        Usuario user = new Usuario();
        using (var streamPhoto = new MemoryStream())
        {
            await usuario.FotoByte.CopyToAsync(streamPhoto);
            user.FotoBd = streamPhoto.ToArray();
        }

        user.UserName = usuario.NombreUsuario;
        user.Password = usuario.Contrasenya;
        user.Estatus = usuario.Activo;
        context.Usuarios.Add(user);

        await context.SaveChangesAsync();

        return RedirectToAction("Index");
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

        UsuarioVM userFound = new UsuarioVM
        {
            ID = userDB.Id,
            NombreUsuario = userDB.UserName,
            Contrasenya = userDB.Password,
            FechaAlta = userDB.FechaAlta,
            FotoSrc = ImageBdToURL(userDB.FotoBd),
            Activo = userDB.Estatus ?? false
        };

        return View(userFound);
    }
    private string ImageBdToURL(byte[] FotoBD)
    {
        if (FotoBD != null)
        {
            string imgBase64 = Convert.ToBase64String(FotoBD);
            return string.Format("data:imagen/*;base64,{0}", imgBase64);
        }
        else
        {
            var foldername = @"image\Usuario";
            var filename = "userDefault.png";
            var path1 = Path.Combine(env.WebRootPath, foldername, filename);
            var path2 = Path.Combine("\\", foldername, filename);//ruta relativa img
            Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");//absoluta
            return location.AbsoluteUri;
        }
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
