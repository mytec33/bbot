using System;
using System.Linq;

namespace MyApp
{
    internal class Program
    {
        static Dictionary<string, int> Letters = new();
        static List<string> LettersByFrequency = new();
        static List<string> PluralsWithS = new();
        static List<string> WordsWithDoubles = new();
        static List<string> WordList = new();


        static void Main(string[] args)
        {
            string? wordle;
            var usedLetters = new List<string>();

            WordList = ReadWordList("./5_letter_words");
            Console.WriteLine($"Words: {WordList.Count:#,##0}");
            if (WordList.Count < 1)
            {
                Console.WriteLine("No data file in file. Exiting.");
                return;
            }


            var letterFrequency = LetterFrequency(WordList);
            //PrintLetterFrequency(letterFrequency);

            PluralsWithS = GetPluralsWithS(WordList);
            WordsWithDoubles = GetWordsWithDoubles(WordList);

            // We don't want plurals ending with 's' as they are incredubly 
            // unlikely to be played
            var firstWordsList = new List<string>(WordList);
            firstWordsList.RemoveAll(item => PluralsWithS.Contains(item));
            firstWordsList.RemoveAll(item => WordsWithDoubles.Contains(item));
            Console.WriteLine($"First words to chose from: {firstWordsList.Count}");
            
            List<string> firstWords = FindMostCommonWordsByLetter(letterFrequency, firstWordsList, 5);

            Console.Write("First words: ");
            Console.Write($"{string.Join(", ", firstWords)}\n");

            Console.Write("Enter the wordle of the day: ");
            wordle = GetTodaysWordle();

            int attempts = 1;
            string? guess = "";
            int[] marks = new int[5] { 0, 0, 0, 0, 0 };
            while(attempts < 7)
            {
                if (attempts == 1)
                    guess = firstWords[0];
                else
                {
                    if (ScoreAllBlanks(marks))
                    {
                        Console.WriteLine("Nothing found. Look for word with most vowels.");
                        
                        // Remove the letters that don't belong (all five in this case!)
                        WordList = RemoveWordsByMark(marks, guess, WordList);

                        // Get a list of vowels we need to find
                        var vowels = GetRemainingVowels(guess);
                        Console.Write($"Remaining vowels: ");
                        foreach(char vowel in vowels)
                        {
                            Console.Write($"{vowel} ");
                        }
                        Console.WriteLine("");

                        // Find a word with the most remaining vowels
                        guess = FindWordWithVowels(vowels, WordList, vowels.Count);
                        WordList.Insert(0, guess);
                    }
                    else if (ScoreAllHints(marks))
                    {
                        Console.WriteLine("Get next word based on hints");

                        WordList = RemoveWordsByHints(marks, guess, WordList);
                        WordList = SortListByMarks(marks, guess, WordList);
                    }
                    else
                    {
                        Console.WriteLine("Get work by chopping off letters");

                        WordList = RemoveWordsByMark(marks, guess, WordList);
                        WordList = SortListByMarks(marks, guess, WordList);                        
                    }

                    Console.WriteLine($"Found {WordList.Count} potential words");
                    PrintWorldList(WordList);

                    if (WordList.Count < 1)
                    {
                        Console.WriteLine("No words remaining.");
                        return;
                    }

                    // TODO: if more than one item, this should have some heuristic rather
                    // than picking first.
                    guess = WordList[0];    
                }

                if (guess == null)
                {
                    return;
                }

                Console.WriteLine($"Guess {attempts}: {guess.ToString()}");

                if (guess == wordle)
                {
                    Console.WriteLine($"You found the wordle in {attempts} tries!");
                    return;
                }

                marks = ScoreWord(guess, wordle);
                Console.WriteLine($"Marks: {string.Join(" ", marks)}");

                attempts++;
            }
        }

        static string FindWordWithVowels(List<char> vowels, List<string> list, int numberMatches)
        {
            int count = 0;

            foreach(string word in list)
            {
                count = 0;
                foreach(char vowel in vowels)
                {
                    if (word.Contains(vowel))
                    {
                        count++;
                    }
                }

                if (count == vowels.Count)
                {
                    Console.WriteLine($"Found a word with all vowels. Returning: {word}");
                    return word;
                }
            }

            if (count < vowels.Count)
            {
                FindWordWithVowels(vowels, list, vowels.Count - 1);
            }

            return "";
        }

