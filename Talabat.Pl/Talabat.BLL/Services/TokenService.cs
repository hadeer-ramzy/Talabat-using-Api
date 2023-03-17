﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Identity;

namespace Talabat.BLL.Services
{
    public class TokenService : ITokenServices
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public  async Task<string> CreateToken(ApplicationUser applicationUser,UserManager<ApplicationUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,applicationUser.Email),
                new Claim(ClaimTypes.GivenName,applicationUser.DisplayName)
            };  //private claim
            var userRoles = await userManager.GetRolesAsync(applicationUser);
            foreach(var role in userRoles) { authClaims.Add(new Claim(ClaimTypes.Role,role)); }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var token = new JwtSecurityToken(
                  issuer: configuration["JWT:ValidIssuer"],
                  audience: configuration["JWT:ValidAudiance"],
                  expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                  claims: authClaims,
                  signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return  new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}