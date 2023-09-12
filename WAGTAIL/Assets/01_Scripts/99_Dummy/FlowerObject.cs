using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using IPariUtility;

/// <summary>
/// 터지고 Object를 생성하여 포물선 궤적으로 던져줘야함.
/// </summary>

public class FlowerObject : MonoBehaviour
{
    public GameObject CoinPrefab;
    public Transform FlowerTransform;
    public List<Vector3> targets;
    public float pointRadian = 3f;        // 초기 값 3;
    public int InitCount = 5;
    public float FlightTIme = 2;
    public bool IsDance = false;
    [SerializeField] private GameObject _explosionVFX;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("Dance", IsDance);
    }

    public void CreatePoint()
    {
        for(int i = 0; i < InitCount; i++)
            targets.Add(Search());
        
        foreach(var t in targets)
        {
            GameObject _obj = GameObject.Instantiate(CoinPrefab);

            _obj.transform.rotation = Quaternion.Euler(90, 0, 0);
            _obj.transform.position = FlowerTransform.position;
            _obj.transform.position += Vector3.up * 1.5f;
            _obj.AddComponent<Rigidbody>().velocity = IpariUtility.CaculateVelocity(t, FlowerTransform.position, FlightTIme);
            _obj.GetComponent<ScoreObject>().SetTime(FlightTIme);
            //marker.Add(_obj);
        }

        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Flowers_Burst);
        //SoundTest.GetInstance().PlaySound("isCoinFlowerExplo");
        if (_explosionVFX != null )         
        {           
            GameObject exploVFX = Instantiate(_explosionVFX, transform.position, transform.rotation);
            Destroy(exploVFX, 7);
        }
        Destroy(this.gameObject);
    }

    private Vector3 Search()
    {
        // Random.onUnitSphere : 반경 1을 갖는 구의 표면상에서 임의의 지점을 반환함
        Vector3 getPoint = Random.onUnitSphere;
        getPoint.y = 0.1f;

        // 0.0f 부터 지정한 반지름의 길이 사이의 랜덤 값을 산출함.
        float r = Random.Range(0.0f, pointRadian);
        Vector3 vec = (getPoint * r) + FlowerTransform.position;

        return new Vector3(vec.x, 0.1f, vec.z);
    }
}
