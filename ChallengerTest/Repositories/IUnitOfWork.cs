using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;
using System.Threading.Tasks;

namespace ChallengerTest.Repositories
{
    public interface IUnitOfWork
    {
        IPermissionQueryRepository PermissionQueries { get; }
        IPermissionCommandRepository PermissionCommands { get; }
        Task SaveAsync();
    }
}