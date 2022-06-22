using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionarySpellbook
{
    string[] words;
    /// <summary>
    /// Creates a DictionarySpellbook object
    /// </summary>
    /// <param name="spellbookSize">
    /// The number of slots the spellbook has
    /// </param>
    public DictionarySpellbook(int spellbookSize)
    {
        words = new string[spellbookSize];
        for(int i = 0; i < spellbookSize; i++)
        {
            words[i] = "EMPTY";
        }
    }

    /// <summary>
    /// Returns the words in the spellbook in string array form
    /// </summary>
    /// <returns>
    /// The words in the spellbook
    /// </returns>
    public string[] GetSpellbook()
    {
        return words;
    }


    /// <summary>
    /// Adds a word to the spellbook
    /// </summary>
    /// <param name="word">
    /// Word to be added to the spellbook
    /// </param>
    /// <returns>
    /// If the word was succesfully added to the spellbook
    /// </returns>
    public bool AddWord(string word)
    {
        for(int i = 0; i < words.Length; i++)
        {
            if (words[i].Equals("EMPTY"))
            {
                words[i] = word;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes a word from the spellbook
    /// </summary>
    /// <param name="word">
    /// Word to be removed from the spellbook
    /// </param>
    /// <returns>
    /// If the word was successfully removed from the spellbook
    /// </returns>
    public bool RemoveWord(string word)
    {
        for(int i = 0; i < words.Length; i++)
        {
            if (words[i].Equals(word))
            {
                words[i] = "EMPTY";
                return true;
            }
        }
        return false;
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
    public string SwapWord(string word)
    {
        for(int i = 0; i < words.Length; i++)
        {
            if (words[i].Equals(word))
            {
                string oldWord = words[i];
                words[i] = word;
                return oldWord;
            }
        }
        return null;
    }

    /// <summary>
    /// Changes the size of the spellbook
    /// </summary>
    /// <param name="newSize">
    /// The new number of slots the spellbook has
    /// </param>
    public void SetSize(int newSize)
    {
        string[] wordsCopy = new string[newSize];
        if (newSize > words.Length)
        {
            for (int i = 0; i < words.Length; i++)
            {
                wordsCopy[i] = words[i];
            }
            words = wordsCopy;
        } else
        {
            for(int i = 0; i < wordsCopy.Length; i++)
            {
                wordsCopy[i] = words[i];
            }
        }
    }
}
