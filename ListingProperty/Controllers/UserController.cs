using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ListingProperty.Data;
using ListingProperty.Enums;
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

namespace ListingProperty.Controllers
{
    public class UserController : Controller
    {
        private readonly AppContextDb _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoService _photoService;

        public UserController(
                AppContextDb context,
                IConfiguration config,
                IWebHostEnvironment webHostEnvironment,
                IPhotoService photoService
                )
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
        [Route("/userDetails/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var userDetail = await _context.LpUser.Where(w => w.UserId == userId).Select(u => new
            {
                u.UserId,
                u.Name,
                u.Email,
                u.UserType,
                u.Phone
            }).FirstOrDefaultAsync();

            return Ok(userDetail);
        }

        [HttpPost]
        [Route("/AddUser")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                var existingUser = _context.LpUser.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                // Proceed to add the user if it doesn't exist
                var newUser = new User
                {
                    UserType = user.UserType,
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    Phone = user.Phone
                };

                _context.LpUser.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet]
        [Route("/getAllproperty")]
        public async Task<IActionResult> GetAllProperty()
        {
            /*  var property = await _context.lpProperty.ToListAsync();
			    var image = await _context.LpImages.ToListAsync();*/
            var combinedData = await (
                    from property in _context.lpProperty
                    join image in _context.LpImages
                    on property.PropertyId equals image.PropertyId
                    into propertyImages
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
                    }
            ).ToListAsync();

            return Ok(combinedData);
        }

        [HttpPost]
        [Route("/searchProperty")]
        public async Task<IActionResult> SearchProperty(
                [FromBody] SearchPropertyViewModel searchCriteria
                )
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

            var imageUrls = await _context
                .LpImages.Where(img => propertyIds.Contains(img.PropertyId))
                .Select(img => new { img.PropertyId, img.ImageUrl })
                .ToListAsync();

