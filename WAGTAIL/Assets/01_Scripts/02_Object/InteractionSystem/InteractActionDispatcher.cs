using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*******************************************************
 *    단순하게 상호작용 기능을 구현할 수 있는 컴포넌트입니다.
 * *****/
[RequireComponent(typeof(BoxCollider))]
public sealed class InteractActionDispatcher : MonoBehaviour, IInteractable
{
    #region Define
    [System.Serializable]
    public sealed class InteractableEvent : UnityEvent
    {
    }
    #endregion

    //================================================
    ////////             Property             ////////
    //================================================
    public string  InteractionPrompt    { get { return _InteractPrompt; } set{ if(value!=null) _InteractPrompt = value; } }
    public Vector3 InteractPopupOffset  { get { return (!IsInteractable? (Vector3.up*99999f):_InteractPopupOffset); } set { _InteractPopupOffset = value; } }

    [SerializeField] public bool              IsInteractable   = false;
    [SerializeField] private string           _InteractPrompt = "None";
    [SerializeField] private Vector3          _InteractPopupOffset;
    [SerializeField] public InteractableEvent OnInteract;

    private BoxCollider _collider;


    //=================================================
    ///////            Magic methods            ///////
    //=================================================
    private void Start()
    {
        #region Omit
        gameObject.tag   = "interactable";
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        _collider = gameObject.GetComponent<BoxCollider>();
        #endregion
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        #region Omit
        /************************************
         *  화면에 Trigger 범위를 출력한다...
         * **/
        if (_collider == null) _collider = GetComponent<BoxCollider>();
        if (_collider != null)
        {
            Color color = Color.yellow;
            color.a = .5f; 
            Bounds bounds = _collider.bounds;
            Matrix4x4 mat = Matrix4x4.Translate(bounds.center - transform.position) * transform.localToWorldMatrix;
            Vector3 size = _collider.size;

            Gizmos.color = color;
            Gizmos.matrix = mat;
            Gizmos.DrawCube(Vector3.zero, size);

            Gizmos.color = Color.black;
            Gizmos.matrix = mat;
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
        #endregion
    }
#endif

    //=======================================================
    /////////          Override methods              ////////
    //=======================================================
    public bool AnimEvent()
    {
        //None Implements...
        return false;
    }

    public bool Interact(GameObject interactor)
    {
        #region Omit
        if (!IsInteractable) return false;

        OnInteract?.Invoke();
        return false;
        #endregion
    }
}
