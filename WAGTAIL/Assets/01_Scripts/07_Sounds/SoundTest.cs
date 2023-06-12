using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundHandler))]
public class SoundTest : Singleton<SoundTest>
{
    /*
     * 테스트 키 설명
     * 
     * H - Trigger
     * J - Bool
     * K - Bool 에다 Bind 할 때 세팅값 같이 넘겨준 것
     * V - 모든 효과음 일시정지
     * B - 모든 효과음 일시정지 해제
     * N - 모든 효과음 볼륨 0.1 줄이기
     * M - 모든 효과음 볼륨 0.1 늘리기
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

        // 현재 설정할 수 있는 상태는 Bool 타입과 Trigger 타입 2개가 있음
        sndHdr.RegisterBool("isShake");
        sndHdr.RegisterBool("isShake2");
        sndHdr.RegisterTrigger("isShoot");

        // 같은 이름의 상태를 추가하는 것은 금지됨
        // sndHdr.RegisterBool("isShoot");

        // 상태의 이름에 특수 문자를 추가하는 것은 권장하지 않으며 '/' 가 들어가면 안됨.
        // sndHdr.RegisterBool("isShoot");

        // 설정은 미리 만들어서 등록해야함
        SoundSetting setting = new SoundSetting();
        setting.volume = 1f;
        // trigger 같은 경우는 forceStop 적용 안됨
        setting.forceStop = true;

        sndHdr.Bind("isShake", shakeClip);
        sndHdr.Bind("isShoot", shootClip);

        // 설정이 있는 경우 뒤에 추가하면 됨
        sndHdr.Bind("isShake2", shakeClip, setting);

        // 같은 조건에 다른 clip 을 등록하는 것 가능
        // sndHdr.Bind("isShoot", shakeClip);

        // AudioClip 파일의 이름에 특수문자가 들어가는 것은 권장하지 않으며 '/'가 들어가면 안됨.
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
        // 두 개의 조건으로 하나의 사운드를 컨트롤 하는 경우
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
