﻿using GuardaImagenNet6.Models;
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

        [HttpGet,ActionName("Edit")]
        public IActionResult Editar()
        {
            return View();
        }


    }
}
