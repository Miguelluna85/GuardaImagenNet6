using GuardaImagenNet6.Helpers;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Controllers
{
    public class FotoPathController : Controller
    {
        private readonly PruebasDBContext context;
        private readonly IWebHostEnvironment env;

        public FotoPathController(IWebHostEnvironment Env, PruebasDBContext Context)
        {
            this.context = Context;
            this.env = Env;
        }

        public async Task<IActionResult> Listado()
        {
            IEnumerable<Usuario> listUserDB = await context.Usuarios.AsNoTracking().ToListAsync();
            List<UsuarioVM> listUserVM = new List<UsuarioVM>();

            foreach (Usuario userDB in listUserDB)
            {
                UsuarioVM usrVM = new UsuarioVM
                {
                    ID = userDB.Id,
                    NombreUsuario = userDB.UserName,
                    Contrasenya = userDB.Password,
                    FotoSrc = userDB.FotoPath,
                    Activo = userDB.Estatus ?? false,
                    FechaAlta = userDB.FechaAlta
                };
                listUserVM.Add(usrVM);
            }
            return View(listUserVM);
        }

        [HttpGet, ActionName("Create")]
        public IActionResult Crear()
        {
            
            return View();
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioVM usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            if (usuario == null)
                return BadRequest("Error usuario no valido");

            //buscar si un usuario con ese nombre ya existe.

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

            return RedirectToAction("Listado", "FotoBinary");
        }


        [HttpGet,ActionName("Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet, ActionName("details")]
        public IActionResult Detalle()
        {
            return View();
        }

    }
}
