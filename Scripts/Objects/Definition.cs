using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definition
{
    Dictionary<string, int> swapables;
    string sentence;
    int nextSwapNumber;

    /// <summary>
    /// Creates a Definition object
    /// </summary>
    /// <param name="_sentence">
    /// The full sentence of the definition
    /// </param>
    public Definition(string _sentence)
    {
        sentence = _sentence;
        nextSwapNumber = 0;
    }

    /// <summary>
    /// Creates a Definition object
    /// </summary>
    /// <param name="_sentence">
    /// The full sentence of the definition
    /// </param>
    /// <param name="swapableWords">
    /// The words that are swappable
    /// </param>
    public Definition(string _sentence, string[] swapableWords)
    {
        sentence = _sentence;
        for(int i = 0; i < swapableWords.Length; i++)
        {
            sentence.Replace(swapableWords[i], $"[{i}]");
            swapables.Add(swapableWords[i], i);
        }
        nextSwapNumber = swapableWords.Length;
    }

    /// <summary>
    /// Gets the definition in pure string form
    /// </summary>
    /// <returns>
    /// The definition in string form
    /// </returns>
    //TODO: Change so it only returns an array of the keywords
    public string GetDefinition()
    {
        string definition = sentence;
        foreach(KeyValuePair<string, int> kvp in swapables)
        {
            definition.Replace($"[{kvp.Value}]", kvp.Key);
        }
        return definition;
    }

    /// <summary>
    /// Returns the dictionary of swapable words.
    /// </summary>
    /// <returns>
    /// The dictionary of swapable words
    /// </returns>
    public Dictionary<string, int> GetKeywords()
    {
        return swapables;
    }

    /// <summary>
    /// Swaps a word in the definition, throws an error if the target or new value aren't in the defintiion.
    /// </summary>
    /// <param name="target">
    /// Word that is getting swapped out
    /// </param>
    /// <param name="newValue">
    /// Word that is getting swapped in
    /// </param>
    /// <returns>
    /// The word that is getting swapped out
    /// </returns>
    public string Swap(string target, string newValue)
    {
        int targetValue = swapables[target];
        swapables.Remove(target);
        swapables.Add(newValue, targetValue);
        return target;
    }

    /// <summary>
    /// Sets a word in the definition to be swappable
    /// </summary>
    /// <param name="word">
    /// The word that will now be swappable.
    /// </param>
    public void SetSwappable(string word)
    {
        sentence.Replace(word, $"[{nextSwapNumber}]");
        swapables.Add(word, nextSwapNumber);
        nextSwapNumber++;
    }
}
