using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**********************************************************
 *    ������ �԰� ��ȣ�ۿ��ϴ� ����� ������ ������Ʈ�Դϴ�...
 * ******/
public sealed class WhaleHorn : MonoBehaviour, IInteractable
{
    //========================================================
    //////            Property and fields               //////
    //========================================================
    public string  InteractionPrompt   { get; set; } = "ȹ���ϱ�";
    public Vector3 InteractPopupOffset { get; set; } = (Vector3.up*1.5f);
    public bool    Gettable            { get { return _Gettable; } set { _Gettable = value; _moveDurationDiv = (1f / MoveDuration); } }

    [SerializeField] public GameObject     ShineSFXPrefab;
    [SerializeField] public AnimationCurve MoveCurve;
    [SerializeField] private float         MoveDuration;
    [SerializeField] private bool          _Gettable       = false;


    private float _currTime        = 0f;
    private float _moveDurationDiv = 1f;



    //======================================================
    ////////              Magic methods             ////////
    //======================================================
    private void Update()
    {
        #region Omit
        if (Gettable == false) return;

        /****************************************
         *    ���� ��鸰��.....
         * ********/
        float progressRatio = (_currTime += Time.deltaTime) * _moveDurationDiv;


        #endregion
    }



    //==================================================
    //////             Public methods              //////
    //==================================================
    public bool AnimEvent()
    {
        //No Implements
        return false;
    }

    public bool Interact(GameObject interactor)
    {
        #region Omit
        if (!interactor.CompareTag("Player") || Gettable)
            return false;

        Gettable            = false;
        InteractPopupOffset = (Vector3.up*99999f);

        return false;
        #endregion
    }

}
