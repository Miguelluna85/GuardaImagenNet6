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
        private string folderName = @"image\Usuario\";
        private string imgDefault = "userDefault.png";

        public FotoPathController(IWebHostEnvironment Env, PruebasDBContext Context)
        {
            this.context = Context;
            this.env = Env;
        }

        public async Task<IActionResult> Listado()
        {
            IEnumerable<Usuario> listUserDB = await context.Usuarios
                .Where(u => u.GuardaFotoDisco == false)
                .AsNoTracking()
                .ToListAsync();
            List<UsuarioEditVM> listUserVM = new List<UsuarioEditVM>();

            foreach (Usuario userDB in listUserDB)
            {
                UsuarioEditVM usrVM = new UsuarioEditVM
                {
                    ID = userDB.Id,
                    NombreUsuario = userDB.UserName,
                    Contrasenya = userDB.Password.Length <= 10 ? userDB.Password + " ..." : userDB.Password.Substring(0, 10) + " ...",
                    FotoPath = FotoPathDBToURL(userDB.FotoPath),
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

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("NombreUsuario,Contrasenya,FotoByte,Activo")] UsuarioCreateVM usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            if (usuario == null)
                return BadRequest("Error usuario no valido");

            //buscar si un usuario con ese nombre ya existe.

            Usuario userBD = new Usuario();

            if (usuario.FotoByte != null)
            {
                Guid idGuid = Guid.NewGuid();
                // string photoName = Path.GetFileName(usuario.FotoByte.FileName);
                string extPhoto = Path.GetExtension(usuario.FotoByte.FileName);

                if (!HelperImagenes.ExtensionsFotosValid(extPhoto))
                    return BadRequest("El archivo no es una imagen valida");

                string NameGuidFoto = idGuid.ToString().Replace("-", "_") + "_"
                    + usuario.NombreUsuario + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extPhoto;

                string rutaFoto = folderName + NameGuidFoto;
                string path = Path.Combine(env.WebRootPath, folderName, NameGuidFoto);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await usuario.FotoByte.CopyToAsync(stream);
                }

                userBD.FotoPath = rutaFoto;

            }

            userBD.UserName = usuario.NombreUsuario;
            userBD.Password = HelperCifraPassword.EncodePasswSHA256(usuario.Contrasenya);
            userBD.Estatus = usuario.Activo;
            userBD.GuardaFotoDisco = false;
            context.Usuarios.Add(userBD);

            await context.SaveChangesAsync();

            return RedirectToAction("Listado", "FotoPath");
        }


        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> Editar(int id)
        {
            if (id == 0) return BadRequest("Usuario no Proporcionado");

            var userFound = await usuarioVMSearchFirstOr(id);

            if (userFound == null)
                return BadRequest("Usuario No Encontrado.");

            return View(userFound);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> Editar(int? id, [Bind("NombreUsuario, Contrasenya,FotoByte,Activo")] UsuarioEditVM userVM)
        {
            if (!ModelState.IsValid) return View(userVM);
            if (id == null && userVM == null) return NotFound();
            if (id <= 0) return BadRequest("Usuario no encontrado");

            //var usrBD1 = await context.Usuarios.AsNoTracking().FindAsync(id);
            var userToUpdate = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (userVM.FotoByte != null)
            {
                Guid idGuid = Guid.NewGuid();
                // string photoName = Path.GetFileName(usuario.FotoByte.FileName);
                string extPhoto = Path.GetExtension(userVM.FotoByte.FileName);

                if (!HelperImagenes.ExtensionsFotosValid(extPhoto))
                    return BadRequest("El archivo no es una imagen valida");

                string NameGuidFoto = idGuid.ToString().Replace("-", "_") + "_"
                    + userVM.NombreUsuario + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extPhoto;

                string rutaFoto = folderName + NameGuidFoto;
                string path = Path.Combine(env.WebRootPath, folderName, NameGuidFoto);

                if (!string.IsNullOrEmpty(userToUpdate.FotoPath))
                {
                    string pathFotoDelete = FotoPathAbsolute(userToUpdate.FotoPath);
                    if (System.IO.File.Exists(pathFotoDelete))
                    {
                        System.IO.File.Delete(pathFotoDelete);
                    }
                    else
                    {
                        //archivo no existe
                        //hacer algo con la imagen
                    }
                }

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await userVM.FotoByte.CopyToAsync(stream);
                }
                userToUpdate.FotoPath = rutaFoto;
            }
            else
            {

            }

            userToUpdate.Id = int.Parse(id.ToString());
            userToUpdate.Password = HelperCifraPassword.EncodePasswSHA256(userVM.Contrasenya);
            userToUpdate.Estatus = userVM.Activo;
            userToUpdate.FechaModifico = DateTime.Now;

            if (await TryUpdateModelAsync<Usuario>(
                userToUpdate,
                "",
                u => u.Password, u => u.Estatus, u => u.FechaModifico, u => u.FotoPath
                ))
            {
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Listado));
        }

        [HttpGet, ActionName("details")]
        public async Task<ActionResult> Detalle(int id)
        {
            UsuarioEditVM userFound = await usuarioVMSearchFirstOr(id);

            if (userFound == null)
                return BadRequest("Usuario No Encontrado.");

            return View(userFound);
        }


        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return BadRequest("Usuario no encontrado");

            UsuarioEditVM userVM = await usuarioVMSearchFirstOr(int.Parse(id.ToString()));

            if (userVM == null)
                return BadRequest("Usuario no encontrado");

            return View("Delete", userVM);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest("Usuario no proporcionado");

            var userToDeleted = await context.Usuarios.FindAsync(id);

            if (userToDeleted == null)
                return BadRequest("Usuario no encontrado");

            context.Usuarios.Remove(userToDeleted);
            context.SaveChanges();

            return RedirectToAction(nameof(Listado));

            //para mejorar el rendimiento, solo que no elimina encascada
            //Student studentToDelete = new Student() { ID = id };
            //_context.Entry(studentToDelete).State = EntityState.Deleted;
         
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
                FotoPath = FotoPathDBToURL(userDB.FotoPath),
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
                FotoPath = FotoPathDBToURL(userDB.FotoPath),
                Activo = userDB.Estatus ?? false
            };
            return userFound;
        }

        private string FotoPathDBToURL(string pathFotoBD)
        {
            //Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");//ruta uri absoluta
            return Path.Combine("\\", pathFotoBD ?? folderName + imgDefault);
        }
        private string FotoPathAbsolute(string pathFotoBD)
        => Path.Combine(env.WebRootPath, pathFotoBD ?? folderName + imgDefault);

    }
}
