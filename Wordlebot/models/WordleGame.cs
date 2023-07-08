using System.Diagnostics;
using System.Text;

namespace Wordlebot
{
    public class WordleGame
    {
        private struct WordleLetter
        {
            public char Letter;
            public int Frequency;
        }

        static bool ResultOnly = false;
        static string StartingWord = "";
        static string Wordle = "";

        private ILogger Logger { get; init; }

        static readonly List<char> FrequentLetters = new() { 't', 's', 'r', 'e', 'a', 'i', 'c', 'n', 'l' };
        static List<string> Words = new();

        public WordleGame(ILogger logger, List<string> words, string startingWord, string wordle, bool resultsOnly)
        {
            Logger = logger;
            ResultOnly = resultsOnly;
            StartingWord = startingWord;
            Wordle = wordle;
            Words = words;
        }

        public string PlayWordle()
        {
            Logger.WriteLine($"Words: {Words.Count:#,##0}");
            Logger.WriteLine($"First word: {StartingWord}");
            Logger.WriteLine($"Wordle: {Wordle}\n");

            int attempts = 1;
            string? guess;
            var marks = new Scoring(Logger);
            while (attempts <= Constants.GAME_MAX_GUESSES)
            {
                if (attempts == 1)
                {
                    guess = StartingWord;
                }
                else
                {
                    // TODO: if more than one item, this should have some heuristic rather
                    // than picking first.
                    //ReduceSet(Words);

                    guess = Words[0];
                }
                RemoveFrequentLettersByGuess(guess);

                Logger.WriteLine($"Guess {attempts}: {guess}");

                if (guess == Wordle)
                {
                    if (ResultOnly)
                    {
                        return $"{attempts} tries";
                    }
                    else
                    {
                        return $"You found the wordle in {attempts} tries!";
                    }
                }
                else if (attempts == Constants.GAME_MAX_GUESSES)
                {
                    return $"You didn't find the Wordle. The Wordle is: {Wordle}.";
                }

                Logger.WriteLine($"Scoring word: {guess}");
                marks.ScoreWord(guess, Wordle);
                marks.PrintMarks();

                // Work through each tile based on current score
                for (int x = 0; x < 5; x++)
                {
                    Logger.WriteLine($"Tile: {x + 1}");

                    int score = marks.GetTileScore(x);
                    string action = Scoring.GetTileScoreDescription(score);

                    if (score == Constants.SCORE_NOT_IN_WORD)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words");
                        Words = WordList.RemoveWordsWithLetter(guess[x], Words);
                    }
                    else if (score == Constants.SCORE_HINT || score == Constants.SCORE_MATCH_UNUSED)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words with this letter in this spot: {x + 1}");

                        // Word cannot have hint in this spot, so remove those before we try to find words
                        // with hint elsewhere otherwise this spot will be a false positive
                        Words = WordList.RemoveWordsWithLetterByIndex(x, guess[x], Words);
                        Words = WordList.RemoveWordsWithoutLetter(guess[x], Words);
                    }
                    else if (score == Constants.SCORE_MATCH)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words without this letter in this spot: {x + 1}");
                        Words = WordList.RemoveWordsWithoutLetterByIndex(x, guess[x], Words);
                    }
                    else if (score == Constants.SCORE_HINT_UNUSED)
                    {
                        Logger.WriteLine("\tHINT_USED hit");
                    }
                }

                Logger.WriteLine($"Found {Words.Count:#,##0} potential words");
                PrintWorldList(Words);

                if (Words.Count > 1)
                {
                    if (marks.NoMisses())
                    {
                        Words = SortWordsByNoMisses(marks.marks, guess, Words);
                    }
                    else
                    {
                        Words = SortListByMisses(Words);
                    }
                }

                if (guess == null)
                {
                    return "null";
                }
                if (Words.Count < 1)
                {
                    return "No words remaining";
                }

                attempts++;
            }

            return "Reached unexepected return point";
        }

        static void RemoveFrequentLettersByGuess(string guess)
        {
            foreach (char letter in guess)
            {
                FrequentLetters.Remove(letter);
            }
        }

        private List<string> SortListByMisses(List<string> wordList)
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

        private List<string> SortWordsByNoMisses(int[] marks, string guess, List<string> wordList)
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

        private List<string> CandidateWordsByKnownLetters(List<string> localWords, List<char> letters)
        {
            var matchedWords = new List<string>();

            Logger.WriteLine($"Looking for words with {letters.Count} hints");

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

        private void PrintWorldList(List<string> list)
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

            Logger.WriteLine(words.ToString());
        }
    }
}