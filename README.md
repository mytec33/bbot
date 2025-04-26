# BBOT

This is a C# implementation of a Wordle guessing program. For a period of roughly 160 games, this bot scored slightly better than the NY Times bot when I was tracking the results.

That said, there is probably much room for improvement.

# Improvements / Notes

## SortListByMisses()

Unsure how to address. With a word like `clash` and the starting word of `arose`, both 'a' and 's' are part of the final word.
The 's' is in the correct spot and the 'a' is part of the word but not in it's final spot.

This function looks at the remaining letters not yet guessed and sorts them by frequency. An attempt is then made to find candidate words which have the most high frequency occurances.

```
Found 25 potential words:
daisy swash waist class palsy clasp patsy gnash clash glass flask sassy quasi stash salsa quash balsa gassy pansy blast slash flash chasm smash spasm 
```

```
Letters and their frequency:
        l 11
        h 9
        y 6
        p 5
        t 4
        c 4
        i 3
        g 3
        m 3
        w 2
        n 2
        f 2
```

Three words are found: clash, slash, and flash.

Interestingly, class is one of the remaining 25 words and not found by this algorithm because the 's' in particular is already matched and not included in the above list. 

If found letters would included the list would be:

```
        s 25
        a 25
        l 11
        h 9
        y 6
        ...
```

Unsure how to address. The letters are already part of the remaining words, and the intent is to find the highest number of remaining (unmatched) letters in the remaining words to help cut down the search space.

# TODO

