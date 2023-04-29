using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollingLevel : Singleton<ScrollingLevel>
{
    public float m_speed = 1;
    public float m_spacing = 10;
    public float m_wrapAroundZ;
    public int m_numActiveTiles = 2;
    
    List<Transform> m_tilePool = new List<Transform>();
    List<Transform> m_activeTiles = new List<Transform>();

    public override void Awake()
    {
        base.Awake();

        foreach(Transform child in transform)
        {
            m_tilePool.Add(child);
            child.gameObject.SetActive(false);
        }

        for(int i = 0; i < m_numActiveTiles; ++i)
            AddTileToEnd();
    }

    void AddTileToEnd()
    {
        int index = Random.Range(0, m_tilePool.Count);
        Transform tile = m_tilePool[index];

        if(m_activeTiles.Count == 0)
            tile.position = Vector3.zero;
        else
            tile.position = m_activeTiles.Last().position + (Vector3.forward * m_spacing);

        tile.gameObject.SetActive(true);

        m_activeTiles.Add(tile);
        m_tilePool.RemoveAt(index);
    }

    void Update()
    {
        Vector3 shift = Vector3.back * Time.deltaTime * m_speed;

        foreach(var tile in m_activeTiles)
        {
            tile.position += shift;
        }

        if(m_activeTiles[0].position.z < m_wrapAroundZ)
        {
            AddTileToEnd();

            m_activeTiles[0].gameObject.SetActive(false);
            
            m_tilePool.Add(m_activeTiles[0]);
            m_activeTiles.RemoveAt(0);
        }
    } 
}
