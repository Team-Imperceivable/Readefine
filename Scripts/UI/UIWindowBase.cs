using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIWindowBase : MonoBehaviour, IDragHandler, IPointerClickHandler
{
	public PlayerController player;
	public DictionaryObject parent;

	RectTransform m_transform = null;
	
	// Use this for initialization
	void Start () {
		m_transform = GetComponent<RectTransform>();
	}

	/// <summary>
	/// Drags the window
	/// </summary>
	/// <param name="eventData">
	/// Mouse event data
	/// </param>
	public void OnDrag(PointerEventData eventData)
	{
		m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
		
		// magic : add zone clamping if's here.
	}

	public void OnPointerClick(PointerEventData eventData)
    {
		//Swap words
		if(eventData.button == PointerEventData.InputButton.Right)
        {
			
			string parentKeyword = parent.SwapWord(player.spellbook.GetWord());
			player.spellbook.SetWord(parentKeyword);
			Debug.Log("Spellbook: " + player.spellbook.GetWord());
			Debug.Log("Object: " + parent.swappable);
			parent.UpdateText();
			player.UpdateSpellbook();
        }
    }
}