using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem
{
    public class BillingUnitOfWork
    {
        public BillingUnitOfWork()
        {
            Calls = new GenericRepository<Call>();
            Clients = new GenericRepository<Client>();
            PhoneNumbers=new GenericRepository<string>();
            Ports= new GenericRepository<PortModel>();
        }

        public IGenericRepository<Call> Calls { get; }

        public IGenericRepository<Client> Clients { get; }

        public IGenericRepository<string> PhoneNumbers { get; }

        public IGenericRepository<PortModel> Ports { get; }
    }
}