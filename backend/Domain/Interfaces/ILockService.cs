using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILockService
    {
        Task LockAsync(string key, TimeSpan expiry);
        Task<bool> ReleaseLockAsync(string key);
        Task<bool> IsLockedAsync(string key);
    }
}
