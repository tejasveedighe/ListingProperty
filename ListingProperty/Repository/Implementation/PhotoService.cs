using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ListingProperty.Data;
using ListingProperty.Repository.Interface;

namespace ListingProperty.Repository.Implementation
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly AppContextDb _context;



        public PhotoService(IConfiguration config, AppContextDb context)
        {


            Account account = new Account(

                 config.GetSection("CloudinarySettings:CloudName").Value,
                config.GetSection("CloudinarySettings:ApiKey").Value,
                config.GetSection("CloudinarySettings:ApiSecret").Value



                );

             _cloudinary = new Cloudinary(account);

            _context = context;
        }

       public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile photo)
        {

            var uploadResult = new ImageUploadResult();
            if (photo.Length > 0)
            {

                using var stream = photo.OpenReadStream();
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(photo.FileName, stream),
                    Transformation = new Transformation()
                    .Height(500).Width(800)
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParam);

            }
            return uploadResult;



        }

        
    }
}
