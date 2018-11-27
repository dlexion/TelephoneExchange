namespace BillingSystem.Models
{
    public class Tariff
    {
        public int PrepaidMinutes { get; } = 200;

        public decimal CostPerMonth = 5.3m;

        public decimal CostPerExtraMinute = 0.2m;
    }
}