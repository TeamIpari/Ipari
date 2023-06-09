using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundHandler))]
public class SoundTest : MonoBehaviour
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

    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip shakeClip;

    SoundHandler sndHdr;
    void Start()
    {
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
        setting.volume = 0.5f;
        // trigger ���� ���� forceStop ���� �ȵ�
        setting.forceStop = true;

        sndHdr.Bind("isShake", shakeClip);
        sndHdr.Bind("isShoot", shootClip);

        // ������ �ִ� ��� �ڿ� �߰��ϸ� ��
        sndHdr.Bind("isShake2", shakeClip, setting);

        // ���� ���ǿ� �ٸ� clip �� ����ϴ� �� ����
        // sndHdr.Bind("isShoot", shakeClip);

        // AudioClip ������ �̸��� Ư�����ڰ� ���� ���� �������� ������ '/'�� ���� �ȵ�.
    }

    bool temp = false;

    void Update()
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
    }
}
