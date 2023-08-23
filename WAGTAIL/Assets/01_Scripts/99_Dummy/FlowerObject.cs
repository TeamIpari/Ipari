using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

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
            _obj.AddComponent<Rigidbody>().velocity = CaculateVelocity(t, FlowerTransform.position, FlightTIme);
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
    private Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        // define the distance x and y first;
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
        distanceXZ.y = 0f; // y는 0으로 설정.

        // Create a float the represent our distance
        float Sy = distance.y;      // 세로 높이의 거리를 지정.
        float Sxz = distanceXZ.magnitude;

        // 속도 추가
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
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
