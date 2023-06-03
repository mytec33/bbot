using System;
using System.Linq;
using Wordlebot;

namespace MyApp
{
    internal class Program
    {
        static List<string> Words = new();

        private static readonly int HINT_MARKER = 1;
        private static readonly int MATCH_MARKER = 2;
        private static readonly int MAX_GUESSES = 6;

        static void Main(string[] args)
        {
            string? wordle;
            var usedLetters = new List<string>();

            try
            {
                //WordList = ReadWordList("./5_letter_words");
                WordList wordlist = new("./5_letter_words_official");
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
            Marks marks = new();
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
                    string action = TileScore(score);

                    if (action == "miss")
                    {
                        Console.WriteLine($"\t{guess[x]} is a miss. Removing from all words");
                        Words = RemoveWordsWithLetter(guess[x], Words);
                    }
                    else if (action == "hint")
                    {
                        Console.WriteLine($"\t{guess[x]} is a miss. Removing from all words with this letter in this spot: {x + 1}");
                        Words = RemoveWordsWithLetterByIndex(x, guess[x], Words);
                    }
                    else if (action == "match")
                    {
                        Console.WriteLine($"\t{guess[x]} is a match. Removing from all words without this letter in this spot: {x + 1}");
                        Words = RemoveWordsWithoutLetterByIndex(x, guess[x], Words);
                    }
                }

                Words = SortListByMarks(marks.marks, guess, Words);

                Console.WriteLine($"Found {Words.Count:#,##0} potential words");
                PrintWorldList(Words);

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

        // This is probably better suited if somehow I had a type that indicated the limits
        // so I'd only need the first 3
        static string TileScore(int score) =>
            score switch
            {
                0 => "miss",
                1 => "hint",
                2 => "match",
                < 0 => "error",
                > 2 => "error"
            };

        static string FindWordWithVowels(List<char> vowels, List<string> list, int numberMatches)
        {
            int count = 0;

            foreach (string word in list)
            {
                count = 0;
                foreach (char vowel in vowels)
                {
                    if (word.Contains(vowel))
                    {
                        count++;
                    }
                }

                if (count == vowels.Count)
                {
                    return word;
                }
            }

            if (count < vowels.Count)
            {
                FindWordWithVowels(vowels, list, vowels.Count - 1);
            }

            return "";
        }

        static List<char> GetHintLetters(int[] marks, string guess)
        {
            var hints = new List<char>();

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] == HINT_MARKER)
                {
                    hints.Add(guess[x]);
                }
            }

            return hints;
        }

        static List<string> SortListByMarks(int[] marks, string guess, List<string> list)
        {
            var frequency = new Dictionary<char, int>();
            int index = 0;

            var hints = GetHintLetters(marks, guess);
            Console.WriteLine($"Hints: {string.Join(" ", hints)}");

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] != MATCH_MARKER)
                {
                    index = x;
                    break;
                }
            }
            Console.WriteLine($"First index with no match: {index + 1}");

            // Find first empty mark and find most common letter
            foreach (string word in list)
            {
                if (frequency.ContainsKey(word[index]))
                {
                    frequency[word[index]] = frequency[word[index]] + 1;
                }
                else
                {
                    frequency.Add(word[index], 1);
                }
            }

            // Remove any hints we have from the frequency list
            frequency = frequency.Where(kv => !hints.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (KeyValuePair<char, int> kv in frequency)
            {
                Console.WriteLine($"\t{kv.Key}: {kv.Value}");
            }

            var maxKeyValuePair = frequency.FirstOrDefault(x => x.Value == frequency.Values.Max());
            char maxKey = maxKeyValuePair.Key;
            int maxValue = maxKeyValuePair.Value;
            Console.WriteLine($"\tMax value: {maxKey} => {maxValue}");

            var maxKeyValueList = list
                .Select(word => new { Word = word, Char = word.ElementAtOrDefault(index) })
                .GroupBy(x => x.Char)
                .OrderByDescending(group => group.Count())
                .FirstOrDefault();

            List<string> sortedWords = maxKeyValueList != null ? maxKeyValueList.Select(x => x.Word).ToList().Concat(list.Except(maxKeyValueList.Select(x => x.Word))).ToList() : list;
            return sortedWords;
        }

        static List<string> RemoveWordsWithLetter(char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach (string word in list)
            {
                if (word.Contains(letter))
                {
                    deletes.Add(word);
                }
            }

            list.RemoveAll(item => deletes.Contains(item));
            return list;
        }

        static List<string> RemoveWordsWithLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach (string word in list)
            {
                if (word[index] == letter)
                {
                    deletes.Add(word);
                }
            }

            list.RemoveAll(item => deletes.Contains(item));
            return list;
        }

        static List<string> RemoveWordsWithoutLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach (string word in list)
            {
                if (word[index] != letter)
                {
                    deletes.Add(word);
                }
            }

            list.RemoveAll(item => deletes.Contains(item));
            return list;
        }

        static void PrintWorldList(List<string> list)
        {
            int count = 1;

            Console.Write("\t");
            foreach (string word in list)
            {
                Console.Write($"{word} ");

                if (count % 20 == 0)
                {
                    Console.WriteLine();
                    Console.Write("\t");
                    count = 0;
                }

                count++;
            }

            Console.WriteLine();
        }
    }
}
