namespace GuardaImagenNet6.Services.Providers
{
    public class PathProviderFile
    {
        private readonly IWebHostEnvironment env;
        public PathProviderFile(IWebHostEnvironment Env)
        {
            env = Env;
        }
        public string MapPath(string fileName, string carpetaName)
        {
            string path = Path.Combine(env.WebRootPath, carpetaName, fileName);

            return path;
        }
    }
}
