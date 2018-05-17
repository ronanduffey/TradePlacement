namespace TradePlacement.Models
{
    public class OrderTick
    {
        public double Price { get; set; }
        public double Stake { get; set; }

        public OrderTick(double price, double stake)
        {
            Price = price;
            Stake = stake;
        }
    }
}
