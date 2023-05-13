using System.Collections;
using UnityEngine;

public class UpdownDummy : MonoBehaviour
{
    // 주기적으로 오브젝트를 올려주는 오브젝트
    // 최저점
    [SerializeField] float down_yPos;
    // 최고점 
    [SerializeField] float up_yPos;

    [Range(0, 1)]
    public float speed = 0f;

    public float moveTimer;
    public float curTime;
    public float stopTimer;
    public bool up = false;
    public bool stop = false;


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StageUp());
    }

    bool Stoptimer(ref float _t)
    {
        if(_t > stopTimer)
        {
            _t = 0;
            return stop = false;
        }
        //stop = false;
        return true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        curTime += Time.deltaTime;
        if(stop && Stoptimer(ref curTime))
        {
            return;
        }

        if (curTime > moveTimer)
        {
            PlatformMove();
            curTime = 0;
        }
        

    }

    public void PlatformMove()
    {
        if(up)
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
        transform.position = transform.position + Vector3.up * speed;
        if (transform.position.y >= up_yPos)
        {
            up = false;
            stop = true;
        }
    }

    public void Down()
    {
        transform.position -= Vector3.up * speed;
        if(transform.position.y <= down_yPos)
        {
            up = true;
            stop = true;
        }
    }
    
    IEnumerator StageUp()
    {
        while(this.transform.position.y < up_yPos)
        {
            transform.position = transform.position + Vector3.up * speed;
            Debug.Log(transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StageDown());    

    }


    IEnumerator StageDown()
    {

        while (this.transform.position.y > down_yPos)
        {
            transform.position -= Vector3.up * speed;
            Debug.Log(transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(StageUp());

    }


}
