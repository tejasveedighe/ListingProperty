using CloudinaryDotNet.Actions;

namespace ListingProperty.Repository.Interface
{
    public interface IPhotoService
    {
        public Task<ImageUploadResult> UploadPhotoAsync(IFormFile photo);


    }
}
