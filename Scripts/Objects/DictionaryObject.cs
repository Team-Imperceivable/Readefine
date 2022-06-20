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
        
    }

    #region PROPERTIES
    [Header("Properties")]
    [SerializeField] private string[] solutions; //solutions are any definitions that results in a change to the object's properties
    
    
    public bool Solved()
    {
        string def = definition.GetDefinition();
        foreach(string solution in solutions)
        {
            if (solution.Equals(def))
                return true;
        }
        return false;
    }
    
    public void SwapWord(string target, string newWord)
    {
        definition.Swap(target, newWord);
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
