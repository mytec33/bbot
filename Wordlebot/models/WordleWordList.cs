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
        static readonly List<char> FrequentLetters = new() { 't', 's', 'r', 'e', 'a', 'i', 'c', 'n', 'l' };
        public List<string> Words { get; private set; }

        public WordleWordList(string filename, ILogger logger)
        {
            Words = ReadWordList(filename);
            Logger = logger;
        }

        private static List<WordleLetter> AddLettersToFrequency(List<string> wordList)
        {
            var foundLetters = new List<char>();
            var frequentLetters = new Dictionary<char, int>();
            var sortedFrequency = new List<WordleLetter>();

            foreach (string word in wordList)
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

        private static List<string> CandidateWordsByFrequentLetters(List<string> localWords, List<WordleLetter> letters, int count)
        {
            var matchedWords = new List<string>();

            foreach (string word in localWords)
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

        private static List<string> CandidateWordsByKnownLetters(List<string> localWords, List<char> letters)
        {
            var matchedWords = new List<string>();

            foreach (string word in localWords)
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

        public static void PrintWorldList(List<string> list, ILogger logger)
        {
            int count = 1;
            var words = new StringBuilder();

            words.Append('\t');
            foreach (string word in list)
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

            logger.WriteLine(words.ToString());
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

        static public List<string> RemoveWordsWithLetter(char letter, List<string> list)
        {
            var deletes = list.Where(word => word.Contains(letter)).ToList();
            list.RemoveAll(item => deletes.Contains(item));

            return list;
        }

        static public List<string> RemoveWordsWithLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = list.Where(word => word[index] == letter).ToList();
            list.RemoveAll(item => deletes.Contains(item));

            return list;
        }

        static public List<string> RemoveWordsWithoutLetter(char letter, List<string> list)
        {
            var keepers = list.Where(word => word.Any(c => c == letter)).ToList();

            return keepers;
        }

        static public List<string> RemoveWordsWithoutLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = list.Where(word => word[index] != letter).ToList();
            list.RemoveAll(item => deletes.Contains(item));

            return list;
        }


        public static void RemoveFrequentLettersByGuess(string guess)
        {
            foreach (char letter in guess)
            {
                FrequentLetters.Remove(letter);
            }
        }

        public List<string> SortWordsByNoMisses(int[] marks, string guess, List<string> wordList)
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
                matchedWords = CandidateWordsByKnownLetters(wordList, hints);

                if (matchedWords.Count > 0)
                {
                    Logger.WriteLine($"Found words with {count} of {hints.Count} matches");
                    break;
                }

                count--;
            }

            return matchedWords;
        }

        public List<string> SortListByMisses(List<string> wordList)
        {
            var frequentLetters = new List<char>();
            var frequentLetters2 = new List<WordleLetter>();

            var sortedFrequency = AddLettersToFrequency(wordList);
            sortedFrequency = sortedFrequency.OrderByDescending(x => x.Frequency).ToList();

            int count = sortedFrequency.Count;
            var matchedWords = new List<string>();
            while (count > 0)
            {
                Logger.WriteLine($"Looking for words with {count} of {sortedFrequency.Count} matches");
                matchedWords = CandidateWordsByFrequentLetters(wordList, sortedFrequency, count);
                if (matchedWords.Count > 0)
                {
                    Logger.WriteLine($"Found words with {count} of {sortedFrequency.Count} matches");
                    break;
                }

                sortedFrequency.Remove(sortedFrequency.Last());
                count--;
            }

            if (matchedWords.Count < 1)
            {
                Logger.WriteLine("No candidate words found. Quitting.");
                Environment.Exit(3);
            }

            var alternateWord = matchedWords[0];
            Logger.WriteLine($"Alternate word: {alternateWord}");

            wordList.Remove(alternateWord);
            wordList.Insert(0, alternateWord);

            return wordList;
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