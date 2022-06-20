using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definition
{
    Dictionary<string, int> swapables;
    string sentence;
    int nextSwapNumber;
    public Definition(string _sentence)
    {
        sentence = _sentence;
        nextSwapNumber = 0;
    }

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

    public string GetDefinition()
    {
        string definition = sentence;
        foreach(KeyValuePair<string, int> kvp in swapables)
        {
            definition.Replace($"[{kvp.Value}]", kvp.Key);
        }
        return definition;
    }

    public void Swap(string target, string newValue)
    {
        int targetValue = swapables[target];
        swapables.Remove(target);
        swapables.Add(newValue, targetValue);
    }

    public void SetSwappable(string word)
    {
        sentence.Replace(word, $"[{nextSwapNumber}]");
        swapables.Add(word, nextSwapNumber);
        nextSwapNumber++;
    }
}
