using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAnimator : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float transitionDistance;
    private Animator animator;
    private bool transitioned = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.Play("mirror");
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, target.position) > transitionDistance)
        {
            if(transitioned)
            {
                animator.Play("reverse-transition");
                transitioned = false;
            }
        } else
        {
            if(!transitioned)
            {
                transitioned = true;
                animator.Play("transition");
            }
        }
    }
}
