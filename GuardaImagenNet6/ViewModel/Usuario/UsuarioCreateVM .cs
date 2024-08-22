using GuardaImagenNet6.Models.Contexto;
using GuardaImagenNet6.Validators.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace GuardaImagenNet6.ViewModel.Usuario;

public class UsuarioCreateVM 
{
    [Remote(action: "ExistsUserName", controller: "ValidationUser")]
    [Required(ErrorMessage = "Dato requerido.")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "Minimo 4 maximo 30 caracteres")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "Dato requerido")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Minimo {1} maximo {0}")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Se requiere al menos una letra mayuscula, un numéro y logitud de 8")]
    public string Contrasenya { get; set; }

    public string FotoPath { get; set; }
    public IFormFile FotoByte { get; set; }
    public string FotoSrc { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }
    public bool GuardaFotoDisco { get; set; }     
}

 