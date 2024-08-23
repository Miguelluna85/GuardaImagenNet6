using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using GuardaImagenNet6.Models;
using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.ViewModel.Usuario;
using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Services.Validators.Usuario;

//No funciona
//[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class ValidationUser : Controller
{
    public readonly PruebasDBContext context;
    public ValidationUser(PruebasDBContext Context)
    {
        context = Context;
    }
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> ExistsUserName(string nombreUsuario)
    {
        Models.Usuario userFound = await context.Usuarios
            .FirstOrDefaultAsync(
            u => u.UserName.ToLower().Equals(nombreUsuario.ToLower()));

        if (userFound != null)
            return Json($"El nombre de usuario {nombreUsuario} ya existe.");

        else
            return Json(true);
    }

    //public override bool IsValid(object? value)
    //{
    //    string nombreUsuario = value as string;
    //    PruebasDBContext context = new PruebasDBContext();

    //    Models.Usuario userFound = context.Usuarios
    //        .FirstOrDefault(
    //        u => u.UserName.ToLower().Equals(nombreUsuario.ToLower()));

    //    if (userFound != null)
    //        return false;
    //    // Json($"El nombre de usuario {nombreUsuario} ya existe.");

    //    else
    //        return true;        
    //}
}
