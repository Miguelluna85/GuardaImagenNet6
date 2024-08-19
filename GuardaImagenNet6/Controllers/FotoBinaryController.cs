using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Controllers
{
    public class FotoBinaryController : Controller
    {
        private readonly PruebasDBContext context;
        private readonly IWebHostEnvironment env;

        public FotoBinaryController(IWebHostEnvironment _env, ILogger<HomeController> logger, PruebasDBContext _context)
        {
            env = _env;
            context = _context;
        }
        public async Task<IActionResult> Listado()
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

        [HttpGet,ActionName("Editar")]
        public async Task<IActionResult> Edit(int id)
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

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("NombreUsuario, Contrasenya,FotoByte,Activo")] UsuarioVM userVM)
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

        public async Task<IActionResult> Detalle(int id)
        {
            var userFound = await usuarioVMSearchFirstOr(id);
            if (userFound == null)
                return BadRequest("Usuario No Encontrado.");

            return View(userFound);
        }

        [HttpGet, ActionName("Eliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Usuario no encontrado");

            var userVM = await usuarioVMSearchFirstOr(int.Parse(id.ToString()));

            if (userVM == null)
                return BadRequest("Usuario no encontrado");

            return View("Eliminar", userVM);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
                return BadRequest("");

            var userToDeleted = await context.Usuarios.FindAsync(id);

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


        private async Task<UsuarioVM> usuarioVMSearchFind(int id)
        {
            if (id <= 0)
                return null;

            Usuario userDB = await context.Usuarios.FindAsync(id);
            if (userDB == null)
                return null;

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

    }
}
