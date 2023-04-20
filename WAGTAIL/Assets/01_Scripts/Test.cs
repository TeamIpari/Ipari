using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Animator anim;
    public Transform rpos;
    public Transform lpos;
    public GameObject onhands;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


       
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("AA");
        anim.SetIKPosition(AvatarIKGoal.RightHand, onhands.transform.position);
    }
}
