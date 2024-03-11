﻿using Shop.Models;
using Shop.Pages;
using System.Security.Claims;

namespace Shop.Services
{
    public class ProfileService
    {
        private static readonly string _claimsBaseUrl = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
        private static readonly string _objectIdentifier = _claimsBaseUrl + "objectidentifier";
        private static readonly string _name = _claimsBaseUrl + "name";
        private static readonly string _emailAddress = _claimsBaseUrl + "emailaddress";

        private readonly ILogger<ProfileService> _logger;

        public ProfileService(ILogger<ProfileService> logger)
        {
            _logger = logger;
        }

        private static string FindClaimValue(ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirst(claimType)?.Value ?? string.Empty;
        }

        public Profile? ReadProfile(HttpRequest request)
        {
            var principal = ClaimsPrincipalParser.Parse(request);
            Profile? profile = null;

            if (principal is not null)
            {
                _logger.LogInformation("Claims: {Claims}", string.Join(",", principal.Claims));

                profile = new Profile
                {
                    Id = FindClaimValue(principal, _objectIdentifier),
                    Name = FindClaimValue(principal, _name),
                    EmailAddress = FindClaimValue(principal, _emailAddress)
                };
            }

            return profile;
        }
    }
}
