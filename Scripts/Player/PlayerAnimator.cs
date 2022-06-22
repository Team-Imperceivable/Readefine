using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public AnimationState state;
    private Animator animator;

    void Start()
    {
        state = AnimationState.Idle;
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case AnimationState.Idle:
                animator.Play("MC_idle");
                break;
            case AnimationState.Walk:
                animator.Play("MC_walk");
                break;
            case AnimationState.Push:
                animator.Play("MC_push");
                break;
            case AnimationState.Swim:
                animator.Play("MC_swim");
                break;
            case AnimationState.Climb:
                animator.Play("MC_climb");
                break;
        }
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", Mathf.Abs(speed));
    }
}

public enum AnimationState
{
    Idle,
    Walk,
    Push,
    Swim,
    Climb
}
