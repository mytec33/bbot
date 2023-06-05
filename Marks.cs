using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlebot
{
    internal class Marks
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
                < 0 => "error",
                > 2 => "error"
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

            for (int x = 0; x < 5; x++)
            {
                // Look for exact matches
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                    Console.WriteLine($"\tLetter '{guess[x]}' exact match");
                    requiredLetters.Add(guess[x]);

                    continue;
                }

                // Look for hints this spot or to the right
                if (WordleHasLetterHint(guess[x], wordle) >= 0)
                {
                    Console.WriteLine($"\tLetter '{guess[x]}' hint match");

                    if (requiredLetters.Contains(guess[x]))
                        marks[x] = 3;
                    else
                        marks[x] = 1;
                }
            }
        }

        public void PrintMarks()
        {
            Console.WriteLine(string.Join(" ", marks));
        }
    }
}
