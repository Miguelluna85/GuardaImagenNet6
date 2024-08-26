namespace GuardaImagenNet6.Helpers;

public static class HelperImagenes
{
    public static bool ExtensionsFotosValid(string extension)
    {
        string[] extensionesValid = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        string ext = Path.GetExtension(extension).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !extensionesValid.Contains(extension))
            return false;
        else
            return true;

    }

    public static string imageByteToURL(byte[] fotoFile)
    {        
        if (fotoFile == null || fotoFile.Length == 0)
        {
            string foldername = @"image\Usuario\";
            string filename = @"userDefault.png";
            var url = Path.Combine("\\", foldername + filename);
            return url;
        }
        else
        {
            string imgBase64 = Convert.ToBase64String(fotoFile);
            return string.Format("data:imagen/*;base64,{0}", imgBase64);
        }
    }
    //private static string ImageBdToUri(byte[] FotoDB)
    //{
    //    if (FotoDB == null || FotoDB.Length == 0)
    //    {
    //        //string cadena = HttpContext.Current.Request.Url.AbsoluteUri;

    //        var foldername = @"image\Usuario";
    //        var filename = "userDefault.png";
    //        //var path1 = Path.Combine(env.WebRootPath, foldername, filename);
    //        //var path2 = Path.Combine("\\", foldername, filename);//ruta relativa img
    //        Uri location = new Uri($"{Request.Scheme}://{Request.Host}/{foldername}/{filename}");//ruta absoluta
    //        return location.AbsoluteUri;
    //    }
    //    else
    //    {
    //        string imgBase64 = Convert.ToBase64String(FotoDB);
    //        return string.Format("data:imagen/*;base64,{0}", imgBase64);
    //    }
    //}

    public static string ConvertBinaryToImage64(byte[] fotoBinary)
    {
        string imgBase64 = Convert.ToBase64String(fotoBinary);
        return string.Format("data:imagen/*;base64,{0}", imgBase64);
    }
    public static byte[] imagenFileToBytes(string pathImagen)
    {
        FileStream fs = new FileStream(pathImagen, FileMode.OpenOrCreate, FileAccess.Read);
        byte[] imagenBytes = new byte[fs.Length];
        fs.Read(imagenBytes, 0, Convert.ToInt32(fs.Length));
        fs.Close();
        return imagenBytes;
    }
    public static string imageDefaultPath(IWebHostEnvironment env)
    {
        var foldername = @"image\Usuario";
        var filename = "userDefault.png";
        var path1 = Path.Combine(env.WebRootPath, foldername, filename);
        var path2 = Path.Combine("\\", foldername, filename);//ruta relativa img
        return path1;
    }

    public static byte[] Imagen_A_Bytes(string ruta)
    {
        FileStream foto = new FileStream(ruta, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        byte[] arreglo = new byte[foto.Length];
        BinaryReader reader = new BinaryReader(foto);
        arreglo = reader.ReadBytes(Convert.ToInt32(foto.Length));
        return arreglo;
    }

    //public static String Redimensionar(Image Imagen_Original, string nombre)
    //{
    //    //RUTA DEL DIRECTORIO TEMPORAL
    //    String DirTemp = Path.GetTempPath() + @"\" + nombre + ".jpg";
    //    //IMAGEN ORIGINAL A REDIMENSIONAR
    //    BitMap imagen = new Bitmap(Imagen_Original);
    //    //CREAMOS UN MAPA DE BIT CON LAS DIMENSIONES QUE QUEREMOS PARA LA NUEVA IMAGEN
    //    Bitmap nuevaImagen = new Bitmap(Imagen_Original.Width, Imagen_Original.Height);
    //    //CREAMOS UN NUEVO GRAFICO
    //    Graphics gr = Graphics.FromImage(nuevaImagen);

    //    //DIBUJAMOS LA NUEVA IMAGEN
    //    gr.DrawImage(imagen, 0, 0, nuevaImagen.Width, nuevaImagen.Height);

    //    //LIBERAMOS RECURSOS
    //    gr.Dispose();

    //    //GUARDAMOS LA NUEVA IMAGEN ESPECIFICAMOS LA RUTA Y EL FORMATO
    //    nuevaImagen.Save(DirTemp, System.Drawing.Imaging.ImageFormat.Jpeg);
    //    //LIBERAMOS RECURSOS
    //    nuevaImagen.Dispose();
    //    imagen.Dispose();

    //    return DirTemp;

    //}

    //FUNCION PARA CONVERTIR LA IMAGEN A BYTES


    //FUNCION PARA CONVERTIR DE BYTES A IMAGEN

    //public static Image Bytes_A_Imagen(Byte[] ImgBytes)
    //{
    //    Bitmap imagen = null;
    //    Byte[] bytes = (Byte[])(ImgBytes);
    //    MemoryStream ms = new MemoryStream(bytes);
    //    imagen = new Bitmap(ms);
    //    return imagen;
    //}




}

