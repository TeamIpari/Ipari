using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoCoSi : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    public int index;
    
    private CoCoSiManager _coCoSiManager;

    private void Start()
    {
        _coCoSiManager = GetComponentInParent<CoCoSiManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Get_Flower);
        SpawnVFX();
        _coCoSiManager.cocosiToggle[index - 1].isOn = true;
        gameObject.SetActive(false);
    }

    private void SpawnVFX()
    {
        if (_interactionVFX != null)
        {
            GameObject exploVFX = Instantiate(_interactionVFX, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(exploVFX, 2);
        }

        else
            Debug.LogWarning("InteractionVFX was missing!");
    }
}
