using Microsoft.AspNetCore.Hosting;

namespace BetaBank.Services.Implementations
{
    

    public static class ImageSaverService
    {
        public async static Task<string> SaveImage(IFormFile file, string webRootPath,string folder = "website-images")
        {
            string fileName = $"{Guid.NewGuid()}-{file.FileName}";
            string path = Path.Combine(webRootPath, "img", folder, fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;

        }
    }
}
