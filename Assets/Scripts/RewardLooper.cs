using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardLooper : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName = "RewardPose";

    void OnEnable()
    {
        // crossfade into your looped state
        animator.CrossFade(stateName, 0f);
    }
}
