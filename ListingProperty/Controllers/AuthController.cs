﻿using ListingProperty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingProperty.Controllers
{
    public class AuthController : Controller
    {

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = Policies.User)]
        public IActionResult GetUserData()
        {
            return Ok("This is an normal user");
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminData()
        {
            return Ok("This is an Admin user");
        }

        [HttpGet]
        [Route("GetBuyerData")]
        [Authorize(Policy = Policies.Buyer)]
        public IActionResult GetBuyerData()
        {
            return Ok("This is an Buyer user");
        }
        [HttpGet]
        [Route("GetSellerData")]
        [Authorize(Policy = Policies.Seller)]
        public IActionResult GetSellerData()
        {
            return Ok("This is an Seller user");
        }
    }
}
