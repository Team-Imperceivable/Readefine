using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayPlatform : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerPos;
    // Update is called once per frame
    void Update()
    {
        platformCollider.enabled = !PlayerInside() && playerPos.position.y > transform.position.y + (2 * platformCollider.bounds.size.y);
    }


    private bool PlayerInside()
    {
        return Physics2D.OverlapBox(transform.position, platformCollider.bounds.size, 0f, playerLayer) != null;
    }
}
