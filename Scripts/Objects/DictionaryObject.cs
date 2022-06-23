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
    private Collider2D myCollider;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the definition, has to be inside a method so it's here
        definition = new Definition(sentence, swappable);
        myCollider = gameObject.GetComponent<Collider2D>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        //
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

    private bool PlayerInContact()
    {
        List<Collider2D> contacts = new List<Collider2D>();

        myCollider.GetContacts(contacts);

        foreach(Collider2D collider in contacts)
        {
            if(collider.tag.Equals("Player"))
            {
                return true;
            }
        }
        return false;
    }
    private Collider2D GetPlayerInContactCollider()
    {
        List<Collider2D> contacts = new List<Collider2D>();

        myCollider.GetContacts(contacts);

        foreach (Collider2D collider in contacts)
        {
            if (collider.tag.Equals("Player"))
            {
                return collider;
            }
        }
        return null;
    }
}

public enum ActiveKeyword
{
    None,
    Deadly,
    Moveable,
    Floating,
    Swimmable,
    Controllable,
    Passable,
    Climbable
}
