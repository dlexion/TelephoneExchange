namespace BillingSystem.Models
{
    public class PortModel
    {
        public string PhoneNumber { get; set; }

        public decimal Balance { get; set; } = 0;

        public Tariff Tariff { get; set; }

        public PortModel(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}