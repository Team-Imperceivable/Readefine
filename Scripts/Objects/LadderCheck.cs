using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderCheck : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] PlayerController controller;
    [SerializeField] private float width;
    [SerializeField] private float height;
    private Bounds topBounds;

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, new Vector2(width, height), 0);
        if (collider != null && collider.tag.Equals("Player"))
        {
            controller.canClimb = true;
        }
        else
        {
            controller.canClimb = false;
        }
        Vector3 top = transform.position;
        top.y += height / 2f;
        topBounds = new Bounds(top, new Vector3(width, 0.25f, 0f));
        controller.topOfLadder = controller.FeetTouching(topBounds);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector2(width, height));
        Vector3 top = transform.position;
        top.y += height / 2f;
        Gizmos.DrawWireCube(top, new Vector2(width, 0.25f));
    }
}
