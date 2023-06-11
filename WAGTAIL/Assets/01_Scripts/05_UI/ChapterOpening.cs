using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChapterOpening : MonoBehaviour
{
    [SerializeField] private float _fadeoutTime;
    private float _currentTime;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;

        if( _currentTime >= _fadeoutTime )
        {
            _currentTime = 0;
            _animator.SetTrigger("fadeout");
        }
    }

    public void DestroyUI()
    {
        gameObject.SetActive(false);
    }
}
