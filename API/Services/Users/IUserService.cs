using API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Users
{
    public interface IUserService
    {
        Task<LoggedDTO?> LoginAsync(string username, string password, CancellationToken ct);
    }
}
