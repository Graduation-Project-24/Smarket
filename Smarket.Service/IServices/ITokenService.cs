using Smarket.Models;


namespace Smarket.Services.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}