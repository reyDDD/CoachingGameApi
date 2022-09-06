using System.ComponentModel.DataAnnotations;

namespace TamboliyaApi.Data
{
    public class SideOfDodecahedron
    {
        private int number;

        [Key]
        public int Id { get; set; }

        public int Number
        {
            get { return number; }
            init
            {
                if (value <= 0 || value > 12)
                {
                    throw new ArgumentException("Number must be less then 12 and better then 0");
                }
                else
                {
                    number = value;
                }
            }
        }

        public Color Color { get; init; }
    }
}
