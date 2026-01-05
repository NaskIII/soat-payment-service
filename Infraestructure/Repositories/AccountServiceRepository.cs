using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infraestructure.BaseRepository;
using Infraestructure.DatabaseContext;

namespace Infraestructure.Repositories
{
    public class AccountServiceRepository : Repository<ServiceAccounts>, IAccountServiceRepository
    {
        public AccountServiceRepository(ApplicationDatabaseContext context) : base(context)
        {
        }
    }
}
