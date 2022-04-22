using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CurrencyConverter.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager,
         SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<bool> CheckEmailExistsAsync(string email) =>
             await _userManager.FindByEmailAsync(email) != null;
        

        public async Task<UserToReturnDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return null;

            return new UserToReturnDto { Email = user.Email };
        }

        public async Task<UserToReturnDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            return !result.Succeeded ? null : new UserToReturnDto { Email = user.Email };
        }
    }
}
