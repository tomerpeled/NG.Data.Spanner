using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NG.Data.Spanner.Tests
{
    public class QueryTests: BaseTests
    {

        [Fact]
        public async Task GetByIdTest()
        {
            using (var ctx = new AppDb())
            {
                var country = await ctx.Countries.Where(c => c.CountryCode == "code0").FirstOrDefaultAsync();
                Assert.NotNull(country);
            }
        }

        [Fact]
        public async Task ProjectionTest()
        {
            using (var ctx = new AppDb())
            {
                var countryName = await ctx.Countries.Where(c => c.CountryCode == "code0").Select(c => c.Name).FirstOrDefaultAsync();
                Assert.NotNull(countryName);
            }
        }

        [Fact]
        public async Task CountTest()
        {
            using (var ctx = new AppDb())
            {
                Assert.NotEqual(0, await ctx.Countries.CountAsync());
            }
        }

        [Fact]
        public async Task SelectAllTest()
        {
            using (var ctx = new AppDb())
            {
                var countries = await ctx.Countries.ToListAsync();
                Assert.NotNull(countries);
            }
        }

        [Fact]
        public async Task TakeTest()
        {
            using (var ctx = new AppDb())
            {
                var countries = await ctx.Countries.Take(10).ToListAsync();
                Assert.Equal(countries.Count, 10);
            }
        }

        [Fact]  
        public async Task SkipTest()
        {
            using (var ctx = new AppDb())
            {
                var countries = await ctx.Countries.Skip(10).Take(2).ToListAsync();
                Assert.Equal(countries.Count, 2);
            }
        }

        [Fact]
        public async Task GreaterThanTest()
        {
            using (var ctx = new AppDb())
            {
                var players = await ctx.Players.Where(p => (int)p.Status > 1).ToListAsync();
                Assert.Equal(players.Count, 0);
            }
        }

        [Fact]
        public async Task IncludeTest()
        {
            using (var ctx = new AppDb())
            {
                var games = await ctx.Players
                    .Include(p => p.GamePlayers)
                    .Where(p => p.PlayerId == "be8a5fd1-ed99-47dd-a35a-05ef4904bb53")
                    .Select(p => new { p.PlayerId, p.GamePlayers })
                    .ToListAsync();
            }
        }

        [Fact]
        public async Task SumTest()
        {
            using (var ctx = new AppDb())
            {
                var statusSum = await ctx.Players.SumAsync(p => (int)p.Status);
                Assert.Equal(statusSum, 10);
            }
        }

        [Fact]
        public async Task AverageTest()
        {
            using (var ctx = new AppDb())
            {
                var statusAvg = await ctx.Players.AverageAsync(p => (int)p.Status);
                Assert.Equal(statusAvg, 1);
            }
        }

        [Fact]
        public async Task NavigationTest()
        {
            using (var ctx = new AppDb())
            {
                var country = await ctx.Players
                    .Select(p => p.Country).FirstOrDefaultAsync();
                Assert.NotNull(country);
            }
        }

        [Fact]
        public async Task GroupByTest()
        {
            using (var ctx = new AppDb())
            {
                var groupByCountries = await ctx.Players.GroupBy(p => p.CountryCode).ToListAsync();
            }
        }

    }
}
