using Microsoft.AspNetCore.Identity;

namespace QFWork.Models.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetUsersByIdsAsync(IEnumerable<Guid> ids);
        Task<IdentityUser?> GetUserByIdAsync(Guid id);
    }
}
