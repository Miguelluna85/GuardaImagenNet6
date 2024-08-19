namespace GuardaImagenNet6.Providers
{
    public class PathProviderFile
    {
        private readonly IWebHostEnvironment env;
        public PathProviderFile(IWebHostEnvironment Env)
        {
                this.env = Env;
        }
        public string MapPath(string fileName,string carpetaName )
        {
            string path = Path.Combine(this.env.WebRootPath, carpetaName, fileName);

            return path;
        }
    }
}