            var combinedData = searchResults
                .Select(property => new
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
                        .Select(img => new { ImageUrl = img.ImageUrl })
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

        [HttpPut]
        [Route("/property/approve/{id}")]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            try
            {
                var property = await _context.lpProperty.FirstOrDefaultAsync(w =>
                        w.PropertyId == id
                        );

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
        [Route("/Property/{propertyId}")]
        public async Task<IActionResult> GetPropertyById(int propertyId)
        {
            var product = await _context.lpProperty.FirstOrDefaultAsync(w =>
                w.PropertyId == propertyId
            );
            var response = new GetPropertiesRS()
            {
                PropertyId = product.PropertyId,
                UserId = product.UserId,
                PropertyTitle = product.PropertyTitle,
                Price = product.Price,
                PropertyType = product.PropertyType,
                FavoriteProperty = product.FavoriteProperty,
                Location = product.Location,
                NoBedroom = product.NoBedroom,
                NoBathroom = product.NoBathroom,
                SquareFeet = product.SquareFeet,
                Description = product.Description
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("/Property/{userId}/{propertyId}")]
        public async Task<IActionResult> GetPropertyByUserId(int userId, int propertyId)
        {
            var product = await _context.lpProperty.FirstOrDefaultAsync(w => w.PropertyId == propertyId);
            var propertyStatus = await _context.LpContactApproval.FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propertyId);
            var response = new GetPropertiesRS()
            {
                PropertyId = product.PropertyId,
                UserId = product.UserId,
                PropertyTitle = product.PropertyTitle,
                Price = product.Price,
                PropertyType = product.PropertyType,
                FavoriteProperty = product.FavoriteProperty,
                Location = product.Location,
                NoBedroom = product.NoBedroom,
                NoBathroom = product.NoBathroom,
                SquareFeet = product.SquareFeet,
                Description = product.Description,
                Status = product.Status
            };
            // case to check is there a entry in table related to propertyId and userId, if not then assign new as approval status
            if (propertyStatus == null)
            {
                response.ApprovalStatus = Enums.ApprovalStatus.New;
            }

            else if (propertyStatus.ApprovalStatus == Enums.ApprovalStatus.Approved)
            {
                response.ContactNumber = product.ContactNumber;
                response.ApprovalStatus = propertyStatus.ApprovalStatus;
                //
                //some more data
            }

            //If there is a entry in table then copy status of approval record into our response so it can be used on frontend to show and hide button and contact details
            else
            {
                response.ApprovalStatus = propertyStatus.ApprovalStatus;
            }
            return Ok(response);
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
        public async Task<IActionResult> UploadPropertyImage(
                [FromForm] IFormFile image,
                int propertyId
                )
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
        public async Task<ActionResult<IEnumerable<PropertyResponseDTO>>> GetPropertiesByUserId(
                int userId
                )
        {
            var properties = await _context
                .lpProperty.Include(p => p.Images)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (properties == null || !properties.Any())
            {
                return NotFound();
            }

            var propertyResponseDTOs = properties
                .Select(p => new PropertyResponseDTO
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
                    Images = p
                        .Images.Select(i => new ImageDTO
                        {
                            Id = i.Id,
                            PublicID = i.PublicID,
                            ImageUrl = i.ImageUrl,
                            PropertyId = i.PropertyId
                        })
                        .ToList(),
                    ContactNumber = p.ContactNumber,
                    Status = p.Status,
                    DateListed = p.DateListed,
                    DateUpdated = p.DateUpdated,
                    FavoriteProperty = p.FavoriteProperty
                })
            .ToList();

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
                response = Ok(new { token = tokenString, userDetails = user, });
            }
            else
            {

                response = StatusCode(StatusCodes.Status404NotFound,
                        "User not exist");
            }

            return response;
        }
        User AuthenticateUser(User loginCredentials)
        {
            User user = _context.LpUser.FirstOrDefault(x =>
                    x.Email == loginCredentials.Email && x.Password == loginCredentials.Password
                    );
            return user;
        }

        string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Password),
                    new Claim("role", user.UserType),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials
                    );

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
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

        [HttpPost]
        [Route("/contactapproval")]
        public async Task<IActionResult> ContactApproval([FromBody] ContactApproverDTO contactApproverDTO)
        {
            try
            {
                var convertedData = ModalConverter.ContactApproverDTOToContactApproval(
                        contactApproverDTO
                        );
                if (convertedData == null)
                {
                    return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            "Error Saving data"
                            );
                }

                _context.LpContactApproval.Add(convertedData);
                await _context.SaveChangesAsync();
                return Ok(convertedData);
                //return Ok("Saved Successfully");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }


        [HttpGet]
        [Route("/getlistofapprovalrequest")]
        public async Task<IActionResult> GetListOfApprovalRequest()
        {
            var result = await _context.LpContactApproval
                .Include(a => a.User)
                .Include(a => a.Property)
                .Select(request => new
                {
                    UserId = request.UserId,
                    Username = request.User.Name,
                    PropertyId = request.PropertyId,
                    PropertyTitle = request.Property.PropertyTitle,
                    ApprovalStatus = request.ApprovalStatus,
                    CreatedOn = request.CreatedOn,
                    UpdatedOn = request.UpdatedOn
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("adminaction")]
        public ActionResult AdminApproval([FromBody] AdminRequestDTO adminRequestDTO)
        {
            try
            {
                var adminResp = _context.LpContactApproval.FirstOrDefault(x => x.UserId == adminRequestDTO.UserId && x.PropertyId == adminRequestDTO.PropertyId);
                adminResp.ApprovalStatus = adminRequestDTO.ApprovalStatus;
                _context.SaveChanges();
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false);
                //throw;
            }
        }


        [HttpPost]
        [Route("/sendOffer")]
        public async Task<IActionResult> SendOffer([FromBody] OfferModal offer)
        {
            if (offer == null)
            {
                return BadRequest("Offer data is missing.");
            }

            // Check if the property and buyer exist
            var property = await _context.lpProperty.FindAsync(offer.PropertyId);
            var buyer = await _context.LpUser.FindAsync(offer.BuyerId);

            if (property == null || buyer == null)
            {
                return NotFound("Property or buyer not found.");
            }

            // Create the offer entity
            var newOffer = new OfferModal
            {
                OfferPrice = offer.OfferPrice,
                OfferText = offer.OfferText,
                OfferLastDate = offer.OfferLastDate,
                SellerStatus = Enums.ApprovalStatus.PendingApproval,
                AdminStatus = Enums.ApprovalStatus.PendingApproval,
                OfferStatus = Enums.PaymentStatus.Pending,
                PropertyId = offer.PropertyId,
                SellerId = offer.SellerId,
                BuyerId = offer.BuyerId
            };

            // Add the offer to the context
            _context.LpPropertyOffers.Add(newOffer);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("Offer sent successfully.");
        }

        [HttpGet]
        [Route("/getOfferById")]
        public async Task<IActionResult> GetOfferById(int propertyId, int userId)
        {
            // Check if an offer exists for the specified property and user
            var existingOffer = await _context.LpPropertyOffers
                .FirstOrDefaultAsync(o => o.PropertyId == propertyId && o.BuyerId == userId);

            if (existingOffer != null)
            {
                return Ok(existingOffer);
            }
            else
            {
                return Ok("No offer exists for this property and user.");
            }
        }

        [HttpGet]
        [Route("/getOffersById")]
        public async Task<IActionResult> GetOffersById(int UserId, string UserType)
        {
            List<OfferModal> offers;

            if (UserType == "Seller")
            {
                offers = await _context.LpPropertyOffers.Where(o => o.SellerId == UserId).ToListAsync();
            }
            else if (UserType == "Buyer") // so that the buyer can see his own offers
            {
                offers = await _context.LpPropertyOffers.Where(o => o.BuyerId == UserId).ToListAsync();
            }
            else if (UserType == "Admin")
            {
                offers = await _context.LpPropertyOffers.ToListAsync();
            }
            else
            {
                return BadRequest("Invalid user type."); // Handle invalid user type
            }

            if (offers.Any()) // condition for checking if offers exist
            {
                return Ok(offers);
            }
            else
            {
                return NotFound("No offers found.");
            }
        }

        [HttpPost]
        [Route("/OfferAction")]
        public ActionResult OfferAction([FromBody] OfferModal sellerOfferDTO, string userType)
        {
            try
            {
                var sellerResp = _context.LpPropertyOffers.FirstOrDefault(x => x.SellerId == sellerOfferDTO.SellerId && x.PropertyId == sellerOfferDTO.PropertyId && x.BuyerId == sellerOfferDTO.BuyerId && x.OfferId == sellerOfferDTO.OfferId);

                if (sellerResp == null) return BadRequest("Offer Not Found");

                if (userType == "Seller")
                {
                    sellerResp.SellerStatus = sellerOfferDTO.SellerStatus;
                }
                else
                {
                    sellerResp.AdminStatus = sellerOfferDTO.AdminStatus;
                }

                _context.SaveChanges();
                return Ok(true);
            }
            catch (Exception)
            {
                return Ok(false);
                //throw;
            }
        }

        [HttpPost("Retry")]
        public async Task<IActionResult> RetryOffer([FromBody] OfferModal updatedOffer)
        {
            try
            {
                var existingOffer = await _context.LpPropertyOffers.FirstOrDefaultAsync(o => o.OfferId == updatedOffer.OfferId);

                if (existingOffer == null)
                    return NotFound("Offer not found");

                // Update existing offer with new data
                existingOffer.OfferPrice = updatedOffer.OfferPrice;
                existingOffer.OfferText = updatedOffer.OfferText;
                existingOffer.OfferLastDate = updatedOffer.OfferLastDate;

                // Set all statuses to pending
                existingOffer.SellerStatus = Enums.ApprovalStatus.PendingApproval;
                existingOffer.AdminStatus = Enums.ApprovalStatus.PendingApproval;
                existingOffer.OfferStatus = Enums.PaymentStatus.Pending;

                // Save changes to database
                await _context.SaveChangesAsync();

                return Ok("Offer retried successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("/BuyProperty")]
        public async Task<IActionResult> BuyProperty([FromBody] OfferModal offer)
        {
            if (offer == null)
            {
                return BadRequest("Invalid offer");
            }

            // Find the offer in the database
            var existingOffer = await _context.LpPropertyOffers.FirstOrDefaultAsync(o => o.OfferId == offer.OfferId);

            if (existingOffer == null)
            {
                return BadRequest("Offer not found");
            }

            // Check if the offer status is correct for payment
            if (existingOffer.AdminStatus != ApprovalStatus.Approved || existingOffer.SellerStatus != ApprovalStatus.Approved)
            {
                return BadRequest("Offer is not approved for payment");
            }

            // Find the property associated with the offer
            var property = await _context.lpProperty.FirstOrDefaultAsync(p => p.PropertyId == existingOffer.PropertyId);

            if (property == null)
            {
                return BadRequest("Property not found");
            }

            // Check if the property status is Sale
            if (property.Status != "Sale")
            {
                return BadRequest("Property is not available for sale");
            }

            // Check if the property has already been sold
            if (property.Status == "Sold")
            {
                return BadRequest("Property has already been sold");
            }

            // Update the property status to Sold
            property.Status = "Sold";

            // Update the offer status to Complete
            existingOffer.OfferStatus = PaymentStatus.Complete;

            // Add payment details to the LpPayments table
            var payment = new Payments
            {
                PropertyId = property.PropertyId,
                SellerId = existingOffer.SellerId,
                BuyerId = existingOffer.BuyerId,
                Price = existingOffer.OfferPrice,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.Complete,
                PaymentType = PaymentType.Buy
            };

            _context.LpPayments.Add(payment);

            // Add an entry to the LpOwnedProperties table
            var ownedProperty = new OwnedProperties
            {
                Property = property,
                Buyer = await _context.LpUser.FirstOrDefaultAsync(u => u.UserId == existingOffer.BuyerId),
                Offer = existingOffer,
                Transaction = payment
            };

            _context.LpOwnedProperties.Add(ownedProperty);

            try
            {
                await _context.SaveChangesAsync();

                // Send a confirmation message
                string confirmationMessage = "Property purchased successfully.";
                return Ok(confirmationMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("/RentProperty")]
        public async Task<IActionResult> RentProperty([FromBody] OfferModal offer)
        {
            if (offer == null)
            {
                return BadRequest("Invalid offer");
            }

            // Find the offer in the database
            var existingOffer = await _context.LpPropertyOffers.FirstOrDefaultAsync(o => o.OfferId == offer.OfferId);

            if (existingOffer == null)
            {
                return BadRequest("Offer not found");
            }

            // Check if the offer status is correct for payment
            if (existingOffer.AdminStatus != ApprovalStatus.Approved || existingOffer.SellerStatus != ApprovalStatus.Approved)
            {
                return BadRequest("Offer is not approved for payment");
            }

            // Find the property associated with the offer
            var property = await _context.lpProperty.FirstOrDefaultAsync(p => p.PropertyId == existingOffer.PropertyId);

            if (property == null)
            {
                return BadRequest("Property not found");
            }

            // Check if the property status is Rent
            if (property.Status != "Rent")
            {
                return BadRequest("Property is not available for rent");
            }

            // Check if the property has already been rented
            if (property.Status == "Rented")
            {
                return BadRequest("Property has already been rented");
            }

            // Update the property status to Rented
            property.Status = "Rented";

            // Update the offer status to Complete
            existingOffer.OfferStatus = PaymentStatus.Complete;

            // Add payment details to the LpPayments table
            var payment = new Payments
            {
                PropertyId = property.PropertyId,
                SellerId = existingOffer.SellerId,
                BuyerId = existingOffer.BuyerId,
                Price = existingOffer.OfferPrice,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.Complete,
                PaymentType = PaymentType.Rent
            };

            _context.LpPayments.Add(payment);

            // Add an entry to the LpOwnedProperties table
            var ownedProperty = new OwnedProperties
            {
                Property = property,
                Buyer = await _context.LpUser.FirstOrDefaultAsync(u => u.UserId == existingOffer.BuyerId),
                Offer = existingOffer,
                Transaction = payment
            };

            _context.LpOwnedProperties.Add(ownedProperty);

            try
            {
                await _context.SaveChangesAsync();

                // Send a confirmation message
                string confirmationMessage = "Property rented successfully.";
                return Ok(confirmationMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpGet("/getOwnedProperties/{buyerId}")]
        public async Task<IActionResult> GetOwnedProperties(int buyerId)
        {
            try
            {
                // Query the LpOwnedProperties table to retrieve owned properties for the specified buyer
                var ownedProperties = await _context.LpOwnedProperties
                    .Where(op => op.Buyer.UserId == buyerId)
                    .Select(op => new
                    {
                        PropertyId = op.Property.PropertyId,
                        PropertyTitle = op.Property.PropertyTitle,
                        PropertyType = op.Property.PropertyType,
                        Location = op.Property.Location,
                        Status = op.Property.Status,
                        Price = op.Property.Price,
                        OfferPrice = op.Offer.OfferPrice,
                        PaymentDate = op.Transaction.PaymentDate,
                        PaymentStatus = op.Transaction.PaymentStatus
                    })
                    .ToListAsync();

                return Ok(ownedProperties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpGet("/getAllPayments")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _context.LpPayments.ToListAsync();
                return Ok(payments);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }
    }
}
