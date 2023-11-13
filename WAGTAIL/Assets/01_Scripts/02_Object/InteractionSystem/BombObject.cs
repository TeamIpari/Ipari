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
    private Rigidbody _rb;
    private Vector3 _vDir;
    
    // Test property // 
    private float force = 13f;
    [SerializeField] private bool _isStart;
    private bool _isExplosionVFXNotNull;
    private const float PushTime = 0.075f;

    // Start is called before the first frame update
    private void Start()
    {
        _isExplosionVFXNotNull = explosionVFX != null;
        _color = GetComponent<Renderer>();
        _baseColor = _color.material.color;
        _currentTime = 0;
        _rb = GetComponent<Rigidbody>();
        _isStart = false;
    }

    private void Update()
    {
        if (_isStart)
        {
            StartCoroutine(StartTimeBomb(explosionTime));
        }
        if (_rb.velocity.y == 0f)
            _rb.velocity = Vector3.zero;

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
        // ÆøÅº ÅÍÁö´Â »ç¿îµå
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_BombBurst);


        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (enabled == false) return;

        if (other.gameObject.CompareTag("Platform"))
        {
            StartCoroutine(StartTimeBomb(explosionTime));
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            if (_isExplosionVFXNotNull)
            {
                other.gameObject.GetComponent<Enemy>().Hit();
                var tf = transform;
                var exploVFX = Instantiate(explosionVFX, tf.position, tf.rotation);
                FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_BombBurst);
                Destroy(exploVFX, 3);
            }
            // enable·Î ¹Ù²Ù»ï
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Shatter"))
        {
            if (_isExplosionVFXNotNull)
            {
                other.gameObject.GetComponent<ShatterObject>().Explode();
                var tf = transform;
                var exploVFX = Instantiate(explosionVFX, tf.position, tf.rotation);
                FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_BombBurst);
                Destroy(exploVFX, 3);
            }
            Destroy(gameObject);
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
