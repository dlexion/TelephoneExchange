using BillingSystem.Repositories;

namespace BillingSystem
{
    public class UnitOfWork
    {
        public IRepository<Call> Calls { get; }

        public IRepository<Client> Clients { get; }
    }
}