using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QFWork.Models.Interfaces;

namespace QFWork.Models.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<IdentityUser>> GetUsersByIdsAsync(IEnumerable<Guid> ids)
        {
            var stringIds = ids.Select(id => id.ToString());
            return await _context.Set<IdentityUser>()
                .Where(u => stringIds.Contains(u.Id))
                .ToListAsync();
        }
        public async Task<IdentityUser?> GetUserByIdAsync(Guid id)
        {
            return await _context.Set<IdentityUser>()
                .FirstOrDefaultAsync(u => Guid.Parse(u.Id) == id);
        }
    }
}
