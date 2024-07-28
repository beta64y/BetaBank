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
        public static IFormFile Base64ToIFormFile(string base64Image, string fileName)
        {
            // "data:image/png;base64," kısmını ayıklıyoruz
            var base64Data = base64Image.Split(',')[1];
            var imageBytes = Convert.FromBase64String(base64Data);

            var ms = new MemoryStream(imageBytes);
            var formFile = new FormFile(ms, 0, ms.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png" // İhtiyaca göre content type ayarlayın
            };

            return formFile;
        }
    }
}
