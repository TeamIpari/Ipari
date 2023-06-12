using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundHandler))]
public class SoundTest : Singleton<SoundTest>
{
    /*
     * �׽�Ʈ Ű ����
     * 
     * H - Trigger
     * J - Bool
     * K - Bool ���� Bind �� �� ���ð� ���� �Ѱ��� ��
     * V - ��� ȿ���� �Ͻ�����
     * B - ��� ȿ���� �Ͻ����� ����
     * N - ��� ȿ���� ���� 0.1 ���̱�
     * M - ��� ȿ���� ���� 0.1 �ø���
     * 
     */
    [SerializeField] private AudioClip mushroomClip;
    [SerializeField] private AudioClip shakeClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip vineClip;
    [SerializeField] private AudioClip brokenWallClip;
    [SerializeField] private AudioClip throwLandClip;
    [SerializeField] private AudioClip throwWaterClip;
    [SerializeField] private AudioClip basketInteractClip;
    [SerializeField] private AudioClip flowerInteractClip;
    [SerializeField] private AudioClip coinInteractClip;
    [SerializeField] private AudioClip coinFlowerExplosionClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip deathUIClip;
    [SerializeField] private AudioClip waterFlowingClip;
    [SerializeField] private AudioClip titleClip;
    [SerializeField] private AudioClip inGameClip;
    [SerializeField] private AudioClip buttonClip;
    SoundHandler sndHdr;
    protected override void Awake()
    {
        base.Awake();
        sndHdr = GetComponent<SoundHandler>();

        // ���� ������ �� �ִ� ���´� Bool Ÿ�԰� Trigger Ÿ�� 2���� ����
        sndHdr.RegisterBool("isShake");
        sndHdr.RegisterBool("isShake2");
        sndHdr.RegisterTrigger("isShoot");

        // ���� �̸��� ���¸� �߰��ϴ� ���� ������
        // sndHdr.RegisterBool("isShoot");

        // ������ �̸��� Ư�� ���ڸ� �߰��ϴ� ���� �������� ������ '/' �� ���� �ȵ�.
        // sndHdr.RegisterBool("isShoot");

        // ������ �̸� ���� ����ؾ���
        SoundSetting setting = new SoundSetting();
        setting.volume = 1f;
        // trigger ���� ���� forceStop ���� �ȵ�
        setting.forceStop = true;

        sndHdr.Bind("isShake", shakeClip);
        sndHdr.Bind("isShoot", shootClip);

        // ������ �ִ� ��� �ڿ� �߰��ϸ� ��
        sndHdr.Bind("isShake2", shakeClip, setting);

        // ���� ���ǿ� �ٸ� clip �� ����ϴ� �� ����
        // sndHdr.Bind("isShoot", shakeClip);

        // AudioClip ������ �̸��� Ư�����ڰ� ���� ���� �������� ������ '/'�� ���� �ȵ�.
        sndHdr.RegisterTrigger("isMushroom");
        sndHdr.Bind("isMushroom", mushroomClip);
        sndHdr.RegisterTrigger("isVine");
        sndHdr.Bind("isVine", vineClip);
        sndHdr.RegisterTrigger("isBrokenWall");
        sndHdr.Bind("isBrokenWall", brokenWallClip);
        sndHdr.RegisterTrigger("isThrowLand");
        sndHdr.Bind("isThrowLand", throwLandClip);
        sndHdr.RegisterTrigger("isThrowWater");
        sndHdr.Bind("isThrowWater", throwWaterClip);
        sndHdr.RegisterTrigger("isBasketInteract");
        sndHdr.Bind("isBasketInteract", basketInteractClip);
        sndHdr.RegisterTrigger("isFlowerInteract");
        sndHdr.Bind("isFlowerInteract", flowerInteractClip);
        sndHdr.RegisterTrigger("isCoinInteract");
        sndHdr.Bind("isCoinInteract", coinInteractClip);
        sndHdr.RegisterTrigger("isCoinFlowerExplo");
        sndHdr.Bind("isCoinFlowerExplo", coinFlowerExplosionClip);
        sndHdr.RegisterTrigger("isDeathUI");
        sndHdr.Bind("isDeathUI", deathUIClip);
        sndHdr.RegisterTrigger("isButton");
        sndHdr.Bind("isButton", buttonClip);
        sndHdr.RegisterBool("isTitle");
        sndHdr.Bind("isTitle", titleClip,setting);
        sndHdr.RegisterBool("isInGame");
        sndHdr.Bind("isInGame", inGameClip,setting);
    }

    bool temp = false;

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            sndHdr.SetTrigger("isShoot");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            sndHdr.SetBool("isShake", true);
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            sndHdr.SetBool("isShake", false);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            sndHdr.SetBool("isShake2", true);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            sndHdr.SetBool("isShake2", false);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SoundManager.GetInstance().volume += -0.1f;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SoundManager.GetInstance().volume += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SoundManager.GetInstance().PauseAll();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SoundManager.GetInstance().ResumeAll();
        }
        // �� ���� �������� �ϳ��� ���带 ��Ʈ�� �ϴ� ���
        //
        // bool condit1 = true;
        // bool condit2 = true;
        // if (condit1 && condit2)
        //      sndHdr.SetBool("isShake", true);
    }*/

    public void PlaySound(string sound)
    {
        sndHdr.SetTrigger(sound);
    }

    public void PlayBGM(string sound, bool isPlay)
    {
        sndHdr.SetBool(sound, isPlay);
    }
}
