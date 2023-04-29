using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingLevel : MonoBehaviour
{
    public float m_speed = 1;
    public float m_spacing = 10;
    public float m_wrapAroundZ;
    
    List<Transform> m_tiles = new List<Transform>();

    void Awake()
    {
        foreach(Transform child in transform)
            m_tiles.Add(child);

        for(int i = 0; i < m_tiles.Count; ++i)
            m_tiles[i].position = Vector3.forward * i * m_spacing;
    }

    void Update()
    {
        Vector3 shift = Vector3.back * Time.deltaTime * m_speed;

        foreach(var tile in m_tiles)
        {
            tile.position += shift;

            if(tile.position.z < m_wrapAroundZ)
                tile.position += Vector3.forward * m_tiles.Count * m_spacing;            
        }
    } 
}
