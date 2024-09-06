using Application.DTOs;
using Application.Services.Tokens;
using Domain.Interfaces;
using Domain.Users;

namespace Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenService tokenService;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
        }


        public async Task<LoggedDTO?> LoginAsync(string username, string password, CancellationToken ct)
        {
            var repository = unitOfWork.Repository<User>();

            var user = await repository.GetAsync(u => u.Username == username, ct);

            if (user != null && user.VerifyPassword(password))
            {
                return new LoggedDTO
                {
                    Username = user.Username,
                    Token = tokenService.GenerateTokenAsync(user.Username)
                };
            }

            return null;
        }
    }
}
