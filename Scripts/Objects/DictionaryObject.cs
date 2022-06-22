using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryObject : MonoBehaviour
{
    [Header("Initialization Variables")]
    [SerializeField] private string[] swappables;
    
    public string sentence;
    public Definition definition;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the definition, has to be inside a method so it's here
        if(swappables != null)
        {
            definition = new Definition(sentence, swappables);
        } else
        {
            definition = new Definition(sentence);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Probably will use this later so just keeping it here for now
    }

    #region PROPERTIES
    [Header("Properties")]
    [SerializeField] private string[][] solutions;
    
    /// <summary>
    /// Checks if the definition will alter the properties of the object
    /// </summary>
    /// <returns>
    /// true if the current combination of keywords can change the object's properties
    /// </returns>
    public bool Solved()
    {
        Dictionary<string, int> keywords = definition.GetKeywords();
        foreach(string[] solution in solutions)
        {
            bool solutionCorrect = true;
            for(int i = 0; i < solution.Length; i++)
            {
                if (keywords["solution"] != i)
                {
                    solutionCorrect = false;
                }
            }
            if (solutionCorrect)
                return solutionCorrect;
        }
        return false;
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
    public string SwapWord(string target, string newWord)
    {
        return definition.Swap(target, newWord);
    }
    

    private void AlterProperties()
    {
        if(Solved())
        {
            //Do something based off properties

        }
    }
    #endregion
}
