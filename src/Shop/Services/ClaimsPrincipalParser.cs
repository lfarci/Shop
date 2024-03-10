﻿using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Shop.Services;

public static class ClaimsPrincipalParser
{
    private class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")]
        public string? Type { get; set; }
        [JsonPropertyName("val")]
        public string? Value { get; set; }
    }

    private class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")]
        public string? IdentityProvider { get; set; }
        [JsonPropertyName("name_typ")]
        public string? NameClaimType { get; set; }
        [JsonPropertyName("role_typ")]
        public string? RoleClaimType { get; set; }
        [JsonPropertyName("claims")]
        public IEnumerable<ClientPrincipalClaim>? Claims { get; set; }
    }

    public static ClaimsPrincipal? Parse(HttpRequest req)
    {
        var principal = new ClientPrincipal();

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];

            if (data is not null)
            {
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }


        }

        if (principal is not null)
        {
            var identity = new ClaimsIdentity(principal.IdentityProvider, principal.NameClaimType, principal.RoleClaimType);
            var claims = principal.Claims?.Select(c => new Claim(c.Type ?? string.Empty, c.Value ?? string.Empty));

            if (claims is not null)
            { 
                identity.AddClaims(claims);
            }

            return new ClaimsPrincipal(identity);
        }

        return null;
    }
}
