namespace twitter.api.Repositories
{
    public interface ICloudinaryRepository
    {
        Task<string?> UploadImageAsync(string base64Image);
        Task DeleteImageAsync(string imageUrl);
    }
}
