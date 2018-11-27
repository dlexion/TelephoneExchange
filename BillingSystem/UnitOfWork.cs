using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem
{
    public class UnitOfWork
    {
        public IRepository<Call> Calls { get; }

        public IRepository<Client> Clients { get; }

        public IRepository<string> PhoneNumbers { get; }

        public IRepository<Phone> Phones { get; }
    }
}