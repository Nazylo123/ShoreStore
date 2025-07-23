using ShoeStore.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeStore.Client.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();
    }
} 