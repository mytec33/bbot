using Wordlebot;

namespace Wordlebot.Tests
{
    public class Wordlebot_WordList
    {
        [Theory]
        [ClassData(typeof(RemoveWordsWitLetterData))]
        public void Test_RemoveWordsWitLetter(char letter, List<string> list, List<string> expected)
        {
            var score = new Scoring();
            var result = WordList.RemoveWordsWithLetter(letter, list);

            Assert.Equal(result, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithLetterByIndexData))]
        public void Test_RemoveWordsWithLetterByIndex(int index, char letter, List<string> list, List<string> expected)
        {
            var score = new Scoring();
            var result = WordList.RemoveWordsWithLetterByIndex(index, letter, list);

            Assert.Equal(result, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithoutLetterData))]
        public void Test_RemoveWordsWithoutLetter(char letter, List<string> list, List<string> expected)
        {
            var score = new Scoring();
            var result = WordList.RemoveWordsWithoutLetter(letter, list);

            Assert.Equal(result, expected);
        }

        [Theory]
        [ClassData(typeof(RemoveWordsWithoutLetterByIndexData))]
        public void Test_RemoveWordsWithoutLetterByIndex(int index, char letter, List<string> list, List<string> expected)
        {
            var score = new Scoring();
            var result = WordList.RemoveWordsWithoutLetterByIndex(index, letter, list);

            Assert.Equal(result, expected);
        }
    }
}