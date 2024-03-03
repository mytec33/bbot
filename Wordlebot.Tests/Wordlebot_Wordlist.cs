using Wordlebot;

namespace Wordlebot.Tests
{
    public class Wordlebot_WordList
    {
        [Theory]
        [ClassData(typeof(RemoveWordsWitLetterData))]
        public void Test_RemoveWordsWitLetter(char letter, List<string> list, List<string> expected)
        {
            var consoleLogger = new ConsoleLogger("quiet");
            var wordList = new WordleWordList(list, consoleLogger);
           
            wordList.RemoveWordsWithLetter(letter);

            Assert.Equal(wordList.Words, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithLetterByIndexData))]
        public void Test_RemoveWordsWithLetterByIndex(int index, char letter, List<string> list, List<string> expected)
        {
            var consoleLogger = new ConsoleLogger("quiet");
            var wordList = new WordleWordList(list, consoleLogger);

            wordList.RemoveWordsWithLetterAtIndex(index, letter);

            Assert.Equal(wordList.Words, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithoutLetterData))]
        public void Test_RemoveWordsWithoutLetter(char letter, List<string> list, List<string> expected)
        {
            var consoleLogger = new ConsoleLogger("quiet");
            var wordList = new WordleWordList(list, consoleLogger);

            wordList.RemoveWordsMissingLetter(letter);

            Assert.Equal(wordList.Words, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithoutLetterByIndexData))]
        public void Test_RemoveWordsWithoutLetterByIndex(int index, char letter, List<string> list, List<string> expected)
        {
            var consoleLogger = new ConsoleLogger("quiet");
            var wordList = new WordleWordList(list, consoleLogger);

            wordList.RemoveWordsWithoutLetterByIndex(index, letter);

            Assert.Equal(wordList.Words, expected);
        }
    }
}