using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Bounds followBounds;
    [Header("Targets")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float minChangeSpeed;
    [SerializeField] private float maxChangeSpeed;

    private Vector3 lastPos;
    private float changeAmount;

    private void Start()
    {
        lastPos = playerTransform.position;
        changeAmount = (maxChangeSpeed - minChangeSpeed) / (float) targets.Count;
    }

    private void Update()
    {
        MoveCamera();

        MoveTargets();

        lastPos = transform.position;
    }

    private void MoveCamera()
    {
        transform.position = followBounds.ClosestPoint(playerTransform.position);
    }

    private void MoveTargets()
    {
        Vector3 delta = GetDelta();
        for(int i = 0; i < targets.Count; i++)
        {
            Vector3 changeVector = delta;
            changeVector.x *= (i + 1) * changeAmount;
            changeVector.y *= (targets.Count - i) * changeAmount;
            changeVector.y = -changeVector.y;
            targets[i].position += changeVector;
        }
    }

    private Vector3 GetDelta()
    {
        return transform.position - lastPos;
    }

    private void OnGizmosDraw()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(followBounds.center, followBounds.size);
    }
}
