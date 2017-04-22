using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Data.Spanner.Tests.Entities
{
    // Many to many relationship example
	public class GamePlayer
	{
		public string GameId { get; set; }
		public Game Game { get; set; }

		public string PlayerId { get; set; }
		public Player Player { get; set; }
	}
}
