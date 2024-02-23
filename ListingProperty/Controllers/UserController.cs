using ListingProperty.Data;
using ListingProperty.Helper;
using ListingProperty.Models;
using ListingProperty.Repository.Implementation;
using ListingProperty.Repository.Interface;
using ListingProperty.ViewModal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ListingProperty.Controllers
{





    public class UserController : Controller
    {
        private readonly AppContextDb _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoService _photoService;

        public UserController(AppContextDb context, IConfiguration config, IWebHostEnvironment webHostEnvironment, IPhotoService photoService)
        {
            _context = context;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _photoService = photoService;
        }





        public IActionResult Index()
        {
            return View();
        }




        [HttpGet]
        [Route("/user")]
        public async Task<IActionResult> GetAllUser()
        {


            var user = await _context.LpUser.ToListAsync();

            return Ok(user);

        }

        [HttpGet]
        [Route("/userDetails")]
        public async Task<IActionResult> GetUserByEmail(User user)
        {


            var userDetail = await _context.LpUser.FirstOrDefaultAsync(w => w.Email == user.Email);

            return Ok(user);

        }


        [HttpPost]
        [Route("/AddUser")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {

            try
            {
                if (user.UserType == "Admin")
                {

                    var RegisterUser = new User
                    {
                        UserType = "User",
                        Email = user.Email,
                        Name = user.Name,
                        Password = user.Password



                    };

                    _context.LpUser.Add(RegisterUser);
                }
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);

            }
            return Ok(user);
        }




        [HttpGet]
        [Route("/getAllproperty")]
        public async Task<IActionResult> GetAllProperty()
        {


            /*  var property = await _context.lpProperty.ToListAsync();
              var image = await _context.LpImages.ToListAsync();*/
            var combinedData = await (from property in _context.lpProperty
                                      join image in _context.LpImages
                                      on property.PropertyId equals image.PropertyId into propertyImages
                                      from img in propertyImages.DefaultIfEmpty()
                                      select new
                                      {
                                          PropertyId = property.PropertyId,
                                          PropertyTitle = property.PropertyTitle,
                                          PropertyType = property.PropertyType,
                                          Location = property.Location,
                                          Price = property.Price,
                                          NoBedroom = property.NoBedroom,
                                          NoBathroom = property.NoBathroom,
                                          SquareFeet = property.SquareFeet,
                                          Description = property.Description,
                                          Status = property.Status,
                                          DateListed = property.DateListed,
                                          DateUpdated = property.DateUpdated,
                                          Approved = property.Approved,


                                          ImageId = img != null ? img.Id : -1,
                                          PublicID = img != null ? img.PublicID : "",
                                          ImageUrl = img != null ? img.ImageUrl : ""
                                      }).ToListAsync();

            return Ok(combinedData);


        }






        [HttpPost]
        [Route("/searchProperty")]
        public async Task<IActionResult> SearchProperty([FromBody] SearchPropertyViewModel searchCriteria)
        {
            if (searchCriteria == null)
            {
                return BadRequest("Invalid search criteria");
            }

            var query = _context.lpProperty.AsQueryable();

            if (!string.IsNullOrEmpty(searchCriteria.Status))
            {
                query = query.Where(p => p.Status == searchCriteria.Status);
            }

            if (!string.IsNullOrEmpty(searchCriteria.Location))
            {
                query = query.Where(p => p.Location.Contains(searchCriteria.Location));
            }

            if (!string.IsNullOrEmpty(searchCriteria.PropertyType))
            {
                query = query.Where(p => p.PropertyType == searchCriteria.PropertyType);
            }

            if (searchCriteria.SquareFeet > 0)
            {
                query = query.Where(p => p.SquareFeet == searchCriteria.SquareFeet);
            }

            if (searchCriteria.NoBedroom > 0)
            {
                query = query.Where(p => p.NoBedroom == searchCriteria.NoBedroom);
            }

            if (searchCriteria.NoBathroom > 0)
            {
                query = query.Where(p => p.NoBathroom == searchCriteria.NoBathroom);
            }

            var searchResults = await query.ToListAsync();

            var propertyIds = await query.Select(p => p.PropertyId).ToListAsync();


            var imageUrls = await _context.LpImages
                .Where(img => propertyIds.Contains(img.PropertyId))
                .Select(img => new { img.PropertyId, img.ImageUrl })
                .ToListAsync();

            var combinedData = searchResults.Select(property => new
            {
                PropertyId = property.PropertyId,
                PropertyTitle = property.PropertyTitle,
                PropertyType = property.PropertyType,
                Location = property.Location,
                Price = property.Price,
                NoBedroom = property.NoBedroom,
                NoBathroom = property.NoBathroom,
                SquareFeet = property.SquareFeet,
                Description = property.Description,
                Images = imageUrls
        .Where(img => img.PropertyId == property.PropertyId)
        .Select(img => new
        {
            ImageUrl = img.ImageUrl
        })
        .ToList(),
                ContactNumber = property.ContactNumber,
                Status = property.Status,
                DateListed = property.DateListed,
                DateUpdated = property.DateUpdated,
                FavoriteProperty = property.FavoriteProperty
            })
.ToList();


            return Ok(combinedData);
        }

        [HttpPost]
        [Route("/BuyProperty")]
        public async Task<IActionResult> BuyProperty([FromBody] BuyRentModel searchCriteria)
        {
            if (searchCriteria == null)
            {
                return BadRequest("Invalid search criteria");
            }

            var query = _context.lpProperty.AsQueryable();


            if (!string.IsNullOrEmpty(searchCriteria.Status))
            {
                query = query.Where(p => p.Status == searchCriteria.Status);
            }


            var searchResults = await query.ToListAsync();


            var saleProperties = searchResults
                .Where(p => p.Status == "Sale")
                .Select(property => new
                {
                    PropertyId = property.PropertyId,
                    PropertyTitle = property.PropertyTitle,
                    PropertyType = property.PropertyType,
                    Location = property.Location,
                    Price = property.Price,
                    Images = _context.LpImages
                        .Where(img => img.PropertyId == property.PropertyId)
                        .Select(img => new { ImageUrl = img.ImageUrl })
                        .ToList(),
                    ContactNumber = property.ContactNumber,
                    Status = property.Status

                })
                .ToList();

            return Ok(saleProperties);
        }



        [HttpPost]
        [Route("/RentProperty")]
        public async Task<IActionResult> RentProperty([FromBody] BuyRentModel searchCriteria)
        {
            if (searchCriteria == null)
            {
                return BadRequest("Invalid search criteria");
            }

            var query = _context.lpProperty.AsQueryable();


            if (!string.IsNullOrEmpty(searchCriteria.Status))
            {
                query = query.Where(p => p.Status == searchCriteria.Status);
            }


            var searchResults = await query.ToListAsync();


            var saleProperties = searchResults
                .Where(p => p.Status == "Rent")
                .Select(property => new
                {
                    PropertyId = property.PropertyId,
                    PropertyTitle = property.PropertyTitle,
                    PropertyType = property.PropertyType,
                    Location = property.Location,
                    Price = property.Price,
                    Images = _context.LpImages
                        .Where(img => img.PropertyId == property.PropertyId)
                        .Select(img => new { ImageUrl = img.ImageUrl })
                        .ToList(),
                    ContactNumber = property.ContactNumber,
                    Status = property.Status

                })
                .ToList();

            return Ok(saleProperties);
        }



        [HttpPut]
        [Route("/property/approve/{id}")]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            try
            {
                var property = await _context.lpProperty.FirstOrDefaultAsync(w => w.PropertyId == id);

                if (property == null)
                {
                    return NotFound();
                }

                property.Approved = true;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/Property/{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {


            var product = await _context.lpProperty.FirstOrDefaultAsync(w => w.PropertyId == id);

            return Ok(product);

        }

        /// <summary>
        /// Add propertis api
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/property")]
        public async Task<IActionResult> AddProperty([FromBody] Property property)
        {

            if (property == null)
            {
                return NotFound();
            }
            _context.lpProperty.Add(property);
            await _context.SaveChangesAsync();


            return Ok(property);
        }



        [HttpDelete]
        [Route("/property/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            var property = await _context.lpProperty.FindAsync(propertyId);

            if (property == null)
            {
                return NotFound();
            }

            _context.lpProperty.Remove(property);
            await _context.SaveChangesAsync();



            return Ok(property);
        }

        [HttpPost("add/photo/{PropertyId}")]
        public async Task<IActionResult> UploadPropertyImage([FromForm] IFormFile image, int propertyId)
        {
            var result = await _photoService.UploadPhotoAsync(image);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var product = _context.lpProperty.Find(propertyId);
            if (product == null)
            {
                return BadRequest("No property record");
            }
            Console.WriteLine(product);




            var newImage = new Image

            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicID = result.PublicId,
                PropertyId = propertyId


            };



            _context.LpImages.Add(newImage);

            await _context.SaveChangesAsync();


            return StatusCode(201);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PropertyResponseDTO>>> GetPropertiesByUserId(int userId)
        {
            var properties = await _context.lpProperty
                .Include(p => p.Images)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (properties == null || !properties.Any())
            {
                return NotFound();
            }

            var propertyResponseDTOs = properties.Select(p => new PropertyResponseDTO
            {
                PropertyId = p.PropertyId,
                UserId = p.UserId,
                PropertyTitle = p.PropertyTitle,
                PropertyType = p.PropertyType,
                Location = p.Location,
                Price = p.Price,
                NoBedroom = p.NoBedroom,
                NoBathroom = p.NoBathroom,
                SquareFeet = p.SquareFeet,
                Description = p.Description,
                Images = p.Images.Select(i => new ImageDTO
                {
                    Id = i.Id,
                    PublicID = i.PublicID,
                    ImageUrl = i.ImageUrl,
                    PropertyId = i.PropertyId
                }).ToList(),
                ContactNumber = p.ContactNumber,
                Status = p.Status,
                DateListed = p.DateListed,
                DateUpdated = p.DateUpdated,
                FavoriteProperty = p.FavoriteProperty
            }).ToList();

            return Ok(propertyResponseDTOs);
        }

        [HttpPost]
        [Route("/login")]
        [AllowAnonymous]
        public IActionResult UserLogin([FromBody] User login)
        {

            IActionResult response = Unauthorized();

            User user = AuthenticateUser(login);
            if (user != null)
            {

                var tokenString = GenerateToken(user);
                response = Ok(new
                {
                    token = tokenString,
                    userDetails = user,

                });
            }
            else
            {

                response = Ok(new { message = "User does not exist" });
            }

            return response;




        }

        User AuthenticateUser(User loginCredentials)
        {

            User user = _context.LpUser.FirstOrDefault(x => x.Email == loginCredentials.Email && x.Password == loginCredentials.Password);
            return user;
        }


        string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email.ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Name),
                new Claim(ClaimTypes.NameIdentifier,user.Password),
                new Claim("role",user.UserType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        [HttpDelete]
        [Route("/delete/{userId}")]
        public ActionResult DeleteEmployee([FromRoute] int userId)
        {
            try
            {
                var user = _context.LpUser.FirstOrDefault(x => x.UserId == userId);

                if (user == null)
                {

                    return NotFound("User is not found");

                }


                var deleteuser = _context.LpUser.Remove(user);
                _context.SaveChangesAsync();


                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }


        [HttpPost]
        [Route("/contactapproval")]
        public async Task<IActionResult> ContactApproval([FromBody] ContactApproverDTO contactApproverDTO)
        {
            try
            {
                var convertedData = ModalConverter.ContactApproverDTOToContactApproval(contactApproverDTO);
                if (convertedData == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                       "Error Saving data");
                }

                _context.LpContactApproval.Add(convertedData);
                await _context.SaveChangesAsync();
                return Ok();
                //return Ok("Saved Successfully");
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                       "Error deleting data");
            }
        }
    }
}
