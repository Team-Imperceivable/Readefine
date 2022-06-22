using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionarySpellbook
{
    string word;
    public DictionarySpellbook(string startingWord)
    {
        word = startingWord;
    }

    /// <summary>
    /// Returns the words in the spellbook in string array form
    /// </summary>
    /// <returns>
    /// The words in the spellbook
    /// </returns>
    public string GetWord()
    {
        return word;
    }

    public void SetWord(string addedWord)
    {
        word = addedWord;        
    }

    public string RemoveWord()
    {
        string wordCopy = word;
        word = null;
        return wordCopy;
    }

    /// <summary>
    /// Swaps a word in the spellbook for a new one
    /// </summary>
    /// <param name="word">
    /// The word to be swapped in
    /// </param>
    /// <returns>
    /// The word that was swapped out
    /// </returns>
    public string SwapWord(string newWord)
    {
        string wordCopy = word;
        word = newWord;
        return wordCopy;
    }
}
