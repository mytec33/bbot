namespace Wordlebot.Tests
{
    public class Wordlebot_Scoring
    {
        [Theory]
        [ClassData(typeof(GetTileScoreTestData))]
        public void GetTileScore(int index, string guess, string wordle, int expected)
        {
            var score = new Scoring();
            score.ScoreWord(guess, wordle);
            var result = score.GetTileScore(index);

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [InlineData(-1, "arose", "brass", 0)]
        [InlineData(5, "arose", "brass", 0)]
        public void GetTileScore_Exception(int index, string guess, string wordle, int expected)
        {
            var score = new Scoring();
            score.ScoreWord(guess, wordle);

            Assert.Throws<InvalidOperationException>(() => score.GetTileScore(index));
        }

        [Theory]
        [InlineData(0, "miss")]
        [InlineData(1, "hint")]
        [InlineData(2, "match")]
        [InlineData(3, "unused match")]
        [InlineData(-1, "error")]
        [InlineData(4, "error")]
        public void GetTileScoreDescription(int input, string expected)
        {
            var score = new Scoring();
            string result = score.GetTileScoreDescription(input);

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [InlineData("joker", "joker", true)]
        [InlineData("slate", "tales", true)]
        [InlineData("arose", "tales", false)]
        [InlineData("balsa", "theme", false)]        
        public void HasNoMisses(string guess, string wordle, bool expected)
        {
            var score = new Scoring();
            score.ScoreWord(guess, wordle);
            var result = score.NoMisses();

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [ClassData(typeof(ScoreWordTestData))]
        public void ScoreWord(string guess, string wordle, string expected)
        {
            var score = new Scoring();
            score.ScoreWord(guess, wordle);
            var result = string.Join("", score.marks);

            Assert.True(result == expected, $"{guess} -> {wordle} should be {expected} not {result}");            
        }        
    }
}