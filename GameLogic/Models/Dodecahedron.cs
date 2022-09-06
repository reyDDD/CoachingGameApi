using TamboliyaApi.Data;

namespace TamboliyaApi.GameLogic.Models
{
    public class Dodecahedron
    {
        private readonly AppDbContext context;
        private readonly List<SideOfDodecahedron> sidesOfDodecahedron;

        public Dodecahedron(AppDbContext context)
        {
            this.context = context;
            sidesOfDodecahedron = context.SideOfDodecahedrons.ToList();
        }

        public SideOfDodecahedron ThrowBone()
        {
            Random random = new Random();
            return sidesOfDodecahedron[random.Next(0, sidesOfDodecahedron.Count())];
        }
    }
}
