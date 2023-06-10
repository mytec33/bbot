using System.Diagnostics;

namespace Wordlebot
{
    public class Marks
    {
        public const int MARK_MATCH = 2;
        public const int MARK_HINT = 1;

        public int[] marks;

        public Marks()
        {
            ResetMarks();
        }

        private void ResetMarks()
        {
            marks = new int[5] { 0, 0, 0, 0, 0 };
        }

        public int GetTileScore(int index)
        {
            Debug.Assert(index < 5 && index >= 0);

            if (index >= 0 && index < 5)
            {
                return marks[index];
            }

            Environment.Exit(2);
            return -99;
        }

        public bool NoMisses()
        {
            int count = 0;
            foreach (int mark in marks)
            {
                if (mark == MARK_MATCH || mark == MARK_HINT)
                {
                    count++;
                }
            }

            return count == 5 ? true : false;
        }

        public string TileScore(int score) =>
            score switch
            {
                0 => "miss",
                1 => "hint",
                2 => "match",
                3 => "unused match",
                < 0 => "error",
                > 3 => "error"
            };

        static int WordleHasLetterHint(char letter, string wordle)
        {
            for (int x = 0; x < 5; x++)
            {
                if (wordle[x] == letter)
                {
                    return x;
                }
            }

            return -1;
        }

        public void ScoreWord(string guess, string wordle)
        {
            var requiredLetters = new List<char>();

            ResetMarks();

            // Establish matches first so we can later determine if multiple occurences
            // of a letter are a hint or a miss based on what is matched
            for (int x = 0; x < 5; x++)
            {
                // Look for exact matches
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                    requiredLetters.Add(guess[x]);
                }
            }

            // Once we establish matches we can see how many times a given
            // letter occurs and if it's a hint or a miss. E.g., 1 s is found
            // and there is only 1 s in the word, additional s would be a miss
            for (int x = 0; x < 5; x++)
            {
                // Look for hints this spot or to the right
                if (WordleHasLetterHint(guess[x], wordle) >= 0)
                {
                    if (requiredLetters.Contains(guess[x]))
                    {
                        int foundCount = 0;
                        int occurrences = 0;

                        // LINQ??
                        foreach (char c in requiredLetters)
                        {
                            if (c == guess[x])
                            {
                                foundCount++;
                            }
                        }

                        foreach (char c in wordle)
                        {
                            if (c == guess[x])
                            {
                                occurrences++;
                            }
                        }

                        if (marks[x] == 2)
                        {
                            Console.WriteLine($"\tLetter '{guess[x]}' exact match");
                        }
                        else if (foundCount == occurrences && marks[x] == 2)
                        {
                            Console.WriteLine($"\tLetter '{guess[x]}' exact match");
                        }
                        else if (foundCount < occurrences && marks[x] != 2)
                        {
                            Console.WriteLine($"\tLetter '{guess[x]}' hint match");
                            marks[x] = 1;
                        }
                        else
                        {
                            Console.WriteLine($"\tLetter '{guess[x]}' is a miss");
                            marks[x] = 3;
                        }

                        Console.WriteLine($"Occurances of '{guess[x]}' is {occurrences} / {foundCount}");
                    }
                    else
                        marks[x] = 1;
                }
            }
        }

        public void PrintMarks()
        {
            string darkGrayBlock = "\u001b[90m█\u001b[0m";
            string greenBlock = "\u001b[32m█\u001b[0m";
            string yellowBlock = "\u001b[33;1m█\u001b[0m";

            foreach(int m in marks)
            {
                if (m == 1)
                {
                    Console.Write($"{yellowBlock} ");
                }
                else if (m == 2)
                {
                    Console.Write($"{greenBlock} ");
                }
                else
                {
                    Console.Write($"{darkGrayBlock} ");
                }
            }
            Console.WriteLine();

            foreach(int m in marks)
            {
                Console.Write($"{m} ");
            }
        }
    }
}
