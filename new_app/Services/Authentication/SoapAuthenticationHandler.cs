using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using new_app.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace new_app.Services.Authentication;

public class SoapAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _context;

    public SoapAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ApplicationDbContext context)
        : base(options, logger, encoder, clock)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return AuthenticateResult.NoResult();

        // Handle Basic Authentication or API Key-based authentication
        if (authHeader.ToString().StartsWith("Basic "))
        {
            var token = authHeader.ToString().Substring("Basic ".Length).Trim();
            
            try
            {
                // Decode the Base64 string
                var decodedBytes = Convert.FromBase64String(token);
                var credentials = Encoding.UTF8.GetString(decodedBytes).Split(':');

                if (credentials.Length == 2)
                {
                    var username = credentials[0];
                    var password = credentials[1];

                    // Find user by username and verify password
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName == username);

                    if (user != null && VerifyPassword(user, password))
                    {
                        var claims = new[] { 
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Role, "SoapUser")
                        };

                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
            catch
            {
                // Fallback to API key approach if Base64 decoding fails
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ApiKey == token);

                if (user != null)
                {
                    var claims = new[] { 
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }
        }
        // Handle SOAP-specific WS-Security token if present
        else if (Request.ContentType?.Contains("soap+xml") == true || 
                 Request.ContentType?.Contains("xml") == true)
        {
            // Extract security token from SOAP envelope if present
            // This would require parsing the XML body in a real implementation
            var securityToken = ExtractSecurityTokenFromSoapEnvelope();
            
            if (!string.IsNullOrEmpty(securityToken))
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ApiKey == securityToken);

                if (user != null)
                {
                    var claims = new[] { 
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim("AuthMethod", "WS-Security")
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }
        }

        return AuthenticateResult.Fail("Invalid credentials");
    }

    private bool VerifyPassword(object user, string password)
    {
        // Implement proper password verification here
        // This is a placeholder - in a real app you would verify against a hashed password
        
        // For demo purposes only:
        // Assuming User entity has a PasswordHash property
        var propertyInfo = user.GetType().GetProperty("PasswordHash");
        if (propertyInfo != null)
        {
            var storedHash = propertyInfo.GetValue(user)?.ToString();
            // You would use a proper hash verification here
            return !string.IsNullOrEmpty(storedHash);
        }
        
        return false;
    }

    private string ExtractSecurityTokenFromSoapEnvelope()
    {
        // In a real implementation, this would parse the SOAP envelope
        // and extract security tokens according to WS-Security standards
        
        // Placeholder implementation - in reality you would:
        // 1. Read the request body
        // 2. Parse the XML
        // 3. Extract relevant security tokens from appropriate namespaces
        
        return null;
    }
}