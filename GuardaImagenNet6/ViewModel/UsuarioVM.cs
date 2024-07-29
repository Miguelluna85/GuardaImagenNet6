namespace GuardaImagenNet6.ViewModel;

public class UsuarioVM
{
    public int ID { get; set; }
    public string NombreUsuario { get; set; }
    public string Contrasenya { get; set; }
    public string FotoPath { get; set; }
    public IFormFile FotoByte { get; set; }
    public bool Activo { get; set; }


}
