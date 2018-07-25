using CompleetKassa.DataTypes.Enumerations;
using System.Windows.Input;

namespace CompleetKassa.Models
{
    public class Product
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public ICommand Command { get; set; }
        public ProductCategories Category { get; set; }
    }
}
