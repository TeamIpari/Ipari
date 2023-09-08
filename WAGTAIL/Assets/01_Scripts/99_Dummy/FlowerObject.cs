using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// ������ Object�� �����Ͽ� ������ �������� ���������.
/// </summary>

public class FlowerObject : MonoBehaviour
{
    public GameObject CoinPrefab;
    public Transform FlowerTransform;
    public List<Vector3> targets;
    public float pointRadian = 3f;        // �ʱ� �� 3;
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
        Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
        distanceXZ.y = 0f; // y�� 0���� ����.

        // Create a float the represent our distance
        float Sy = distance.y;      // ���� ������ �Ÿ��� ����.
        float Sxz = distanceXZ.magnitude;

        // �ӵ� �߰�
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    } 
    private Vector3 Search()
    {
        // Random.onUnitSphere : �ݰ� 1�� ���� ���� ǥ��󿡼� ������ ������ ��ȯ��
        Vector3 getPoint = Random.onUnitSphere;
        getPoint.y = 0.1f;

        // 0.0f ���� ������ �������� ���� ������ ���� ���� ������.
        float r = Random.Range(0.0f, pointRadian);
        Vector3 vec = (getPoint * r) + FlowerTransform.position;

        return new Vector3(vec.x, 0.1f, vec.z);
    }
}
