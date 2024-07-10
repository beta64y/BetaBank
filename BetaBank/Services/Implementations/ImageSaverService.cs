using Microsoft.AspNetCore.Hosting;

namespace BetaBank.Services.Implementations
{
    

    public static class ImageSaverService
    {
        public async static Task<string> SaveImage(IFormFile file, string webRootPath)
        {
            string fileName = $"{Guid.NewGuid()}-{file.FileName}";
            string path = Path.Combine(webRootPath, "img", "website-images", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;

        }
    }
}
