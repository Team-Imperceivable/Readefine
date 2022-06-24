using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetKeywordWhilePressed : MonoBehaviour
{
    [SerializeField] private ButtonScript buttonScript;
    [SerializeField] private DictionaryObject target;
    [SerializeField] private string storedWord;
    private bool alreadySwapped = false;

    private void Update()
    {
        if (buttonScript.pressed && !alreadySwapped)
        {
            alreadySwapped = true;
            storedWord = target.SwapWord(storedWord);
        } else
        {
            if(alreadySwapped && !buttonScript.pressed)
            {
                alreadySwapped = false;
                storedWord = target.SwapWord(storedWord);
            }
        }
    }
}
