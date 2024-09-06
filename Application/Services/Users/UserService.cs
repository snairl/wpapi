using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using Domain.Users;

namespace Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        public async Task<UserDTO?> LoginAsync(string username, string password, CancellationToken ct)
        {
            var repository = unitOfWork.Repository<User>();

            var user = await repository.GetAsync(u => u.Username == username, ct);

            if (user == null || !user.VerifyPassword(password))
            {
                return null;
            }

            return mapper.Map<UserDTO>(user);
        }
    }
}
