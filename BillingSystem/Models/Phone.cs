namespace BillingSystem.Models
{
    public class Phone
    {
        public string PhoneNumber { get; set; }

        public decimal Balance { get; set; } = 0;

        public Phone(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}