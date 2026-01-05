using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infraestructure.BaseRepository;
using Infraestructure.DatabaseContext;

namespace Infraestructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDatabaseContext context) : base(context)
        {
        }
    }
}
