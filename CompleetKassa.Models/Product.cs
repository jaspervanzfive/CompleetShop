using CompleetKassa.DataTypes.Enumerations;
using System.Windows.Input;

namespace CompleetKassa.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public ICommand Command { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
    }
}
