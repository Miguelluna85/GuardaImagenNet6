namespace GuardaImagenNet6.ViewModel;

public class ImagenesUtility
{
    public bool ExtensionsFotosValid(string extension)
    {
        string[] extensionesValid = { ".jpg", "jpeg", ".png" };

        string ext = Path.GetExtension(extension).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !extensionesValid.Contains(extension))
            return false;
        else
            return true;

    }
}
