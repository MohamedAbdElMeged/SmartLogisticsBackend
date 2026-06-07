using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHashed { get; set; }
    [MaxLength(length:40)]
    public required string FirstName { get; set; }
    [MaxLength(length:40)]
    public required string LastName { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public string? EmailVerificationTokenHash { get; set; }

    public DateTime? EmailVerificationExpiresAt { get; set; }
    public string FullName => FirstName + " " + LastName;
    
    private User() { } 
    
    public static User Create(string firstName, string lastName, string email, string password)
    {

        var user =  new User()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLower(),
            PasswordHashed = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            EmailVerified = false,
            // EmailVerificationTokenHash = tokenHash,
            // EmailVerificationExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return user;
    }
    
    public Result VerifyEmail(string rawToken)
    {
        if (EmailVerified)
            return Result.Failure("Email is already verified.");

        if (EmailVerificationExpiresAt < DateTime.UtcNow)
            return Result.Failure("Verification token has expired.");

        var incomingHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        if (incomingHash != EmailVerificationTokenHash)
            return Result.Failure("Invalid verification token.");

        EmailVerified                    = true;
        EmailVerifiedAt                  = DateTime.UtcNow;
        EmailVerificationTokenHash       = null;
        EmailVerificationExpiresAt  = null;

        return Result.Success();
    }
    
    public string GenerateVerificationToken()
    {
        var rawToken  = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        EmailVerificationTokenHash      = tokenHash;
        EmailVerificationExpiresAt      = DateTime.UtcNow.AddHours(24);

        return rawToken;
    }
}