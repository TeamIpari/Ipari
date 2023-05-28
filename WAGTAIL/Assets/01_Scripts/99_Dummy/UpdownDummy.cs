using System.Collections;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class UpdownDummy : MonoBehaviour
{
    // 주기적으로 오브젝트를 올려주는 오브젝트
    // 최저점
    [SerializeField] private float down_yPos;
    // 최고점 
    [SerializeField] private float up_yPos;

    [Range(0, 1)]
    public float Speed = 0f;

    public float MoveTimer;
    public float CurTime;
    public float StopTimer;
    public bool IsUp = false;
    public bool IsStop = false;


    // Start is called before the first frame update
    private void Start()
    {
        //StartCoroutine(StageUp());
    }

    private bool Stoptimer(ref float t)
    {
        if(t > StopTimer)
        {
            t = 0;
            return IsStop = false;
        }
        //stop = false;
        return true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        CurTime += Time.deltaTime;
        if(IsStop && Stoptimer(ref CurTime))
        {
            return;
        }

        if (CurTime > MoveTimer)
        {
            PlatformMove();
            CurTime = 0;
        }
        

    }

    public void PlatformMove()
    {
        if(IsUp)
        {
            Up();
        }
        else
        {
            Down();
        }
    }
    
    public void Up()
    {
        transform.position = transform.position + Vector3.up * Speed;
        if (transform.position.y >= up_yPos)
        {
            IsUp = false;
            IsStop = true;
        }
    }

    public void Down()
    {
        transform.position -= Vector3.up * Speed;
        if(transform.position.y <= down_yPos)
        {
            IsUp = true;
            IsStop = true;
        }
    }

    private IEnumerator StageUp()
    {
        while(this.transform.position.y < up_yPos)
        {
            transform.position = transform.position + Vector3.up * Speed;
            Debug.Log(transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StageDown());    

    }


    private IEnumerator StageDown()
    {

        while (this.transform.position.y > down_yPos)
        {
            transform.position -= Vector3.up * Speed;
            Debug.Log(transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StageUp());

    }


}
