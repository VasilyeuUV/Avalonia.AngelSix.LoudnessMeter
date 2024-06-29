using test.Interfaces;

namespace test.Models
{
    internal class CommonControl : IControl
    {
        public int? Key { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public double? Height { get; set; }
    }
}
