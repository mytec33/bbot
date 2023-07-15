using System.Linq;
using System.Text;

namespace Wordlebot
{
    public class WordleWordList
    {
        public struct WordleLetter
        {
            public char Letter;
            public int Frequency;
        }

        private ILogger Logger { get; set; }
        readonly List<char> FrequentLetters = new() { 't', 's', 'r', 'e', 'a', 'i', 'c', 'n', 'l' };
        public List<string> Words { get; private set; }

        public WordleWordList(string filename, ILogger logger)
        {
            Words = ReadWordList(filename);
            Logger = logger;
        }

        private List<WordleLetter> AddLettersToFrequency()
        {
            var foundLetters = new List<char>();
            var frequentLetters = new Dictionary<char, int>();
            var sortedFrequency = new List<WordleLetter>();

            foreach (string word in Words)
            {
                foundLetters.Clear();
                foreach (char letter in word)
                {
                    if (foundLetters.Contains(letter))
                    {
                        // We don't want to give extra weight to words with double letters like 'p' in guppy
                        continue;
                    }

                    foundLetters.Add(letter);
                    if (frequentLetters.ContainsKey(letter))
                    {
                        frequentLetters[letter] = frequentLetters[letter] + 1;
                    }
                    else
                    {
                        frequentLetters.Add(letter, 1);
                    }
                }
            }
            var frequentLettersList = frequentLetters.OrderByDescending(x => x.Value).ToList();

            foreach (KeyValuePair<char, int> kv in frequentLettersList)
            {
                bool found = false;
                foreach (WordleLetter letter in sortedFrequency)
                {
                    if (letter.Letter == kv.Key)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    var wordleLetter = new WordleLetter()
                    {
                        Letter = kv.Key,
                        Frequency = kv.Value
                    };

                    sortedFrequency.Add(wordleLetter);
                }

                if (sortedFrequency.Count == 5)
                {
                    break;
                }
            }

            return sortedFrequency;
        }

        private List<string> CandidateWordsByFrequentLetters(List<WordleLetter> letters, int count)
        {
            var matchedWords = new List<string>();

            foreach (string word in Words)
            {
                int counter = 0;
                foreach (WordleLetter l in letters)
                {
                    if (word.Contains(l.Letter))
                    {
                        counter++;
                    }
                }

                if (counter >= count)
                {
                    matchedWords.Add(word);
                }
            }

            return matchedWords;
        }

        private List<string> CandidateWordsByKnownLetters(List<char> letters)
        {
            var matchedWords = new List<string>();

            foreach (string word in Words)
            {
                int counter = 0;
                foreach (char letter in word)
                {
                    foreach (char l in letters)
                    {
                        if (letter == l)
                        {
                            counter++;
                        }
                    }
                }

                if (counter >= letters.Count)
                {
                    matchedWords.Add(word);
                }
            }

            return matchedWords;
        }

        public void PrintWorldList()
        {
            int count = 1;
            var words = new StringBuilder();

            words.Append('\t');
            foreach (string word in Words)
            {
                words.Append($"{word} ");

                if (count % 20 == 0)
                {
                    words.Append('\n');
                    words.Append('\t');
                    count = 0;
                }

                count++;
            }

            Logger.WriteLine(words.ToString());
        }

        public void PrintWordList(List<string> wordList)
        {
            int count = 1;
            var words = new StringBuilder();

            words.Append('\t');
            foreach (string word in wordList)
            {
                words.Append($"{word} ");

                if (count % 20 == 0)
                {
                    words.Append('\n');
                    words.Append('\t');
                    count = 0;
                }

                count++;
            }

            Logger.WriteLine(words.ToString());
        }

