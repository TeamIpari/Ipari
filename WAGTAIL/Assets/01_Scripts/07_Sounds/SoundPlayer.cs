using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip deathClip;

    private SoundHandler sndHdr;

    private void Start()
    {
        sndHdr = GetComponent<SoundHandler>();
        
        sndHdr.RegisterBool("isWalk");
        sndHdr.RegisterTrigger("isJump");
        sndHdr.RegisterTrigger("isLanding");
        sndHdr.RegisterTrigger("isDeath");

        sndHdr.Bind("isWalk", walkClip);
        sndHdr.Bind("isJump", jumpClip);
        sndHdr.Bind("isLanding", landingClip);
        sndHdr.Bind("isDeath", deathClip);
    }
}
