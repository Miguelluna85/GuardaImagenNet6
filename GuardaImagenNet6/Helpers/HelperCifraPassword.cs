using System.Security.Cryptography;
using System.Text;

namespace GuardaImagenNet6.Helpers;

public static class HelperCifraPassword
{

    public static string GenerateSalt()
    {
        Random random = new Random();
        string salt = "";
        for (int i = 1; i <= 50; i++)
        {
            int numero = random.Next(0, 255);
            char letra = Convert.ToChar(numero);
            salt += letra;
        }
        return salt;
    }

    public static bool compareArrays(byte[] a, byte[] b)
    {
        bool iguales = true;
        if (a.Length != b.Length)
        {
            iguales = false;
        }
        else
        {
            //comparamos byte a byte
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Equals(b[i]) == false)
                {
                    iguales = false;
                    break;
                }
            }
        }

        return iguales;
    }

    public static byte[] EncodePasswSHA256Managed(string password)
    {
        string salt = GenerateSalt();
        string contenido = password + salt;
        SHA256Managed sha = new SHA256Managed();

        byte[] salida = Encoding.UTF8.GetBytes(contenido);

        for (int i = 1; i <= 107; i++)
            salida = sha.ComputeHash(salida);


        sha.Clear();
        return salida;

    }

    public static string EncodePasswCryptoProvider(string Password)
    {
        SHA1 sha1 = new SHA1CryptoServiceProvider();

        byte[] inputBytes = new UnicodeEncoding().GetBytes(Password);
        byte[] hash = sha1.ComputeHash(inputBytes);

        return Convert.ToBase64String(hash);
    }

    public static string EncodePasswSHA256(string Password)
    {
        SHA256 sha256 = SHA256.Create();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] stream = null;
        StringBuilder sb = new StringBuilder();

        stream = sha256.ComputeHash(encoding.GetBytes(Password));

        for (int i = 0; i < stream.Length; i++)
            sb.AppendFormat("{0:x2}", stream[i]);

        return sb.ToString();

    }
}
