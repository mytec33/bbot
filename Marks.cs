using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlebot
{
    internal class Marks
    {
        public int[] marks = new int[5] { 0, 0, 0, 0, 0 };

        public bool IsOnlyBlanks()
        {
            foreach (int mark in marks)
            {
                if (mark != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOnlyHints()
        {
            foreach (int mark in marks)
            {
                if (mark == 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
