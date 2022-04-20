using CurrencyConverter.Service.DTOs;
using System.Threading.Tasks;

namespace CurrencyConverter.Service.Interfaces
{
    public interface IAuthService
    {
        Task<UserToReturnDto> LoginAsync(LoginDto loginDto);
        Task<UserToReturnDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> CheckEmailExistsAsync(string email);
    }
}
