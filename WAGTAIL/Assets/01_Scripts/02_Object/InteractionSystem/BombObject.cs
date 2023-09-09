using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BombObject : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionTime;
    [SerializeField] private GameObject explosionVFX;

    private readonly Collider[] _colliders = new Collider[10];
    private Color _baseColor;
    private Renderer _color;
    private float _currentTime;
    private CharacterController _cc;
    private Vector3 _vDir;
    
    // Test property // 
    private float force = 13f;
    private bool _isStart;
    private bool _isExplosionVFXNotNull;
    private const float PushTime = 0.075f;

    // Start is called before the first frame update
    private void Start()
    {
        _isExplosionVFXNotNull = explosionVFX != null;
        _color = GetComponent<Renderer>();
        _baseColor = _color.material.color;
        _currentTime = 0;
        _isStart = false;
        //StartCoroutine(StartTimeBomb(explosionTime));
    }
    
    private IEnumerator StartTimeBomb(float time)
    {
        float currentTime = time;
        float totalTime = 0f;
        StartCoroutine(Bomb());
        while (totalTime < time)
        {
            _color.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _color.material.color = _baseColor;
            yield return new WaitForSeconds(currentTime / time - 0.2f);
            totalTime += currentTime / time;
            currentTime -= currentTime / time;
        }
    }

    private IEnumerator Bomb()
    {
        yield return new WaitForSeconds(5f);
        StopCoroutine(nameof(StartTimeBomb));
        Explosion();
    }
    
    public void Explosion()
    {
        var tf = transform;
        var size = Physics.OverlapSphereNonAlloc(tf.position, explosionRange, _colliders);
        Debug.Log(size);
        if (size != 0)
        {
            for (var i = 0; i < size; i++)
            {
                if (_colliders[i].CompareTag("Player"))
                    _colliders[i].GetComponent<Player>().isDead = true;
                
                else if (_colliders[i].CompareTag("Platform"))
                    _colliders[i].GetComponent<IEnviroment>().ExecutionFunction(0.0f);
            }
        }
        
        if (_isExplosionVFXNotNull )         
        {
            var exploVFX = Instantiate(explosionVFX, tf.position, tf.rotation);
            Destroy(exploVFX, 3);
        }
        Destroy(gameObject);


        /*_cc = _colliders[0].GetComponent<CharacterController>();
        CalcDirectionVector(_colliders[0].transform.position, transform.position);
        _currentTime = 0f;
        _isStart = true;*/
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            StartCoroutine(StartTimeBomb(explosionTime));
        }
    }

    private void CalcDirectionVector(Vector3 targetPos, Vector3 currentPos)
    {
        _vDir = (targetPos - currentPos).normalized;
        _vDir.y = 0;
    }

    private void MovePlayer()
    {
        if (_currentTime <= PushTime)
        {
            _currentTime += Time.deltaTime;
            _cc.Move((_vDir * (_currentTime * force)));
        }
        
        else
        {
            _isStart = false;
        }
    }    
}
