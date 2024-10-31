
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
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(base64Image),
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult?.SecureUri?.ToString();
        }
    }
}
