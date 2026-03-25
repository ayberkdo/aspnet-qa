using aspnet_qa.API.DTOs;
using aspnet_qa.API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aspnet_qa.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]

        public async Task<List<UserDto>> List()
        {
            var users = _userManager.Users.ToList();
            //var userDtos = _mapper.Map<List<UserDto>>(users);
            var userDtos = new List<UserDto>();
            foreach (var user in users) {
                var userDto = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault() ?? "User";
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return _mapper.Map<UserDto>(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultDto> Add(RegisterDto dto)
        {
            var result = new ResultDto();
            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                PhotoUrl = "/Files/UserPhotos/default-profile-photo.jpg" // Varsayılan fotoğraf
            };

            var identityResult = await _userManager.CreateAsync(user, dto.Password ?? "");

            if (!identityResult.Succeeded)
            {
                result.Status = false;
                result.Message = string.Join(" ", identityResult.Errors.Select(e => e.Description));
                return result;
            }

            // Rol Kontrolü
            if (!await _roleManager.RoleExistsAsync("Uye"))
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Uye" });
            }

            await _userManager.AddToRoleAsync(user, "Uye");

            result.Status = true;
            result.Message = "Üye başarıyla eklendi.";
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultDto> SignIn(LoginDto dto)
        {
            var result = new ResultDto();
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                result.Status = false;
                result.Message = "Kullanıcı adı veya parola geçersiz!";
                return result;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("UserPhoto", user.PhotoUrl ?? "/Files/UserPhotos/default-profile-photo.jpg"),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            result.Status = true;
            result.Message = GenerateJWT(authClaims);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultDto> SignUp(RegisterDto dto)
        {
            var result = new ResultDto();

            // 1. Yeni kullanıcı nesnesini oluştur
            var user = new AppUser // AppUser senin IdentityUser'dan türettiğin sınıfın adı olmalı
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                // Kayıt olurken varsayılan bir fotoğraf atayalım (SignIn'de beklediğin gibi)
                PhotoUrl = "default-profile-photo.jpg"
            };

            // 2. Kullanıcıyı şifresiyle birlikte oluştur (Identity şifreyi otomatik hash'ler)
            var identityResult = await _userManager.CreateAsync(user, dto.Password!);

            if (!identityResult.Succeeded)
            {
                // Eğer hata varsa (şifre çok kısa, email geçersiz vs.) hataları birleştirip dönelim
                result.Status = false;
                result.Message = string.Join(" | ", identityResult.Errors.Select(e => e.Description));
                return result;
            }

            // 3. Kullanıcıya varsayılan bir rol ata (Örn: "User")
            // Not: Bu rolün veritabanında (AspNetRoles) önceden ekli olması gerekir.
            await _userManager.AddToRoleAsync(user, "User");

            result.Status = true;
            result.Message = "Kullanıcı kaydı başarıyla tamamlandı.";
            return result;
        }

        private string GenerateJWT(List<Claim> claims)
        {
            // appsettings.json'dan verileri çekiyoruz
            var expirationInMinutes = Convert.ToDouble(_configuration["Jwt:AccessTokenExpiration"] ?? "60");
            var key = _configuration["Jwt:Key"] ?? "StandartGizliAnahtarinizEnAz16Karakter";

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(expirationInMinutes),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenObject);
        }

        [HttpPost]
        public async Task<ResultDto> Upload(UploadDto dto)
        {
            var result = new ResultDto();
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                result.Status = false;
                result.Message = "Kayıt bulunamadı!";
                return result;
            }

            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Files", "UserPhotos");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // Eski dosyayı sil (varsayılan değilse)
            if (user.PhotoUrl != "default-profile-photo.jpg" && !string.IsNullOrEmpty(user.PhotoUrl))
            {
                var oldPath = Path.Combine(folderPath, user.PhotoUrl);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            // Yeni dosyayı kaydet
            string base64Data = dto.PicData.Contains(",") ? dto.PicData.Split(',')[1] : dto.PicData;
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            string fileName = $"{Guid.NewGuid()}{dto.PicExt}";
            string fullPath = Path.Combine(folderPath, fileName);

            await System.IO.File.WriteAllBytesAsync(fullPath, imageBytes);

            user.PhotoUrl = fileName;
            await _userManager.UpdateAsync(user);

            result.Status = true;
            result.Message = "Profil fotoğrafı güncellendi.";
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Geçersiz oturum." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var userProfile = new
            {
                Id = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                ProfileImageUrl = user.PhotoUrl
            };

            return Ok(userProfile);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            // 2026 Senior Notu: İleride buraya 'Token'ı kara listeye al' 
            // veya 'Kullanıcının Refresh Token'ını sil' gibi mantıklar gelecek.
            return Ok(new { status = true, message = "Başarıyla çıkış yapıldı." });
        }

        [HttpGet("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { message = "Kullanıcı adı zorunludur." });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                ProfileImageUrl = user.PhotoUrl,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
