﻿using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Users
{
    public interface IUserService
    {
        Task<UserDTO?> LoginAsync(string username, string password, CancellationToken ct);
    }
}
