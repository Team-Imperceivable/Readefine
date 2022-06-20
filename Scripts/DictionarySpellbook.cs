using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionarySpellbook
{
    string[] words;
    public DictionarySpellbook(int spellbookSize)
    {
        words = new string[spellbookSize];
        for(int i = 0; i < spellbookSize; i++)
        {
            words[i] = "EMPTY";
        }
    }

    public string[] GetSpellbook()
    {
        return words;
    }

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
