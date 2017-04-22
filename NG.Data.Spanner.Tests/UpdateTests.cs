using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NG.Data.Spanner.Tests.Entities;
using Xunit;

namespace NG.Data.Spanner.Tests
{
    public class UpdateTests : BaseTests
    {

        [Fact]
        public async Task UpdateInContexTest()
        {
            string playerId;
            using (var ctx = new AppDb())
            {
                var player = await ctx.Players.FirstOrDefaultAsync();
                playerId = player.PlayerId;
                player.Status = Status.NotActive;
                player.Age = 50;

                await ctx.SaveChangesAsync();

                var updatedPlayer = await ctx.Players.Where(p => p.PlayerId == playerId).FirstOrDefaultAsync();
                Assert.Equal(updatedPlayer.Age, 50);
            }
        }

        [Fact]
        public async Task UpdateInBetweenContexesTest()
        {
            Player player;
            using (var ctx = new AppDb())
            {
                player = await ctx.Players.FirstOrDefaultAsync();
            }

            var newStatus = player.Status == Status.Active ? Status.NotActive : Status.Active; // Toggle status
            using (var ctx = new AppDb())
            {
                ctx.Attach(player);
                player.Status = newStatus;

                await ctx.SaveChangesAsync();
            }

            using (var ctx = new AppDb())
            {
                var updatedPlayer = await ctx.Players.Where(p => p.PlayerId == player.PlayerId).FirstOrDefaultAsync();
                Assert.Equal(updatedPlayer.Status, newStatus);
            }
        }
       
    }
}
