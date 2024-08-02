using System.ComponentModel.DataAnnotations;

namespace GuardaImagenNet6.ViewModel.Usuario;

public class UsuarioVM
{
    public int ID { get; set; }
    [Required]
    [StringLength(50,MinimumLength =10,ErrorMessage ="Minimo {1} maximo {0}")]
    public string NombreUsuario { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Minimo {1} maximo {0}")]
    public string Contrasenya { get; set; }
    public string FotoPath { get; set; }
    [Required]
    public IFormFile FotoByte { get; set; }
    public string FotoSrc { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }


}
