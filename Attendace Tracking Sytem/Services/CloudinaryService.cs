using Attendace_Tracking_Sytem.ApiSettings;
using Attendace_Tracking_Sytem.DTO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Attendace_Tracking_Sytem.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<CloudinaryFileDTO> UploadFile(IFormFile file)
        {
            //OPEN THE STREAM TO READ THE FILE
            using var stream = file.OpenReadStream();

            //UPLOAD PARAMETER
            var parameters = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            //UPLOAD ON CLOUDINARY
            var result = await _cloudinary.UploadAsync(parameters);

            return new CloudinaryFileDTO
            {
                ImageId = result.PublicId,
                ImageUrlPath = result.SecureUrl.ToString()
            };
        }

        public async Task<CloudinaryFileDTO> UploadImage(IFormFile file)
        {
            //OPEN STREAM
            using var stream = file.OpenReadStream();

            //SET PARAMETERS 
            var paramaters = new ImageUploadParams
            {
                File = new FileDescription(file.FileName,stream)
            };

            

            var result = await _cloudinary.UploadAsync(paramaters);

            return new CloudinaryFileDTO
            {
                ImageId = result.PublicId,
                ImageUrlPath = result.SecureUrl.ToString()
            };
        }
    }
}
