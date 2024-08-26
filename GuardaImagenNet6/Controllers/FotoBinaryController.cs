using GuardaImagenNet6.Helpers;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.Repository;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Controllers
{
    public class FotoBinaryController : Controller
    {
        private readonly PruebasDBContext context;
        private readonly IWebHostEnvironment env;

        public FotoBinaryController(IWebHostEnvironment _env, PruebasDBContext _context)
        {
            context = _context;
            env = _env;
        }
        public async Task<IActionResult> Listado(string buscar, string ordenActual, int? numPag
                                                                                , string filtroActual)
        {

            IQueryable<UsuarioEditVM> listUserBD = (from usuario in context.Usuarios
                                             .Where(usuario => usuario.GuardaFotoDisco == true)
                                                    select new UsuarioEditVM
                                                    {
                                                        ID = usuario.Id,
                                                        NombreUsuario = usuario.UserName,
                                                        Contrasenya = usuario.Password.Length <= 10 ? usuario.Password + " ..." : usuario.Password.Substring(0, 10) + " ...",
                                                        FotoSrc = HelperImagenes.imageByteToURL(usuario.FotoBd),
                                                        //FotoByte = usuario.FotoBd,
                                                        Activo = usuario.Estatus ?? false,
                                                        FechaAlta = usuario.FechaAlta
                                                    });
                       
            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroUserNombre"] = String.IsNullOrEmpty(ordenActual) ? "UserNombreDescendente" : "";
            ViewData["FiltroFecha"] = ordenActual == "FechaAscendente" ? "FechaDescendente" : "FechaAscendente";

            if (buscar != null)
                numPag = 1;
            else
                buscar = filtroActual;

            ViewData["FiltroActual"] = buscar;

            if (!String.IsNullOrEmpty(buscar))
                listUserBD = listUserBD.Where(s => s.NombreUsuario!.Contains(buscar));

            switch (ordenActual)
            {
                case "UserNombreDescendente":
                    listUserBD = listUserBD.OrderByDescending(usuario => usuario.NombreUsuario);
                    break;
                case "FechaDescendente":
                    listUserBD = listUserBD.OrderByDescending(usuarios => usuarios.FechaAlta);
                    break;
                case "FechaAscendente":
                    listUserBD = listUserBD.OrderBy(usuarios => usuarios.FechaAlta);
                    break;
                default:
                    listUserBD = listUserBD.OrderBy(usuario => usuario.NombreUsuario);
                    break;
            }
            int cantidadRegistros = 10;
            PaginacionList<UsuarioEditVM> listPaginada = await PaginacionList<UsuarioEditVM>
                            .CrearPaginacion(listUserBD.AsNoTracking(), numPag ?? 1, cantidadRegistros);
            
            return View(listPaginada);
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
            if (!ModelState.IsValid) return View(usuario);

            if (usuario == null) return BadRequest("Error usuario no valido");

            //buscar si un usuario con ese nombre ya existe.
            Usuario userFound = await context.Usuarios
                .FirstOrDefaultAsync(
                u => u.UserName.ToLower().Equals(usuario.NombreUsuario.ToLower()));

            if (userFound != null)
            {
                return View(usuario);
            }
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
            userBD.Password = HelperCifraPassword.EncodePasswSHA256(usuario.Contrasenya);
            userBD.Estatus = usuario.Activo;
            userBD.GuardaFotoDisco = true;
            context.Usuarios.Add(userBD);

            await context.SaveChangesAsync();

            return RedirectToAction("Listado", "FotoBinary");
        }

        [HttpGet, ActionName("Editar")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return BadRequest("Usuario no Proporcionado");

            UsuarioEditVM userFound = await usuarioBaseVMSearchFirstOr(id);

            if (userFound == null)
                return BadRequest("Usuario No Encontrado.");

            return View(userFound);
        }

        [HttpPost, ActionName("Editar")]
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
                    await userVM.FotoFile.CopyToAsync(streamPhoto);
                    userToUpdate.FotoBd = streamPhoto.ToArray();
                }
            }
            else
            {
            }

            userToUpdate.Id = int.Parse(id.ToString());
            //password se puede quitar 
            userToUpdate.Password = HelperCifraPassword.EncodePasswSHA256(userVM.Contrasenya);
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

            return RedirectToAction(nameof(Listado));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            UsuarioEditVM userFound = await usuarioBaseVMSearchFirstOr(id);

            if (userFound == null)
                return BadRequest("Usuario No Encontrado.");

            return View(userFound);
        }

        [HttpGet, ActionName("Eliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Usuario no encontrado");

            UsuarioEditVM userBaseVM = await usuarioBaseVMSearchFirstOr(int.Parse(id.ToString()));

            if (userBaseVM == null)
                return BadRequest("Usuario no encontrado");

            return View("Eliminar", userBaseVM);
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

            return RedirectToAction(nameof(Listado));
            //para mejorar el rendimiento, solo que no elimina encascada
            //Student studentToDelete = new Student() { ID = id };
            //_context.Entry(studentToDelete).State = EntityState.Deleted;
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }


        private async Task<UsuarioEditVM> usuarioBaseVMSearchFind(int id)
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
                FotoSrc = HelperImagenes.imageByteToURL(userDB.FotoBd),
                Activo = userDB.Estatus ?? false
            };
            return userFound;
        }
        private async Task<UsuarioEditVM> usuarioBaseVMSearchFirstOr(int id)
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
                FotoSrc = HelperImagenes.imageByteToURL(userDB.FotoBd),
                Activo = userDB.Estatus ?? false
            };
            return userFound;
        }
        //private string ImageBdToURL(byte[] FotoDB)
        //{
        //    if (FotoDB == null || FotoDB.Length == 0)
        //    {
        //        var foldername = @"image\Usuario";
        //        var filename = "userDefault.png";
        //        var path1 = Path.Combine(env.WebRootPath, foldername, filename);
        //        var path2 = Path.Combine("\\", foldername, filename);//ruta relativa img
        //        Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");//ruta absoluta
        //        return location.AbsoluteUri;
        //    }
        //    else
        //    {
        //        string imgBase64 = Convert.ToBase64String(FotoDB);
        //        return string.Format("data:imagen/*;base64,{0}", imgBase64);
        //    }
        //}

    }
}
