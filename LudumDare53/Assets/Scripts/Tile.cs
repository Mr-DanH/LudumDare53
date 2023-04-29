using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform m_waveOffset;

    public void Activate(bool activateWave)
    {
        foreach(Transform child in m_waveOffset)
            child.gameObject.SetActive(activateWave);
            
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        foreach(Transform child in m_waveOffset)
            child.gameObject.SetActive(false);
            
        gameObject.SetActive(false);
    }
}
