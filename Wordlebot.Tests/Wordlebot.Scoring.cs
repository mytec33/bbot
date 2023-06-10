

namespace Wordlebot.Tests
{
    public class PrimeService_IsPrimeShould
    {
        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var marks = new Marks();
            string result = marks.TileScore(0);

            Assert.True(result == "miss", "miss");
        }
    }
}