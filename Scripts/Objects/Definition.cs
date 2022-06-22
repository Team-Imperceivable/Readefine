using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definition
{
    string swappable;
    string sentence;

    /// <summary>
    /// Creates a Definition object
    /// </summary>
    /// <param name="_sentence">
    /// The full sentence of the definition
    /// </param>
    public Definition(string _sentence, string _swappable)
    {
        sentence = _sentence;
        swappable = _swappable;
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
        return sentence.Replace(swappable, $"<b>{swappable}</b>");
    }

    public string GetKeyword()
    {
        return swappable;
    }

    public string Swap(string word)
    {
        remakeSentence(word);
        string swappableCopy = swappable;
        swappable = word;
        return swappableCopy;
    }

    public void SetSwappable(string word)
    {
        remakeSentence(word);
        swappable = word;
    }

    private void remakeSentence(string replacement)
    {
        sentence = sentence.Replace(swappable, replacement);
    }
}
