namespace Wordlebot.Tests
{
    public class RemoveWordsWitLetterData : TheoryData<char, List<string>, List<string>>
    {
        public RemoveWordsWitLetterData()
        {
            // Remove none
            Add('a', new List<string> { "below", "clerk", "ghoul" }, new List<string> { "below", "clerk", "ghoul" });

            // Remove all
            Add('z', new List<string> { "jazzy", "plaza" }, new List<string> { });

            // Remove one
            Add('e', new List<string> { "least", "guppy" }, new List<string> { "guppy" });

            // Remove two
            Add('t', new List<string> { "ditto", "scout", "ounce" }, new List<string> { "ounce" });
        }
    }

    public class RemoveWordsWithLetterByIndexData : TheoryData<int, char, List<string>, List<string>>
    {
        public RemoveWordsWithLetterByIndexData()
        {
            // Remove None
            Add(0, 't', new List<string> { "shorn", "skimp" }, new List<string> { "shorn", "skimp" });
            Add(1, 't', new List<string> { "shorn", "skimp" }, new List<string> { "shorn", "skimp" });
            Add(2, 't', new List<string> { "shorn", "skimp" }, new List<string> { "shorn", "skimp" });
            Add(3, 't', new List<string> { "shorn", "skimp" }, new List<string> { "shorn", "skimp" });
            Add(4, 't', new List<string> { "shorn", "skimp" }, new List<string> { "shorn", "skimp" });

            // Remove All
            Add(0, 's', new List<string> { "slate", "smash", "scone" }, new List<string> { });
            Add(1, 'o', new List<string> { "loves", "power", "vowel" }, new List<string> { });
            Add(2, 'e', new List<string> { "plead", "speed", "greed" }, new List<string> { });
            Add(3, 'p', new List<string> { "guppy", "slope", "trope" }, new List<string> { });
            Add(4, 'y', new List<string> { "sappy", "happy", "pappy" }, new List<string> { });

            // Remove One
            Add(0, 'a', new List<string> { "aglow", "mired" }, new List<string> { "mired" });
            Add(1, 'a', new List<string> { "paint", "skate" }, new List<string> { "skate" });
            Add(2, 'a', new List<string> { "scant", "humid" }, new List<string> { "humid" });
            Add(3, 'a', new List<string> { "cedar", "skate" }, new List<string> { "skate" });
            Add(4, 'a', new List<string> { "balsa", "below" }, new List<string> { "below" });

            // Remove Two
            Add(0, 's', new List<string> { "swine", "sulky", "guppy" }, new List<string> { "guppy" });
            Add(1, 's', new List<string> { "issue", "usual", "guppy" }, new List<string> { "guppy" });
            Add(2, 's', new List<string> { "casts", "laser", "guppy" }, new List<string> { "guppy" });
            Add(3, 's', new List<string> { "least", "crest", "guppy" }, new List<string> { "guppy" });
            Add(4, 's', new List<string> { "booms", "acids", "guppy" }, new List<string> { "guppy" });
        }
    }

    public class RemoveWordsWithoutLetterData : TheoryData<char, List<string>, List<string>>
    {
        public RemoveWordsWithoutLetterData()
        {
            // Remove none
            Add('a', new List<string> { "match", "catch", "latch" }, new List<string> { "match", "catch", "latch" });

            // Remove all
            Add('z', new List<string> { "bends", "canny" }, new List<string> { });

            // Remove one
            Add('e', new List<string> { "least", "guppy" }, new List<string> { "least" });

            // Remove two
            Add('t', new List<string> { "ditto", "arose", "ounce" }, new List<string> { "ditto" });
        }
    }

    public class RemoveWordsWithoutLetterByIndexData : TheoryData<int, char, List<string>, List<string>>
    {
        public RemoveWordsWithoutLetterByIndexData()
        {
            // Remove None
            Add(0, 's', new List<string> { "slate", "smash", "scone" }, new List<string> { "slate", "smash", "scone" });
            Add(1, 'o', new List<string> { "loves", "power", "vowel" }, new List<string> { "loves", "power", "vowel" });
            Add(2, 'e', new List<string> { "plead", "speed", "greed" }, new List<string> { "plead", "speed", "greed" });
            Add(3, 'p', new List<string> { "guppy", "slope", "trope" }, new List<string> { "guppy", "slope", "trope" });
            Add(4, 'y', new List<string> { "sappy", "happy", "pappy" }, new List<string> { "sappy", "happy", "pappy" });

            // Remove All
            Add(0, 't', new List<string> { "shorn", "skimp" }, new List<string> { });
            Add(1, 't', new List<string> { "shorn", "skimp" }, new List<string> { });
            Add(2, 't', new List<string> { "shorn", "skimp" }, new List<string> { });
            Add(3, 't', new List<string> { "shorn", "skimp" }, new List<string> { });
            Add(4, 't', new List<string> { "shorn", "skimp" }, new List<string> { });

            // Remove One
            Add(0, 'a', new List<string> { "aglow", "mired" }, new List<string> { "aglow" });
            Add(1, 'a', new List<string> { "paint", "skate" }, new List<string> { "paint" });
            Add(2, 'a', new List<string> { "scant", "humid" }, new List<string> { "scant" });
            Add(3, 'a', new List<string> { "cedar", "skate" }, new List<string> { "cedar" });
            Add(4, 'a', new List<string> { "balsa", "below" }, new List<string> { "balsa" });

            // Remove Two
            Add(0, 't', new List<string> { "toast", "utter", "guppy" }, new List<string> { "toast" });
            Add(1, 't', new List<string> { "state", "table", "guppy" }, new List<string> { "state" });
            Add(2, 't', new List<string> { "latte", "table", "guppy" }, new List<string> { "latte" });
            Add(3, 't', new List<string> { "ditto", "trunk", "guppy" }, new List<string> { "ditto" });
            Add(4, 't', new List<string> { "beast", "latte", "guppy" }, new List<string> { "beast" });
        }
    }
}