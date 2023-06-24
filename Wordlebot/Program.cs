using System;
using System.Linq;
using System.Text;
using Wordlebot;

namespace Wordlebot
{
    internal class Program
    {
        private struct WordleLetter
        {
            public char Letter;
            public int Frequency;
        }

        static List<char> FrequentLetters = new() { 't', 's', 'r', 'e', 'a', 'i', 'c', 'n', 'l' };
        static List<string> PlayedGuesses = new();
        static List<string> Words = new();

        private static readonly int HINT_MARKER = 1;
        private static readonly int MATCH_MARKER = 2;
        private static readonly int MAX_GUESSES = 6;

        static void Main(string[] args)
        {
            string? wordle;
            var usedLetters = new List<string>();

            if (args.Length != 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tdotnet run guess wordle");
                Environment.Exit(1);
            }

            try
            {
                //var wordlist = new WordList("./Wordlebot/5_letter_words.txt");
                var wordlist = new WordList("./5_letter_words_official.txt");
                Words = wordlist.Words;

                Console.WriteLine($"Words: {Words.Count:#,##0}");
                if (Words.Count < 1)
                {
                    Console.WriteLine("No data file in file. Exiting.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            wordle = args[1];
            Console.WriteLine($"First word: {args[0]}");
            Console.WriteLine($"Wordle: {wordle}\n");

            int attempts = 1;
            string? guess = "";
            Scoring marks = new();
            while (attempts <= MAX_GUESSES)
            {
                if (attempts == 1)
                {
                    guess = args[0];
                }
                else
                {
                    // TODO: if more than one item, this should have some heuristic rather
                    // than picking first.
                    guess = Words[0];
                }
                PlayedGuesses.Add(guess);
                RemoveFrequentLettersByGuess(guess);

                Console.WriteLine($"Guess {attempts}: {guess}");

                if (guess == wordle)
                {
                    Console.WriteLine($"You found the wordle in {attempts} tries!");
                    return;
                }
                else if (attempts == MAX_GUESSES)
                {
                    Console.WriteLine("\nYou didn't find the Wordle. Better luck tomorrow.");
                    Console.WriteLine($"The Wordle is: {wordle}");
                    return;
                }

                Console.WriteLine($"Scoring word: {guess}");
                marks.ScoreWord(guess, wordle);
                marks.PrintMarks();

                // Work through each tile based on current score
                for (int x = 0; x < 5; x++)
                {
                    Console.WriteLine($"Tile: {x + 1}");

                    int score = marks.GetTileScore(x);
                    string action = marks.GetTileScoreDescription(score);

                    if (action == "miss")
                    {
                        Console.WriteLine($"\t{guess[x]} is a miss. Removing from all words");
                        Words = WordList.RemoveWordsWithLetter(guess[x], Words);
                    }
                    else if (action == "hint" || action == "unused match")
                    {
                        Console.WriteLine($"\t{guess[x]} is a {action}. Removing from all words with this letter in this spot: {x + 1}");

                        // Word cannot have hint in this spot, so remove those before we try to find words
                        // with hint elsewhere otherwise this spot will be a false positive
                        Words = WordList.RemoveWordsWithLetterByIndex(x, guess[x], Words);
                        Words = WordList.RemoveWordsWithoutLetter(guess[x], Words);
                    }
                    else if (action == "match")
                    {
                        Console.WriteLine($"\t{guess[x]} is a match. Removing from all words without this letter in this spot: {x + 1}");
                        Words = WordList.RemoveWordsWithoutLetterByIndex(x, guess[x], Words);
                    }
                }

                Console.WriteLine($"Found {Words.Count:#,##0} potential words");
                PrintWorldList(Words);

                if (Words.Count > 1)
                {
                    if (marks.NoMisses())
                    {
                        Words = SortWordsByNoMisses(marks.marks, guess, Words);
                    }
                    else
                    {
                        Words = SortListByMisses(marks.marks, guess, Words);
                    }
                }

                if (guess == null)
                {
                    return;
                }
                if (Words.Count < 1)
                {
                    Console.WriteLine("No words remaining.");
                    return;
                }

                attempts++;
            }
        }

        static void RemoveFrequentLettersByGuess(string guess)
        {
            foreach (char letter in guess)
            {
                FrequentLetters.Remove(letter);
            }
        }

        static List<string> SortListByMisses(int[] marks, string guess, List<string> wordList)
        {
            var frequentLetters = new List<char>();
            var frequentLetters2 = new List<WordleLetter>();

            var sortedFrequency = AddLettersToFrequency(wordList);
            sortedFrequency = sortedFrequency.OrderByDescending(x => x.Frequency).ToList();

            int count = sortedFrequency.Count;
            var matchedWords = new List<string>();
            while (count > 0)
            {
                matchedWords = CandidateWordsByFrequentLetters(wordList, sortedFrequency, count);
                if (matchedWords.Count > 0)
                {
                    Console.WriteLine($"Found words with {count} of {sortedFrequency.Count} matches");
                    break;
                }

                sortedFrequency.Remove(sortedFrequency.Last());
                count--;
            }

            if (matchedWords.Count < 1)
            {
                Console.WriteLine("No candidate words found. Quitting.");
                Environment.Exit(3);
            }

            var alternateWord = matchedWords[0];
            Console.WriteLine($"Alternate word: {alternateWord}");

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

        private static List<string> SortWordsByNoMisses(int[] marks, string guess, List<string> wordList)
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
                    Console.WriteLine($"Found words with {count} of {hints.Count} matches");
                    break;
                }

                count--;
            }

            return matchedWords;
        }

        private static List<string> CandidateWordsByFrequentLetters(List<string> localWords, List<WordleLetter> letters, int count)
        {
            var matchedWords = new List<string>();

            Console.WriteLine($"Looking for words with {count} of {letters.Count} matches");

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

            Console.WriteLine($"Looking for words with {letters.Count} hints");

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

        static void PrintWorldList(List<string> list)
        {
            int count = 1;
            StringBuilder words = new StringBuilder();

            words.Append("\t");
            foreach (string word in list)
            {
                words.Append($"{word} ");

                if (count % 20 == 0)
                {
                    words.Append("\n");
                    words.Append("\t");
                    count = 0;
                }

                count++;
            }

            Console.WriteLine(words);
        }
    }
}
