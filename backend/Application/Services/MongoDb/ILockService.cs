using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.MongoDb
{
    public interface ILockService
    {
        Task LockAsync(string key);
        Task<bool> ReleaseLockAsync(string key);
        Task<bool> IsLockedAsync(string key);
        Task WaitForLockToBeReleased(string key);
    }
}
