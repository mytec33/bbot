namespace Wordlebot.Tests
{
    public class GetTileScoreTestData : TheoryData<int, string, string, int>
    {
        public GetTileScoreTestData()
        {
            // No matches
            Add(0, "slate", "crumb", 0);
            Add(1, "slate", "crumb", 0);
            Add(2, "slate", "crumb", 0);
            Add(3, "slate", "crumb", 0);
            Add(4, "slate", "crumb", 0);

            // All hints
            Add(0, "slate", "tales", 1);
            Add(1, "slate", "tales", 1);
            Add(2, "slate", "tales", 1);
            Add(3, "slate", "tales", 1);
            Add(4, "slate", "tales", 1);

            // All matches
            Add(0, "crane", "crane", 2);
            Add(1, "crane", "crane", 2);
            Add(2, "crane", "crane", 2);
            Add(3, "crane", "crane", 2);
            Add(4, "crane", "crane", 2);

            // Mixed hints and matches
            Add(0, "arose", "brass", 1);
            Add(1, "arose", "brass", 2);
            Add(2, "arose", "brass", 0);
            Add(3, "arose", "brass", 2);
            Add(4, "arose", "brass", 0);

            // Mixed with unused match
            Add(0, "ditto", "slate", 0);
            Add(1, "ditto", "slate", 0);
            Add(2, "ditto", "slate", 4);
            Add(3, "ditto", "slate", 2);
            Add(4, "ditto", "slate", 0);
        }
    }

    public class ScoreWordTestData : TheoryData<string, string, string>
    {
        public ScoreWordTestData()
        {
            // No match
            Add("arose", "climb", "00000");
            Add("arose", "guppy", "00000");
            Add("least", "broom", "00000");
            Add("slate", "broom", "00000");

            // All hints
            Add("slate", "tales", "11111");
            Add("steal", "least", "11111");

            // All matches
            Add("again", "again", "22222");
            Add("arose", "arose", "22222");
            Add("crane", "crane", "22222");
            Add("kneel", "kneel", "22222");
            Add("nanny", "nanny", "22222");
            
            // Mixed
            Add("arose", "bagel", "10001");
            Add("arose", "brass", "12020");
            Add("slate", "stale", "21212");
            
            // Mixed with unused match
            Add("ditto", "slate", "00420");

            // NY Times Scoring examples
            Add("piano", "again", "01210");
            Add("snail", "again", "01220");
            Add("churn", "again", "00002");
            Add("anglo", "aglow", "20111");
            Add("brick", "crumb", "12010");
            Add("chump", "crumb", "20220");
            Add("sonic", "ennui", "00210");
            Add("penni", "ennui", "01212");
            Add("promo", "broom", "02211");
            Add("tread", "guard", "01012");
            Add("award", "guard", "40222"); // unused match
            Add("least", "guard", "00200");
            Add("puppy", "guppy", "42222"); // unused match
            Add("scoup", "guppy", "00011");
            Add("caret", "hater", "02121");
            Add("clear", "hater", "00112");
            Add("stare", "hater", "01111");
            Add("tamer", "hater", "12022");
            Add("tread", "hater", "11110");
            // TODO: Bingo! This is the missing part of my scoring. Should be 00142 (unused hint)
            // NY Times scores as blank, blank, hint, blank, match
            Add("flood", "hound", "00112"); 
            Add("fleck", "kneel", "01201");
            Add("sheep", "kneel", "00220");
            Add("slate", "kneel", "01001");
            Add("found", "nanny", "00020");
            Add("manny", "nanny", "02222");
            Add("botch", "scout", "01110");
            Add("court", "scout", "11102");
            Add("scoot", "scout", "22242"); // unused match
            Add("stoic", "scout", "21201");
            Add("least", "whiff", "00000");
            // TODO: Same as "flood" -> "hound" This is the missing part of my scoring. Should be 01040 (unused hint)
            // NY Times scores as blank, hint, blank, blank, blank            
            Add("vivid", "whiff", "01010");
        }
    }
}