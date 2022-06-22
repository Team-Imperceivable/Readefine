using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryObject : MonoBehaviour
{
    [Header("Initialization Variables")]
    [SerializeField] public string swappable;
    
    public string sentence;
    public Definition definition;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the definition, has to be inside a method so it's here
        definition = new Definition(sentence, swappable);

        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        //Probably will use this later so just keeping it here for now
    }

    #region PROPERTIES
    [Header("Properties")]
    [SerializeField] private string[] solutions;
    [SerializeField] private Text definitionTextBox;
    /// <summary>
    /// Checks if the definition will alter the properties of the object
    /// </summary>
    /// <returns>
    /// true if the current combination of keywords can change the object's properties
    /// </returns>
    public bool Solved()
    {
        foreach(string solution in solutions)
        {
            if(solution.Equals(swappable))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateText()
    {
        definition.SetSwappable(swappable);
        definitionTextBox.text = definition.GetDefinition();
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
    public string SwapWord(string newWord)
    {
        swappable = newWord;
        return definition.Swap(swappable);
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
