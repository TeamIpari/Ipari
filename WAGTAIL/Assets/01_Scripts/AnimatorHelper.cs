using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class AnimatorHelper : MonoBehaviour
{
    public delegate void LateUpdateNotify();
    public LateUpdateNotify AnimatorLateUpdate;

    private void LateUpdate()
    {
        AnimatorLateUpdate?.Invoke();
    }

}
