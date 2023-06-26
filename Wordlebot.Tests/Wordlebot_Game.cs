using System.Diagnostics;

namespace Wordlebot.Tests
{
    public class Wordlebot_Game
    {
        [Theory]
        [ClassData(typeof(GameResults_Arose))]
        [ClassData(typeof(GameResults_Crane))]
        [ClassData(typeof(GameResults_Magic))]
        public void GetWordleResult(string startingWord, string wordle, string expected)
        {
            string? projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
            if (projectDirectory == null)
            {
                Assert.Fail($"Wordlist file not found.");
            }

            string WordListFile = Path.Combine(projectDirectory, "Wordlebot", "5_letter_words_official.txt");
            var wordlist = new WordList(WordListFile);
            var Words = wordlist.Words;

            var logger = new FileLogger("quiet");

            var game = new WordleGame(logger, Words, startingWord, wordle, true);
            var result = game.PlayWordle();

            Assert.Equal(expected, result);
        }
    }
}