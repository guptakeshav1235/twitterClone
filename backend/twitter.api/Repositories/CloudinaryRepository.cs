
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace twitter.api.Repositories
{
    public class CloudinaryRepository : ICloudinaryRepository
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryRepository(IConfiguration configuration)
        {
            cloudinary = new Cloudinary(new Account(
                    configuration["Cloudinary:CloudName"],
                    configuration["Cloudinary:ApiKey"],
                    configuration["Cloudinary:ApiSecret"]
                ));
        }
        public async Task DeleteImageAsync(string imageUrl)
        {
            var publicId = imageUrl.Split('/').Last().Split('.').First();
             cloudinary.Destroy(new DeletionParams(publicId));
        }

        public async Task<string?> UploadImageAsync(string base64Image)
        {
            // Remove the data:image/jpeg;base64, prefix if it exists
            var base64Data = base64Image.Contains(",")
                ? base64Image.Substring(base64Image.IndexOf(",") + 1)
                : base64Image;

            // Convert base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            using (var memoryStream = new MemoryStream(imageBytes))
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription("image.jpg", memoryStream), // Use a memory stream here
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };

                var uploadResult = cloudinary.Upload(uploadParams);

                return uploadResult?.SecureUri?.ToString();
            }
        }
    }
}
