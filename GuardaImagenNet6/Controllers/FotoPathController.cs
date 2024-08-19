using GuardaImagenNet6.Helpers;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.Providers;
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
            var path1 = env.ApplicationName;
            var path2 = env.EnvironmentName;
            var path3 = env.ContentRootPath;
            var path4 = env.ContentRootFileProvider;
            var path5 = env.WebRootFileProvider;
            var path6 = env.WebRootPath;
            var path7 = env.IsEnvironment;




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

           
                string foldername = @"image\Usuario";                
                string path = Path.Combine(env.WebRootPath, foldername, photoName);
                 
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await usuario.FotoByte.CopyToAsync(stream);
                }
                userBD.FotoPath = path;

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
