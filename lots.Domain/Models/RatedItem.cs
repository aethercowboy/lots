namespace lots.Domain.Models
{
    public class RatedItem : MeasuredItem
    {
        public RatedItem(MeasuredItem mi, decimal rating)
        {
            Id = mi.Id;
            Name = mi.Name;
            Description = mi.Description;
            Score = mi.Score;
            Rating = rating;
        }

        public decimal Rating { get; set; }

        public override string ToString()
        {
            var x = base.ToString();

            return $"{x} {{{Rating}}}";
        }
    }
}
