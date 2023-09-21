using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InteractionUIManager : MonoBehaviour
{
    private bool _isActive;
    private Animator _animator;
    private Player _player;
    private static readonly int Fadein = Animator.StringToHash("fadein");
    private static readonly int Fadeout = Animator.StringToHash("fadeout");

    //============================================
    /////          property and Felds          /////
    //============================================
    private Transform hasTarget;
    
    //============================================
    /////          Show Inspector Info          /////
    //============================================
    [SerializeField] private float OutDistance = 10f;
    [SerializeField] private Color32 DefualtColor;
    [SerializeField] private Color32 TargetColor;
    [SerializeField] private Image[] IconImages;
    
    private void Start()
    {
        _player = Player.Instance;
        _isActive = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.speed = 0f;
    }

    private void Update()
    {
        /*
        if(hasTarget == null)
            CheckDistance();
        else*/
        if(hasTarget != null)
            FadeInteractableIconColor();

        if ( _player.isCarry || _player.isPull )
        {
            if(_isActive)
            {
                _isActive = false;
                _animator.SetTrigger(Fadeout);
            }
        }
    }

    //==================================================
    /////               core methods                /////
    //==================================================
    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * (OutDistance * 2));
    }
*/
    // 원의 범위 내로 플레이어가 들어오면 체크를 함.
    private void CheckDistance()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, new Vector3(1, 1, 1) * OutDistance,Quaternion.identity);
        foreach(var col in cols)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                hasTarget = col.gameObject.transform;
                OnActionUI();
                return;
            }
        }
    }

    // hasTarget을 얻으면 거리를 지속적으로 체크하면서 색을 바꿔줌.
    private void FadeInteractableIconColor()
    {
        // A to B의 거리를 체크 
        float distance = Vector3.Distance(this.transform.position, hasTarget.transform.position);
        if(distance > OutDistance + 1)
        {
            hasTarget = null;
            OnActionUI();
        }
        // 최대 거리로부터 현재 위치가 얼마나 떨어졌는지 체크.
        float percent = (OutDistance - distance) / OutDistance;
        foreach(var img in IconImages)
        {
            img.color = Color32.Lerp(DefualtColor, TargetColor, percent);
        }
    }

    private void OnActionUI()
    {
        if (!_isActive)
        {
            _isActive = true;
            if (_animator.speed > 0f)
            {
                _animator.SetTrigger(Fadein);
            }
            _animator.speed = 1.0f;
        }
        else if (_isActive)
        {
            _isActive = false;
            _animator.SetTrigger(Fadeout);
        }
    }


    //==================================================
    /////
    //==================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            hasTarget = other.gameObject.transform;
            if (!_isActive)
            {
                _isActive = true;
                if (_animator.speed > 0f)
                {
                    _animator.SetTrigger(Fadein);
                }
                _animator.speed = 1.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (_isActive)
            {
                _isActive = false;
                _animator.SetTrigger(Fadeout);
            }
        }
    }
}