        public void ReduceWordsBaseOnScore(WordleScoring score, string guess)
        {
            // Work through each tile based on current score
            for (int x = 0; x < 5; x++)
            {
                int tileScore = score.GetTileScore(x);
                string action = WordleScoring.GetTileScoreDescription(tileScore);

                if (tileScore == Constants.SCORE_NOT_IN_WORD)
                {
                    Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words");
                    RemoveWordsWithLetter(guess[x]);
                }
                else if (tileScore == Constants.SCORE_HINT || tileScore == Constants.SCORE_MATCH_UNUSED)
                {
                    Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words with this letter in this spot: {x + 1}");

                    // Word cannot have hint in this spot, so remove those before we try to find words
                    // with hint elsewhere otherwise this spot will be a false positive
                    RemoveWordsWithLetterByIndex(x, guess[x]);
                    RemoveWordsWithoutLetter(guess[x]);
                }
                else if (tileScore == Constants.SCORE_MATCH)
                {
                    Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words without this letter in this spot: {x + 1}");
                    RemoveWordsWithoutLetterByIndex(x, guess[x]);
                }
                else if (tileScore == Constants.SCORE_HINT_UNUSED)
                {
                    Logger.WriteLine("\tHINT_USED hit");
                }
            }
        }

        private static List<string> ReadWordList(string path)
        {
            var list = new List<string>();

            if (!File.Exists(path))
            {
                throw new WordListReadingException($"Could not find file: {path}");
            }

            try
            {
                // Should load a user defined library of words
                StreamReader sr = new(path);

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }

                if (list.Count < 1)
                {
                    throw new WordListReadingException("No data found in file.", new WordListReadingException(""));
                }
            }
            catch (Exception ex)
            {
                throw new WordListReadingException("Error reading word list file.", ex);
            }

            return list;
        }

        public void RemoveWordsWithLetter(char letter)
        {
            var deletes = Words.Where(word => word.Contains(letter)).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }

        public void RemoveWordsWithLetterByIndex(int index, char letter)
        {
            var deletes = Words.Where(word => word[index] == letter).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }

        public void RemoveWordsWithoutLetter(char letter)
        {
            Words = Words.Where(word => word.Any(c => c == letter)).ToList();
        }

        public void RemoveWordsWithoutLetterByIndex(int index, char letter)
        {
            var deletes = Words.Where(word => word[index] != letter).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }


        public void UpdateFrequentLetters(string guess)
        {
            foreach (char letter in guess)
            {
                FrequentLetters.Remove(letter);
            }
        }

        public void SortWordsByNoMisses(int[] marks, string guess)
        {
            var hints = new List<char>();

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] == 1)
                {
                    hints.Add(guess[x]);
                }
            }

            int count = hints.Count;
            var matchedWords = new List<string>();
            while (count > 0)
            {
                matchedWords = CandidateWordsByKnownLetters(hints);

                if (matchedWords.Count > 0)
                {
                    Logger.WriteLine($"Found words with {count} of {hints.Count} matches");
                    break;
                }

                count--;
            }

            Words = matchedWords;
        }

        public void SortListByMisses()
        {
            var alternateWord = new List<string>();
            var frequentLetters = AddLettersToFrequency();

            frequentLetters = frequentLetters.OrderByDescending(x => x.Frequency).ToList();

            while (frequentLetters.Count > 0)
            {
                Logger.WriteLine($"Looking for words with {frequentLetters.Count} of {frequentLetters.Count} matches");
                alternateWord = CandidateWordsByFrequentLetters(frequentLetters, frequentLetters.Count);
                if (alternateWord.Count > 0)
                {
                    Logger.WriteLine($"Found {frequentLetters.Count} words with {frequentLetters.Count} letter matches");
                    break;
                }

                frequentLetters.Remove(frequentLetters.Last());
            }

            if (Words.Count < 1)
            {
                Logger.WriteLine("No candidate words found. Quitting.");
                Environment.Exit(3);
            }

            Logger.WriteLine($"Alternate word: {alternateWord[0]}");
            Words.Remove(alternateWord[0]);
            Words.Insert(0, alternateWord[0]);
        }

    }

    public class WordListReadingException : Exception
    {
        public WordListReadingException(string message) : base(message)
        {
        }

        public WordListReadingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}