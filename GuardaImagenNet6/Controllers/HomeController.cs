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
using System.Net.Mime;

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
        IEnumerable<Usuario> listUserBD = await context.Usuarios.AsNoTracking().ToListAsync();
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

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Actualizar(int id)
    {
        if (id == 0) return BadRequest("Usuario no Proporcionado");

        var userFound = await usuarioVMSearchFirstOr(id);

        if (userFound == null)
            return BadRequest("Usuario No Encontrado.");

        return View(userFound);


        //if (usuario == null)
        //    return BadRequest("Error usuario no valido");
        //if (usuario.FotoByte == null || usuario.FotoByte.Length == 0)
        //    return BadRequest("Imagen no seleccionada");

        //string photoName = Path.GetFileName(usuario.FotoByte.FileName);
        //string contentType = usuario.FotoByte.ContentType;

        //using (var streamPhoto = new MemoryStream())
        //{
        //    usuario.FotoByte.CopyToAsync(streamPhoto);
        //    Usuario user = new Usuario();
        //    user.UserName = usuario.NombreUsuario;
        //    user.Password = usuario.Contrasenya;
        //    user.FotoBd = streamPhoto.ToArray();
        //    user.Estatus = usuario.Activo;

        //    context.Usuarios.Update(user);
        //    context.SaveChanges();
        //}

    }

    [HttpPost, ActionName("Actualizar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioVM userVM)
    {
        if (id == null || userVM == null)
            return NotFound();

        if (id <= 0)
            return BadRequest("Usuario no encontrado");

        //var usrBD1 = await context.Usuarios.AsNoTracking().FindAsync(id);
        var userToUpdate = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

        if (userVM.FotoByte != null)
        {
            using (var streamPhoto = new MemoryStream())
            {
                await userVM.FotoByte.CopyToAsync(streamPhoto);
                userToUpdate.FotoBd = streamPhoto.ToArray();
            }
        }

        userToUpdate.Id = int.Parse(id.ToString());
        userToUpdate.Password = string.IsNullOrEmpty(userVM.Contrasenya) ? userToUpdate.Password : userVM.Contrasenya;
        userToUpdate.Estatus = userVM.Activo;
        userToUpdate.FechaModifico = DateTime.Now;

        if (await TryUpdateModelAsync<Usuario>(
            userToUpdate,
            "",
            u => u.Password, u => u.Estatus, u => u.FechaModifico
            ))
        {
            await context.SaveChangesAsync();

        }

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detalles(int id)
    {
        var userFound = await usuarioVMSearchFirstOr(id);
        if (userFound == null)
            return BadRequest("Usuario No Encontrado.");

        return View(userFound);
    }
    private async Task<UsuarioVM> usuarioVMSearchFind(int id)
    {
        if (id <= 0) return null;

        Usuario userDB = await context.Usuarios.FindAsync(id);
        if (userDB == null) return null;

        UsuarioVM userFound = new UsuarioVM
        {
            ID = userDB.Id,
            NombreUsuario = userDB.UserName,
            Contrasenya = userDB.Password,
            FechaAlta = userDB.FechaAlta,
            FotoSrc = ImageBdToURL(userDB.FotoBd),
            Activo = userDB.Estatus ?? false
        };
        return userFound;
    }
    private async Task<UsuarioVM> usuarioVMSearchFirstOr(int id)
    {
        if (id <= 0) return null;

        Usuario userDB = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (userDB == null) return null;

        UsuarioVM userFound = new UsuarioVM
        {
            ID = userDB.Id,
            NombreUsuario = userDB.UserName,
            Contrasenya = userDB.Password,
            FechaAlta = userDB.FechaAlta,
            FotoSrc = ImageBdToURL(userDB.FotoBd),
            Activo = userDB.Estatus ?? false
        };
        return userFound;
    }


    private string ImageBdToURL(byte[] FotoDB)
    {
        if (FotoDB == null || FotoDB.Length == 0)
        {
            var foldername = @"image\Usuario";
            var filename = "userDefault.png";
            var path1 = Path.Combine(env.WebRootPath, foldername, filename);
            var path2 = Path.Combine("\\", foldername, filename);//ruta relativa img
            Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");//ruta absoluta

            return location.AbsoluteUri;
        }
        else
        {
            string imgBase64 = Convert.ToBase64String(FotoDB);
            return string.Format("data:imagen/*;base64,{0}", imgBase64);
        }
    }

    [HttpGet, ActionName("ConfirmarEliminar")]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null) return BadRequest("Usuario no encontrado");

        var userVM = await usuarioVMSearchFirstOr(int.Parse(id.ToString()));

        if (userVM == null) return BadRequest("Usuario no encontrado");

        return View(userVM);
    }

    [HttpPost, ActionName("ConfirmarEliminar")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
            return BadRequest("");

        var userToDeleted = await context.Usuarios.FindAsync(id);

        if (userToDeleted == null)
            return BadRequest("Usuario no encontrado");

        context.Usuarios.Remove(userToDeleted);
        context.SaveChanges();

        return View(nameof(Index));
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
