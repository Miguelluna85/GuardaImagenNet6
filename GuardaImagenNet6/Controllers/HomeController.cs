using GuardaImagenNet6.Helpers;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        IEnumerable<Usuario> listUserBD = await context.Usuarios.AsNoTracking().ToListAsync();
        List<UsuarioEditVM> listUserVM = new List<UsuarioEditVM>();

        foreach (Usuario userDB in listUserBD)
        {
            UsuarioEditVM usrVM = new UsuarioEditVM
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
    public async Task<IActionResult> Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioCreateVM usuario)
    {
        if (!ModelState.IsValid)
            return View(usuario);

        if (usuario == null)
            return BadRequest("Error usuario no valido");

        Usuario userBD = new Usuario();

        if (usuario.FotoByte != null)
        {
            string photoName = Path.GetFileName(usuario.FotoByte.FileName);
            string extPhoto = Path.GetExtension(usuario.FotoByte.FileName);

            if (!HelperImagenes.ExtensionsFotosValid(extPhoto))
                return BadRequest("El archivo no es una imagen valida");

            using (var streamPhoto = new MemoryStream())
            {
                await usuario.FotoByte.CopyToAsync(streamPhoto);
                userBD.FotoBd = streamPhoto.ToArray();
            }
        }

        userBD.UserName = usuario.NombreUsuario;
        userBD.Password = usuario.Contrasenya;
        userBD.Estatus = usuario.Activo;
        context.Usuarios.Add(userBD);

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
    }

    [HttpPost, ActionName("Actualizar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("NombreUsuario, Contrasenya,FotoByte,Activo")] UsuarioEditVM userVM)
    {
        if (!ModelState.IsValid) return View(userVM);
        if (id == null && userVM == null) return NotFound();
        if (id <= 0) return BadRequest("Usuario no encontrado");

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
        else
        { 
        }

        userToUpdate.Id = int.Parse(id.ToString());
        userToUpdate.Password = string.IsNullOrEmpty(userVM.Contrasenya) ? userToUpdate.Password : userVM.Contrasenya;
        userToUpdate.Estatus = userVM.Activo;
        userToUpdate.FechaModifico = DateTime.Now;

        if (await TryUpdateModelAsync<Usuario>(
            userToUpdate,
            "",
            u => u.Password, u => u.Estatus, u => u.FechaModifico, u => u.FotoBd
            ))
        {
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detalles(int id)
    {
        UsuarioEditVM userFound = await usuarioVMSearchFirstOr(id);
        if (userFound == null)
            return BadRequest("Usuario No Encontrado.");

        return View(userFound);
    }
    private async Task<UsuarioEditVM> usuarioVMSearchFind(int id)
    {
        if (id <= 0) 
            return null;

        Usuario userDB = await context.Usuarios.FindAsync(id);
        if (userDB == null)
            return null;

        UsuarioEditVM userFound = new UsuarioEditVM
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
    private async Task<UsuarioEditVM> usuarioVMSearchFirstOr(int id)
    {
        if (id <= 0) return null;

        Usuario userDB = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (userDB == null) return null;

        UsuarioEditVM userFound = new UsuarioEditVM
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

    [HttpGet, ActionName("Eliminar")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return BadRequest("Usuario no encontrado");

        UsuarioEditVM userVM = await usuarioVMSearchFirstOr(int.Parse(id.ToString()));

        if (userVM == null)
            return BadRequest("Usuario no encontrado");

        return View("Eliminar", userVM);
    }

    [HttpPost, ActionName("Eliminar")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (id <= 0)
            return BadRequest("");

        Usuario userToDeleted = await context.Usuarios.FindAsync(id);

        if (userToDeleted == null)
            return BadRequest("Usuario no encontrado");

        context.Usuarios.Remove(userToDeleted);
        context.SaveChanges();

        return RedirectToAction(nameof(Index));
        //para mejorar el rendimiento, solo que no elimina encascada
        //Student studentToDelete = new Student() { ID = id };
        //_context.Entry(studentToDelete).State = EntityState.Deleted;
        //await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Index));
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
