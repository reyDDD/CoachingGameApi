using TamboliyaApi.Data;
using TamboliyaApi.Services;

namespace TamboliyaApi.GameLogic.Models
{
	public class Dodecahedron
	{
		private readonly AppDbContext context;
		private readonly List<SideOfDodecahedron> sidesOfDodecahedron;
		private readonly RandomService randomService;

		public Dodecahedron(AppDbContext context, RandomService randomService)
		{
			this.randomService = randomService;
			this.context = context;
			sidesOfDodecahedron = context.SideOfDodecahedrons.ToList();
		}

		public SideOfDodecahedron ThrowBone()
		{
			var randomValue = randomService.RandomNumber(0, 11);
			return sidesOfDodecahedron[randomValue];
		}
	}
}
