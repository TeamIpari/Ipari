using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public sealed class UIPointerEmulator : MonoBehaviour
{
    public enum PointerEventType
    {
        None,
        Pointer_Enter,
        Pointer_Click,
        Pointer_Move,
        Pointer_Exit
    }

    //=======================================================
    //////           Property and fields               //////
    //======================================================
    [SerializeField] TMP_Dropdown DropDown;
    
    private PointerEventData _newEven;
    private int              _index = 1;



    //================================================
    //////            Magic methods             //////
    //================================================
    private void Start()
    {
        #region Omit
        /*******************************************************
         *   마우스 동작을 흉내내는데 방해되는 일부조작을 제거한다...
         * ***/
        EventSystem system = EventSystem.current;
        InputSystemUIInputModule module;
        if(system!=null && (module=system.GetComponent<InputSystemUIInputModule>()))
        {
            module.move   = null;
            module.submit = null;
            module.cancel = null;
        }

        #endregion
    }
    


    //============================================
    /////            Core methods           /////
    //============================================

    /****************************************
     *    TMP DropDown 관련 메소드.....
     * ****/
    public void ShowDropDown()
    {
        #region Omit
        if (DropDown == null) return;
        _index = (DropDown.value+1);
        DropDown.Show();
        #endregion
    }

    public void HideDropDown()
    {
        #region Omit
        if (DropDown == null) return;
        DropDown.Hide();
        DropDown.RefreshShownValue();
        #endregion
    }

    private void InvokeEventDropDownItems(int target, PointerEventType targetEvent, PointerEventType otherEvent = PointerEventType.None)
    {
        #region Omit
        if (DropDown == null || DropDown.IsExpanded == false) return;

        /***************************************************
         *    대상을 찾아낸다....
         * ****/
        Transform list      = DropDown.transform.Find("Dropdown List");
        Transform viewport  = list.Find("Viewport");
        Transform content   = viewport.Find("Content");
        int Count           = (content.childCount - 1);

        _index = ClampRepeat(target, 1, Count);

        Debug.Log(_index);
        for (int i = 1; i <= Count; i++)
        {
            GameObject obj = content.GetChild(i).gameObject;

            if (_index != i) InvokeEvent(otherEvent, obj);
            else InvokeEvent(targetEvent, obj);
        }

        #endregion
    }

    public void MoveDropDownList(int movIndex)
    {
        InvokeEventDropDownItems(_index + movIndex, PointerEventType.Pointer_Enter, PointerEventType.Pointer_Exit);
    }

    public void SelectDropDownList()
    {
        InvokeEventDropDownItems(_index, PointerEventType.Pointer_Click);
    }

    public void ChangeDropDown(GameObject other)
    {
        #region Omit
        TMP_Dropdown otherDropDown = other.GetComponent<TMP_Dropdown>();    
        if(otherDropDown!=null)
        {
            HideDropDown();
            DropDown = otherDropDown;
        }
        #endregion
    }


    /********************************************
     *   기본 제공 메소드....
     * *****/
    public void InvokeEvent(PointerEventType eventType, GameObject target)
    {
        #region Omit

        /**이벤트가 존재하지 않는다면 생성한다...*/
        if (_newEven == null){

            _newEven = new PointerEventData(EventSystem.current);
        }

        switch (eventType){

            case (PointerEventType.None):
                break;

            case (PointerEventType.Pointer_Move):
                ExecuteEvents.Execute(target, _newEven, ExecuteEvents.pointerMoveHandler);
                break;

            case (PointerEventType.Pointer_Enter):
                ExecuteEvents.Execute(target, _newEven, ExecuteEvents.pointerEnterHandler);
                break;

            case (PointerEventType.Pointer_Exit):
                ExecuteEvents.Execute(target, _newEven, ExecuteEvents.pointerExitHandler);
                break;

            case (PointerEventType.Pointer_Click):
                ExecuteEvents.Execute(target, _newEven, ExecuteEvents.pointerClickHandler);
                break;
        }
        #endregion
    }

    public void InvokeEnterEvent(GameObject target)
    {
        InvokeEvent(PointerEventType.Pointer_Enter, target);
    }

    public void InvokeExitEvent(GameObject target)
    {
        InvokeEvent(PointerEventType.Pointer_Exit, target);
    }

    public void InvokeClickEvent(GameObject target)
    {
        InvokeEvent(PointerEventType.Pointer_Click, target);
    }

    public void InvokeMoveEvent(GameObject target)
    {
        InvokeEvent(PointerEventType.Pointer_Move, target);
    }

    public int ClampRepeat(int value, int min, int max)
    {
        #region Omit
        if (value < min) return max;
        else if (value > max) return min;

        return value;
        #endregion
    }
}
