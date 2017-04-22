using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NG.Data.Spanner.Tests.Entities;

namespace NG.Data.Spanner.Tests
{
    public class DBInitializer
    {
        private readonly Random _rnd;

        private readonly string _spannerConnectionString;

        public DBInitializer(string spannerConnectionString)
        {
            _rnd = new Random();
            _spannerConnectionString = spannerConnectionString;
        }

        public async Task Seed()
        {
            //Drop DB first
            var connectionInfo = new SpannerConnectionStringBuilder(_spannerConnectionString);
            await SpannerInstanceModifier.DropDatabaseAsync(connectionInfo.ProjectId, connectionInfo.InstanceId, connectionInfo.DBName);

            // Create DB with Schema
            var schema = LoadSchema();
            await SpannerInstanceModifier.CreateDatabaseAsync(connectionInfo.ProjectId, connectionInfo.InstanceId, connectionInfo.DBName, schema);

            await GenerateDataAsync();
        }

        private List<string> LoadSchema()
        {
            var schemaLines = File.ReadAllLines("GenerateDBSchema");
            var schemas = new List<string>();
            var currentSchema = string.Empty;
            for (var i = 0; i < schemaLines.Length; i++)
            {
                var line = schemaLines[i];
                if (string.IsNullOrEmpty(line) || line.StartsWith("--")) // Empty or comment
                {
                    continue;
                }

                currentSchema += line.Replace(";", "");
                if (line.EndsWith(";") || i == schemaLines.Length - 1) // Ending with semicolon or last element
                {
                    schemas.Add(currentSchema);
                    currentSchema = string.Empty;
                }
            }
            return schemas;
        }

        private async Task GenerateDataAsync(int countriesCount = 100, int playersCount = 10)
        {
            var countries = new List<Country>();
            var players = new List<Player>();

            using (var ctx = new AppDb())
            {
                for (var i = 0; i < countriesCount; i++)
                {
                    var country = new Country
                    {
                        CountryCode = $"code{i}",
                        Name = $"Name{i}",
                        PropertyNoMapped = 232
                    };
                    countries.Add(country);
                    ctx.Countries.Add(country);
                }

                for (int i = 0; i < playersCount; i++)
                {
                    var player = new Player
                    {
                        PlayerId = ctx.GenerateUniqueId(),
                        Status = Status.Active,
	                    CountryCode = countries[_rnd.Next(0, 20)].CountryCode,
                        Age = 18,
                        IsGamer = true,
                        Address = new ComplexAddress
                        {
                            Street = "Gold 16",
                            Test = 1
                        }
                    };
                    players.Add(player);
                    ctx.Players.Add(player);
                }

                // Create GamePlayer by taking sibling players
                int playerIndex = 0;
                while (playerIndex + 1 < players.Count)
                {
                    var player1 = players[playerIndex];
                    var player2 = players[playerIndex + 1];
                    var gameId = ctx.GenerateUniqueId();
                    var game = new Game
                    {
                        GameId = gameId,
                        CreationTime = DateTime.Now,
                        GamePlayers = new List<GamePlayer>
                        {
                            new GamePlayer
                            {
                                GameId = gameId,
                                PlayerId = player1.PlayerId
                            },
                            new GamePlayer
                            {
                                GameId = gameId,
                                PlayerId = player2.PlayerId
                            }
                        }
                    };
                    ctx.Games.Add(game);
                    playerIndex = playerIndex + 2;
                }

                await ctx.SaveChangesAsync();

            }
        }

    }
}