        static List<string> SortListByMarks(int[] marks, string guess, List<string> list)
        {
            var frequency = new Dictionary<char, int>();
            int index = 0;

            Console.WriteLine($"\tSorting list of {list.Count:#,##0} words");

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] == 0)
                {
                    index = x;
                    break;
                }
            }
            Console.WriteLine($"\tSortListByMarks index: {index + 1}");

            // Find first empty mark and find most common letter
            foreach(string word in list)
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

            foreach(KeyValuePair<char, int> kv in frequency)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }

            var maxKeyValuePair = frequency.FirstOrDefault(x => x.Value == frequency.Values.Max());
            char maxKey = maxKeyValuePair.Key;
            int maxValue = maxKeyValuePair.Value;
            Console.WriteLine($"{maxKey} => {maxValue}");

            var maxKeyValueList = list
                .Select(word => new { Word = word, Char = word.ElementAtOrDefault(index) })
                .GroupBy(x => x.Char)
                .OrderByDescending(group => group.Count())
                .FirstOrDefault();

            List<string> sortedWords = maxKeyValueList != null ? maxKeyValueList.Select(x => x.Word).ToList().Concat(list.Except(maxKeyValueList.Select(x => x.Word))).ToList() : list;
            return sortedWords;
        }

        static bool ScoreAllBlanks(int[] marks)
        {
            for(int x = 0; x < 5; x++)
            {
                if (marks[x] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        static bool ScoreAllHints(int[] marks)
        {
            for(int x = 0; x < 5; x++)
            {
                if (marks[x] == 0 || marks[x] == 2)
                {
                    return false;
                }
            }

            return true;
        }

        static List<string> RemoveWordsByHints(int[] marks, string guess, List<string> list)
        {
            var discards = new List<string>();
            var hints = new List<char>();

             for (int x = 0; x < 5; x++)
            {
                if (marks[x] == 1)
                {
                    hints.Add(guess[x]);

                    // Remove words with the hint at a specific location; it's elsewhere!
                    foreach(string word in list)
                    {
                        if (word[x] == guess[x])
                        {
                            discards.Add(word);
                        }
                    }
                }
            }
            list.RemoveAll(item => discards.Contains(item));

            // Now find words that have all three letters in any combination
            // TODO!
            discards.Clear();
            bool discard = false;
            foreach (string word in list)
            {
                discard = false;
                foreach(char letter in hints)
                {
                    if (!word.Contains(letter))
                    {
                        discard = true;
                    }
                }

                if (discard)
                {
                    discards.Add(word);
                }
            }
            list.RemoveAll(item => discards.Contains(item));

            list.Remove(guess);
            return list;            
        }        

        static List<string> RemoveWordsByMark(int[] marks, string guess, List<string> list)
        {
            var newList = new List<string>(list);
            Console.WriteLine($"Words in list: {newList.Count}");

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] == 0)
                {
                    Console.WriteLine($"\tLetter '{guess[x]}' not present in Wordle, removing.");
                    newList = RemoveWordsWithLetter(x, guess[x], newList);                    
                    Console.WriteLine($"\tWords in list: {newList.Count}");
                }
                else if (marks[x] == 1)
                {
                    Console.WriteLine($"\tRemoved words with hint '{guess[x]}' at index {x + 1}");
                    newList = RemoveWordsWithLetterInPlace(x, guess[x], newList);
                    Console.WriteLine($"\tWords in list: {newList.Count}");                
                }
                else if (marks[x] == 2)
                {
                    Console.WriteLine($"\tRemoved words missing match '{guess[x]}' at index {x + 1}");
                    newList = RemoveWordsWithoutLetterInPlace(x, guess[x], newList);
                    Console.WriteLine($"\tWords in list: {newList.Count}");                
                }
                else if (marks[x] == 3)
                {
                    Console.WriteLine($"\tRemoved words missing hint '{guess[x]}' at index {x + 1}");
                    newList = RemoveWordsWithLetterInPlace(x, guess[x], newList);
                    Console.WriteLine($"\tWords in list: {newList.Count}");                
                }
                else
                {
                    Console.WriteLine($"\t\tUnknown mark!! {marks[x]}");
                }                
            }

            newList.Remove(guess);
            return newList;            
        }

        static List<string> RemoveWordsWithLetterInPlace(int index, char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach(string word in list)
            {
                if (word[index] == letter)
                {
                    deletes.Add(word);
                }
            }

            foreach(string word in deletes)
            {
                list.Remove(word);
            }

            return list;
        }

        static List<string> RemoveWordsWithoutLetterInPlace(int index, char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach(string word in list)
            {
                if (word[index] != letter)
                {
                    deletes.Add(word);
                }
            }

            list.RemoveAll(item => deletes.Contains(item));
            return list;
        }

        static List<string> RemoveWordsWithLetter(int index, char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach(string word in list)
            {
                if (word.Contains(letter))
                {
                    deletes.Add(word);
                }
            }

            list.RemoveAll(item => deletes.Contains(item));
            return list;
        }

        static int WordleHasLetterHint(char letter, string wordle)
        {
            Console.WriteLine($"Checking {letter}");

            for (int x = 0; x < 5; x++)
            {
                if (wordle[x] == letter)
                {
                    Console.WriteLine($"Found {letter} at {x}");
                    return x;
                }
            }

            return -1;
        }

        static int[] ScoreWord(string guess, string wordle)
        {
            int[] marks = new int[5] { 0, 0, 0, 0, 0 };
            var requiredLetters = new List<char>();

            for (int x = 0; x < 5; x++)
            {
                // Look for exact matches
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                    requiredLetters.Add(guess[x]);

                    continue;
                }

                // Look for hints this spot or to the right
                if (WordleHasLetterHint(guess[x], wordle) >= 0)
                {
                    Console.WriteLine($"Letter '{guess[x]}' exists in wordle");

                    if (requiredLetters.Contains(guess[x]))
                        marks[x] = 3;
                    else 
                        marks[x] = 1;
                }
            }

           return marks;
        }

        static List<char> GetRemainingVowels(string guess)
        {
            var vowels = new List<char> { 'a', 'e', 'i', 'o', 'u' };

            foreach (char letter in guess)
            {
                if (vowels.Contains(letter))
                {
                    vowels.Remove(letter);
                }
            }

            return vowels;
        }

        static string GetTodaysWordle()
        {
            string? wordle = Console.ReadLine();
            if (string.IsNullOrEmpty(wordle) ||
                string.IsNullOrWhiteSpace(wordle) ||
                wordle.Length != 5)
            {
                Console.WriteLine("Invalid world. Must be five leters");
                Environment.Exit(1);
            }
            return wordle.ToLower();
        }

        static List<string> GetWordsWithDoubles(List<string> wordList)
        {
            var lookup = new Dictionary<char, int>();
            var doubles = new List<string>();


            foreach(string word in wordList)
            {
                 System.Diagnostics.Debug.Assert(word.Length == 5);

                if (word.Length != 5)
                    continue;      

                foreach(char letter in word)
                {
                    if (lookup.ContainsKey(letter))
                    {
                        lookup[letter] = lookup[letter] + 1;
                    }
                    else
                    {
                        lookup.Add(letter, 1);
                    }
                }

                int dupes = 0;
                foreach(KeyValuePair<char, int> kv in lookup)
                {
                    if (kv.Value > 1)
                    {
                        dupes = 1;
                    }
                }

                if (dupes != 0)
                    doubles.Add(word);

                lookup.Clear();
            }

            return doubles;
        }

        static List<string> GetPluralsWithS(List<string> wordList)
        {
            var plurals = new List<string>();

            foreach(string word in wordList)
            {
                System.Diagnostics.Debug.Assert(word.Length == 5);

                if (word.Length != 5)
                {
                    Console.WriteLine($"Skipping {word}. Not 5 characters long.");
                    continue;
                }

                if (word.ToLower()[4] == 's')
                {
                    plurals.Add(word);
                }
            }

            return plurals;
        }

        static void PrintLetterFrequency(List<KeyValuePair<string, int>> letters)
        {
            Console.WriteLine($"Letters: {letters.Count}");
            foreach(KeyValuePair<string, int> kv in letters)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
        }

        static void PrintWorldList(List<string> list)
        {
            int count = 1;
            foreach (string word in list)
            {
                Console.Write($"{word} ");

                if (count % 20 == 0)
                {
                    Console.WriteLine();
                    count = 0;
                }

                count++;
            }

            Console.WriteLine();
        }

        static List<string> FindMostCommonWordsByLetter(List<KeyValuePair<string, int>> occurences, List<string> wordList, int matches)
        {
            var letters = new List<string>();
            var candidates = new List<string>();

            var firstFive = occurences.Take(5);
            foreach(KeyValuePair<string, int> kv in firstFive)
            {
                letters.Add(kv.Key.ToString());
            }

            // Look for any words containing all five letters
            int count = 0;
            foreach(string word in wordList)
            {
                count = 0;
                foreach(char letter in word)
                {
                    if (letters.Contains(letter.ToString()))
                        count++;
                }

                if (count >= matches)
                    candidates.Add(word);
            }

            return candidates;
        }

        static List<KeyValuePair<string, int>> LetterFrequency(List<string> wordList)
        {
            var letterCount = new Dictionary<string, int>();
            var found = new List<char>();
            var frequency = new List<string>();

            foreach(string word in wordList)
            {
                found.Clear();

                foreach(char letter in word)
                {
                    if (found.Contains(letter))
                        continue;

                    found.Add(letter);
                    if (letterCount.ContainsKey(letter.ToString()))
                    {
                        letterCount[letter.ToString()] = letterCount[letter.ToString()] + 1;
                    }
                    else
                    {
                        letterCount.Add(letter.ToString(), 1);
                    }
                }
            }

            var sortedList = letterCount.OrderByDescending(x => x.Value).ToList();
            foreach(KeyValuePair<string, int> kv in sortedList)
            {
                LettersByFrequency.Add(kv.Key);
            }
            return sortedList;
        }

        private static List<string> ReadWordList(string path)
        {
            var list = new List<string>();

            if (!File.Exists(path))
            {
                Console.WriteLine($"Could not find file: {path}");
                return list;
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
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading word library: {ex.Message}");
            }

            return list;
        }
    }
}
