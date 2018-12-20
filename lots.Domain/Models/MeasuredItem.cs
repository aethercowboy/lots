namespace lots.Domain.Models
{
    public class MeasuredItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Score { get; set; }

        public override string ToString() => $"[{Id}] {Name} - {Description} ({Score})";
    }
}
