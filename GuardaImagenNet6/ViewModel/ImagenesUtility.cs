namespace GuardaImagenNet6.ViewModel;

public class ImagenesUtility
{
    public bool ExtensionsFotosValid(string extension)
    {
        string[] extensionesValid = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

        string ext = Path.GetExtension(extension).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !extensionesValid.Contains(extension))
            return false;
        else
            return true;

    }
    public string ConvertBinaryToURLImage(byte[] fotoBinary)
    {
        string imgBase64 = Convert.ToBase64String(fotoBinary);
        return string.Format("data:imagen/*;base64,{0}", imgBase64);
    }
}
