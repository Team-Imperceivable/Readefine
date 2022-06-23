using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer normalButtonSprite;
    [SerializeField] private SpriteRenderer buttonPressedSprite;
    [SerializeField] private Bounds checkBounds;
    [SerializeField] private LayerMask pressableLayers;
    public bool pressed { get; private set; }


    void Start()
    {
        normalButtonSprite.enabled = true;
        buttonPressedSprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        pressed = checkPressed();
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
