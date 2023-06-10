using System;
using System.Linq;
using Wordlebot;

namespace Wordlebot
{
    internal class Program
    {
        private struct WordleLetter
        {
            public int Index;
            public char Letter;
            public int Frequency;
        }

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
                //var wordlist = new WordList("./5_letter_words");
                var wordlist = new WordList("./5_letter_words_official");
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
                        Words = RemoveWordsWithLetter(guess[x], Words);
                    }
                    else if (action == "hint" || action == "unused match")
                    {
                        Console.WriteLine($"\t{guess[x]} is a {action}. Removing from all words with this letter in this spot: {x + 1}");

                        // Word cannot have hint in this spot, so remove those before we try to find words
                        // with hint elsewhere otherwise this spot will be a false positive
                        Words = RemoveWordsWithLetterByIndex(x, guess[x], Words);
                        Words = RemoveWordsWithoutLetter(guess[x], Words);
                    }
                    else if (action == "match")
                    {
                        Console.WriteLine($"\t{guess[x]} is a match. Removing from all words without this letter in this spot: {x + 1}");
                        Words = RemoveWordsWithoutLetterByIndex(x, guess[x], Words);
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
                        //Words = SortListByMarks(marks.marks, guess, Words);
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

        static List<string> SortListByMisses(int[] marks, string guess, List<string> wordList)
        {
            var frequentLetters = new List<char>();
            var frequentLetters2 = new List<WordleLetter>();

            // Find the most frequent letters in each blank position
            for (int x = 0; x < 5; x++)
            {
                Console.WriteLine($"\n\tIndex: {x + 1}");

                if (marks[x] == 0 || marks[x] == 3)
                {
                    var kv = GetMostFrequentLetter(wordList, x);

                    if (frequentLetters.Contains(kv.Key))
                    {
                        Console.WriteLine("\tIgnoring duplicate");
                    }
                    else
                    {
                        frequentLetters2.Add(new WordleLetter() { Index = x, Letter = kv.Key, Frequency = kv.Value });
                        frequentLetters.Add(kv.Key);
                    }
                }
            }

            var sortedFrequency = frequentLetters2.OrderByDescending(x => x.Frequency)
                .ToList();

            Console.WriteLine($"\tHighest frequency: {sortedFrequency.First().Frequency}");
            var removeFrequences = new List<WordleLetter>();
            if (sortedFrequency.First().Frequency > 1 && sortedFrequency.Count > 1)
            {
                foreach (WordleLetter l in sortedFrequency)
                {
                    if (l.Frequency == 1)
                    {
                        Console.WriteLine($"Removing letter frequency of one for: {l.Letter}");
                        removeFrequences.Add(l);
                    }
                }

                sortedFrequency.RemoveAll(item => removeFrequences.Contains(item));
            }

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
                    if (word[l.Index] == l.Letter)
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

        private static KeyValuePair<char, int> GetMostFrequentLetter(List<string> list, int index)
        {
            var frequency = new Dictionary<char, int>();

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

            var maxKV = frequency.FirstOrDefault(x => x.Value == frequency.Values.Max());
            Console.WriteLine($"\tMax value: {maxKV.Key} => {maxKV.Value}");

            return maxKV;
        }

        static List<string> SortListByMarks(int[] marks, string guess, List<string> list)
        {
            var frequency = new Dictionary<char, int>();
            int index = 0;

            // This isn't quite right. We care about hints in the index we are looking at,
            // not hints in general
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

        static List<string> RemoveWordsWithoutLetter(char letter, List<string> list)
        {
            var keepers = new List<string>();

            foreach (string word in list)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (word[x] == letter)
                    {
                        keepers.Add(word);
                        break;
                    }
                }
            }

            return keepers;
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
