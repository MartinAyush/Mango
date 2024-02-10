using Mango.Services.AuthApi.Data;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenRepository tokenRepository)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._tokenRepository = tokenRepository;
        }

        public async Task<bool> AssignRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);

                return addToRoleResult.Succeeded;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto requestDto)
        {
            var user = await _userManager.FindByEmailAsync(requestDto.UserName);

            if(user != null)
            {
                bool success = await _userManager.CheckPasswordAsync(user, requestDto.Password);

                if (success)
                {
                    // Generate Token
                    var roles = await _userManager.GetRolesAsync(user);
                    var jwtToken = _tokenRepository.GenerateToken(user, roles);

                    var userdto = new UserDto()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        Name = user.FirstName + " " + user.LastName,
                        PhoneNumber = user.PhoneNumber
                    };

                    return  new()
                    {
                        User = userdto,
                        Token = jwtToken,
                    };
                }
            }

            return new()
            {
                User = null,
                Token = ""
            };
        }

        public async Task<string> Register(RegistrationRequestDto requestDto)
        {
            var user = new ApplicationUser()
            {
                UserName = requestDto.Email,
                Email = requestDto.Email,
                NormalizedEmail = requestDto.Email.ToUpper(),
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                PhoneNumber = requestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, requestDto.Password);
                if (result.Succeeded)
                {
                    // get the user from db
                    var dbUser = _dbContext.ApplicationUsers.First(u => u.UserName == requestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = dbUser.Email,
                        Id = dbUser.Id,
                        Name = dbUser.FirstName + " " + dbUser.LastName,
                        PhoneNumber = dbUser.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {   
            }
            return "Error Encountered";
        }
    }
}
