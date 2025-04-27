using System.Text;

namespace Wordlebot
{
    public class WordleWordList
    {
        public struct WordleLetter
        {
            public char Letter;
            public int Frequency;

            public override readonly string ToString()
            {
                return $"{Letter} {Frequency}";
            }
        }

        private ILogger Logger { get; set; }
        private readonly List<char> PlayedLetters = new();
        public List<string> Words { get; private set; }

        public WordleWordList(string filename, ILogger logger)
        {
            Words = ReadWordList(filename);
            Logger = logger;
        }

        public WordleWordList(List<string> list, ILogger logger)
        {
            Words = list;
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

            // For now we remove all played letters
            // Exact matches are already part of all remaining words so we don't need to find those
            foreach (var letter in PlayedLetters)
            {
                frequentLetters.Remove(letter);
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

                if (sortedFrequency.Count == 12)
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
                    Logger.WriteLine($"\t{guess[x]} is a {action}. Removing all words with this letter in this spot: {x + 1}");
                    Logger.WriteLine($"\t\t     Removing all words missing this letter");

                    // Word cannot have hint in this spot, so remove those before we try to find words
                    // with hint elsewhere otherwise this spot will be a false positive
                    RemoveWordsWithLetterAtIndex(x, guess[x]);
                    RemoveWordsMissingLetter(guess[x]);
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

                // Each character played adjusts the word list one way or the other. Words
                // are either kept and present in all words (somewhere) or don't exist and
                // no word will have them.
                UpdatePlayedLettersByScore(guess[x]);
            }
        }

        private static List<string> ReadWordList(string path)
        {
            var list = new List<string>();

            if (!File.Exists(path))
            {
                throw new WordListReadingException($"Could not find file: {path}");
            }

            list.AddRange(File.ReadAllLines(path));
            if (list.Count < 1)
            {
                throw new WordListReadingException("No data found in file.", new WordListReadingException(""));
            }

            foreach (string word in list)
            {
                if (word.Length != 5)
                {
                    throw new WordListReadingException($"word not equal to 5 characters: {word}");
                }
            }

            return list;
        }

        public void RemoveWordsWithLetter(char letter)
        {
            var deletes = Words.Where(word => word.Contains(letter)).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }

        public void RemoveWordsWithLetterAtIndex(int index, char letter)
        {
            var deletes = Words.Where(word => word[index] == letter).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }

        public void RemoveWordsMissingLetter(char letter)
        {
            Words = Words.Where(word => word.Any(c => c == letter)).ToList();
        }

        public void RemoveWordsWithoutLetterByIndex(int index, char letter)
        {
            var deletes = Words.Where(word => word[index] != letter).ToList();
            Words.RemoveAll(item => deletes.Contains(item));
        }

        public void UpdatePlayedLettersByScore(char letter)
        {
            PlayedLetters.Add(letter);
        }

        public void SortListByMisses()
        {
            var alternateWords = new List<string>();
            var frequentLetters = AddLettersToFrequency();

            frequentLetters = frequentLetters.OrderByDescending(x => x.Frequency).ToList();

            Console.WriteLine("Letters and their frequency:");
            foreach (var letter in frequentLetters.ToList())
            {
                Console.WriteLine($"\t{letter}");
            }

            while (frequentLetters.Count > 0)
            {
                Logger.WriteLine($"Looking for words with {frequentLetters.Count} of {frequentLetters.Count} matches");
                alternateWords = CandidateWordsByFrequentLetters(frequentLetters, frequentLetters.Count);

                Console.WriteLine("Alternate words:");
                foreach (string word in alternateWords)
                {
                    Console.WriteLine($"\t{word}");
                }

                if (alternateWords.Count > 0)
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

            Logger.WriteLine($"Alternate word: {alternateWords[0]}");
            Words.Remove(alternateWords[0]);
            Words.Insert(0, alternateWords[0]);
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