
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<ApplicationUser>> GetAllVerifiedUsersAsync();
        Task<IReadOnlyList<ApplicationUser>> GetAllNonVerifiedUsersAsync();
        Task<IReadOnlyList<ApplicationUser>> GetBannedUsersAsync();
    }
}
