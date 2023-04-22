using System;
using System.Linq;

namespace MyApp
{
    internal class Program
    {
        static Dictionary<string, int> Letters = new();
        static List<string> LettersByFrequency = new();
        static List<string> WordList = new();


        static void Main(string[] args)
        {
            string? wordle;
            var usedLetters = new List<string>();

            try
            {
                // Should load a user defined library of words
                StreamReader sr = new("./5_letter_words");

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    WordList.Add(line);
                }

                Console.WriteLine($"Words: {WordList.Count:#,##0}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error loading word library: {ex.Message}");
            }

            var letterFrequency = LetterFrequency(WordList);
            PrintLetterFrequency(letterFrequency);

            var wlNoPlural = NoPluralWords_S(WordList);
            var wlNoDouble = NoDoubleLetters(wlNoPlural);

            // TODO: Tweak
            List<string> firstWords = FindMostCommonWordsByLetter(letterFrequency, wlNoDouble, 5);
            Console.WriteLine("First words:");
            Console.Write($"{string.Join(", ", firstWords)}\n");

            Console.Write("Enter the wordle of the day:");
            wordle = GetTodaysWordle();

            int attempts = 1;
            string? guess = "";
            int[] marks = new int[5] { 0, 0, 0, 0, 0 };
            List<string> newList = NoPluralWords_S(WordList);
            while(attempts < 7)
            {
                if (attempts == 1)
                    guess = "tread"; //firstWords[0];
                else
                {
                    Console.WriteLine("Mixed marks");

                    newList = RemoveWordsByMark(marks, guess, newList);

                    Console.WriteLine($"Found {newList.Count} potential words");
                    Console.Write($"{string.Join(", ", newList)}\n");

                    // TODO: if more than one item, this should have some heuristic rather
                    // than picking first.
                    guess = newList[0];    
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

        static List<string> RemoveWordsByMark(int[] marks, string guess, List<string> list)
        {
            var newList = new List<string>(list);
            Console.WriteLine($"Words in list: {newList.Count}");

            for (int x = 0; x < 5; x++)
            {
                if (marks[x] == 0)
                {
                    newList = RemoveWordsWithLetter(guess[x], newList);
                    Console.WriteLine($"\tRemoved words with letter '{guess[x]}'");
                    Console.WriteLine($"\tWords in list: {newList.Count}");
                }
                else if (marks[x] == 1)
                {
                    newList = RemoveWordsWithLetterInPlace(guess[x], x, newList);
                    Console.WriteLine($"\tRemoved words with letter '{guess[x]}' in index {x + 1}");
                    Console.WriteLine($"\tWords in list: {newList.Count}");                
                }
                else if (marks[x] == 2)
                {
                    newList = RemoveWordsWithoutLetterInPlace(guess[x], x, newList);
                    Console.WriteLine($"\tRemoved words missing letter '{guess[x]}' in index {x + 1}");
                    Console.WriteLine($"\tWords in list: {newList.Count}");                
                }
                else
                {
                    Console.WriteLine($"\t\tUnknown mark!! {marks[x]}");
                }                
            }

            return newList;            
        }

        static List<string> RemoveWordsWithLetterInPlace(char letter, int index, List<string> list)
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

        static List<string> RemoveWordsWithoutLetterInPlace(char letter, int index, List<string> list)
        {
            var deletes = new List<string>();

            foreach(string word in list)
            {
                if (word[index] != letter)
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

        static List<string> RemoveWordsWithLetter(char letter, List<string> list)
        {
            var deletes = new List<string>();

            foreach(string word in list)
            {
                foreach(char curLetter in word)
                {
                    if (curLetter == letter)
                    {
                        deletes.Add(word);
                        continue;
                    }
                }
            }

            foreach(string word in deletes)
            {
                list.Remove(word);
            }

            return list;
        }

        static int[] ScoreWord(string guess, string wordle)
        {
            int[] marks = new int[5] { 0, 0, 0, 0, 0 };
            var matches = new Dictionary<char, int>();

            // Matches first, then hints
            for (int x = 0; x < 5; x++)
            {
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                }

                // Keep track of all characters for hinting
                if (matches.ContainsKey(guess[x]))
                {
                    matches[guess[x]] = matches[guess[x]] + 1;
                }
                else
                {
                    matches.Add(guess[x], 1);
                }                
            }

            // Look for hints
            Console.WriteLine("Looking for hints");
            for (int x = 0; x < 5; x++)
            {
                // if we find a match, we skip as these are taken care of
                if (guess[x] == wordle[x])
                {
                    Console.WriteLine("\tFound exact match. Skipping.");
                    continue;
                }
                
                if (WordleContainsLetter(wordle, guess[x]))
                {
                    Console.WriteLine($"\tPotential hint: '{guess[x]}'");
                    if (matches[guess[x]] >= 1)
                    {
                        if (GuessIsWordleMatch(marks, wordle, guess[x]))
                        {
                            // We have a dupe of a letter that is already found
                            // and there is only one of these words so we don't
                            // hit but mark to not remove words with this letter
                            marks[x] = 3;
                        }
                        else
                        {
                            marks[x] = 1;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"\tNot found: '{guess[x]}'");
                }
            }

            return marks;
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

        static bool GuessIsWordleMatch(int[] marks, string wordle, char search)
        {
            for (int x = 0; x < 5; x++)
            {
                if (wordle[x] == search)
                {
                    if (marks[x] == 2)
                        return true;
                }
            }

            return false;
        }

        static bool WordleContainsLetter(string wordle, char letter)
        {
            if (wordle.Contains(letter))
                return true;

            return false;
        }

        static List<string> NoDoubleLetters(List<string> wordList)
        {
            var lookup = new Dictionary<char, int>();
            var noDoubles = new List<string>();


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

                if (dupes == 0)
                    noDoubles.Add(word);

                lookup.Clear();
            }

            return noDoubles;
        }

        static List<string> NoPluralWords_S(List<string> wordList)
        {
            var nonPlural = new List<string>();

            foreach(string word in wordList)
            {
                System.Diagnostics.Debug.Assert(word.Length == 5);

                if (word.Length != 5)
                    continue;

                if (word.ToLower()[4] != 's')
                {
                    nonPlural.Add(word);
                }
            }

            return nonPlural;
        }

        static void PrintLetterFrequency(List<KeyValuePair<string, int>> letters)
        {
            Console.WriteLine($"Letters: {letters.Count}");
            foreach(KeyValuePair<string, int> kv in letters)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
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

            // Refine. Remove words with double occurances of any given letter.
            // TODO: 

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
    }
}
