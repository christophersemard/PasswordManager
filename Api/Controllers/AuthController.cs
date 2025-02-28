using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Api.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // // Méthode pour s'inscrire
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var alreadyUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (alreadyUser != null)
            {
                return BadRequest(new { message = "Un utilisateur avec cet email existe déjà." });
            }

            // Générer un sel sécurisé
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hacher le mot de passe avec PBKDF2
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            // Sauvegarder l'utilisateur
            var user = new User
            {
                Email = model.Email,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt) // Stocke le sel pour la vérification au login
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Utilisateur créé avec succès !" });
            
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Rechercher l'utilisateur par email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });
            }

            // Récupérer le sel et hasher le mot de passe fourni
            byte[] salt = Convert.FromBase64String(user.Salt);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            // Vérifier si les mots de passe correspondent
            if (hashedPassword != user.Password)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });
            }

            // Générer un token JWT
            var token = GenerateJwtToken(user);

            return Ok(new { token, salt = user.Salt });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Expiration en 24h
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize] // 👈 Nécessite un JWT valide
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            return Ok(new { Id = userId, Email = userEmail });
        }
        
       
    }
    
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}