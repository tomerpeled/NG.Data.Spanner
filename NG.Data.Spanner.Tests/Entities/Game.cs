
using System;
using System.Collections.Generic;

namespace NG.Data.Spanner.Tests.Entities
{
	public class Game
	{
		public string GameId { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime? EndTime { get; set; }

		public List<GamePlayer> GamePlayers { get; set; }

	}
}
