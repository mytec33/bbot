namespace Wordlebot.Tests
{
    public class Wordlebot_Scoring
    {
        [Theory]
        [ClassData(typeof(GetTileScoreTestData))]
        public void GetTileScore(int index, string guess, string wordle, int expected)
        {
            var logger = new ConsoleLogger("quiet");
            var score = new WordleScoring(logger);
            score.ScoreWord(guess, wordle);
            var result = score.GetTileScore(index);

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [InlineData(-1, "arose", "brass")]
        [InlineData(5, "arose", "brass")]
        public void GetTileScore_Exception(int index, string guess, string wordle)
        {
            var logger = new ConsoleLogger("quiet");
            var score = new WordleScoring(logger);
            score.ScoreWord(guess, wordle);

            Assert.Throws<InvalidOperationException>(() => score.GetTileScore(index));
        }

        [Theory]
        [InlineData(0, "miss")]
        [InlineData(1, "hint")]
        [InlineData(2, "match")]
        [InlineData(3, "unused hint")]
        [InlineData(4, "unused match")]
        [InlineData(-1, "error")]
        [InlineData(5, "error")]
        public void GetTileScoreDescription(int input, string expected)
        {
            string result = WordleScoring.GetTileScoreDescription(input);

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [InlineData("joker", "joker", true)]
        [InlineData("slate", "tales", true)]
        [InlineData("arose", "tales", false)]
        [InlineData("balsa", "theme", false)]
        public void Test_NoMisses(string guess, string wordle, bool expected)
        {
            var logger = new ConsoleLogger("quiet");
            var score = new WordleScoring(logger);
            score.ScoreWord(guess, wordle);
            var result = score.NoMisses();

            Assert.True(result == expected, $"{result} should be {expected}");
        }

        [Theory]
        [ClassData(typeof(ScoreWordTestData))]
        public void ScoreWord(string guess, string wordle, string expected)
        {
            var logger = new ConsoleLogger("quiet");
            var score = new WordleScoring(logger);
            score.ScoreWord(guess, wordle);
            var result = string.Join("", score.marks);

            Assert.True(result == expected, $"{guess} -> {wordle} should be {expected} not {result}");
        }

        [Theory]
        [ClassData(typeof(HasLetterHint))]
        public void Test_WordleHasLetterHint(char letter, string guess, bool expected)
        {
            Assert.Equal(WordleScoring.WordleHasLetterHint(letter, guess), expected);
        }
    }
}