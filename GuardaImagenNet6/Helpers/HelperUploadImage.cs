using GuardaImagenNet6.Providers;
using System.IO;

namespace GuardaImagenNet6.Helpers
{
    public class HelperUploadImage
    {
        private readonly IWebHostEnvironment env;
        public HelperUploadImage(IWebHostEnvironment Env)
        {
            env = Env;
        }
        public async Task<String> UploadImageAsync(IFormFile file, string imageName, string carpeta)
        {
            PathProviderFile pathProvider = new PathProviderFile(env);
           string path= pathProvider.MapPath("", "");

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;
        }
    }
}
