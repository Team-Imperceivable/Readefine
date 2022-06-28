using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer normalButtonSprite;
    [SerializeField] private SpriteRenderer buttonPressedSprite;
    [SerializeField] private Bounds checkBounds;
    [SerializeField] private LayerMask pressableLayers;
    [SerializeField] private AudioClip buttonEffect;
    public bool pressed { get; private set; }

    private AudioSource audioSource;

    void Start()
    {
        normalButtonSprite.enabled = true;
        buttonPressedSprite.enabled = false;
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = buttonEffect;
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPressed())
        {
            if(!pressed)
            {
                audioSource.Play();
            }
            pressed = true;
        } else
        {
            pressed = false;
        }
        
        UpdateSprite();
    }

    private bool checkPressed()
    {
        return Physics2D.OverlapBox(transform.position + checkBounds.center, checkBounds.size, 0f, pressableLayers) != null;
    }

    private void UpdateSprite()
    {
        if (pressed)
        {
            buttonPressedSprite.enabled = true;
            normalButtonSprite.enabled = false;
        } else
        {
            buttonPressedSprite.enabled = false;
            normalButtonSprite.enabled = true;
        }
            
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + checkBounds.center, checkBounds.size);
    }
}
