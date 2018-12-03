using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem
{
    public class BillingUnitOfWork
    {
        public IGenericRepository<Call> Calls { get; }

        public IGenericRepository<Client> Clients { get; }

        public IGenericRepository<string> PhoneNumbers { get; }

        public IGenericRepository<PortModel> Ports { get; }
    }
}