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

            var logger = new ConsoleLogger("quiet");
            string WordListFile = Path.Combine(projectDirectory, "Wordlebot", "5_letter_words_official.txt");
            var wordlist = new WordleWordList(WordListFile, logger);

            var args = new ProgramArguments
            {
                ResultOnly = true,
                StartingWord = startingWord,
                Wordle = wordle,
                WordListFile = ""
            };

            var game = new WordleGame(logger, wordlist, args);
            var result = game.PlayWordle();

            Assert.Equal(expected, result);
        }

        [Theory]
        [ClassData(typeof(VersusWordleBot))]
        public void VersusWordleBot(string startingWord, string wordle, string expected, string wordleResult)
        {
            string? projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.FullName;
            if (projectDirectory == null)
            {
                Assert.Fail($"Wordlist file not found.");
            }

            var logger = new ConsoleLogger("quiet");
            string WordListFile = Path.Combine(projectDirectory, "Wordlebot", "5_letter_words_official.txt");
            var wordlist = new WordleWordList(WordListFile, logger);

            var args = new ProgramArguments
            {
                ResultOnly = true,
                StartingWord = startingWord,
                Wordle = wordle,
                WordListFile = ""
            };

            var game = new WordleGame(logger, wordlist, args);
            var result = game.PlayWordle();

            Assert.Equal(expected, result);
            Assert.Equal(expected, wordleResult);
        }
    }
}