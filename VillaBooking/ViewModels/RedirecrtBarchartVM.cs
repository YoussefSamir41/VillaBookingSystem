namespace VillaBooking.ViewModels
{
    public class RedirecrtBarchartVM
    {
        public decimal TotalCount { get; set; }
        public decimal IncreasDeacreasAmount { get; set; }
        public bool HasRatioIncrease { get; set; }
        public int[] Series { get; set; }
    }
}
