namespace BetaBank.Services.Validators
{
    public static class FileExtension
    {
        public static bool CheckFileType(this IFormFile file, string fileType) // image/
            => file.ContentType.Contains(fileType);
        public static bool CheckFileSize(this IFormFile file, int fileSize)
            => file.Length/1024 < fileSize;
        
    }
}
