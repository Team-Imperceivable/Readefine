using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckClicked : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string clickedMethodName;
    [SerializeField] private Color hoverColor;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioClip clickEffect;
    private Image text;
    private Color normalColor;

    void Start()
    {
        text = gameObject.GetComponent<Image>();
        normalColor = text.color;
        clickAudio.clip = clickEffect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickAudio.Play();
        SendMessageUpwards(clickedMethodName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}
