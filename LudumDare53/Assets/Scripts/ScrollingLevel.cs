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
    
    List<Tile> m_tilePool = new List<Tile>();
    public List<Tile> ActiveTiles { get; } = new List<Tile>();

    public override void Awake()
    {
        base.Awake();

        m_tilePool.AddRange(GetComponentsInChildren<Tile>());

        foreach(Tile tile in m_tilePool)
            tile.Deactivate();

        for(int i = 0; i < m_numActiveTiles; ++i)
            AddTileToEnd(i > 1);
    }

    public void ReparentWaves(Transform parent)
    {
        foreach(Tile tile in m_tilePool)
            tile.m_waveOffset.SetParent(parent);

        foreach(Tile tile in ActiveTiles)
            tile.m_waveOffset.SetParent(parent);
    }

    void AddTileToEnd(bool activateWave)
    {
        int index = Random.Range(0, m_tilePool.Count);
        Tile tile = m_tilePool[index];

        if(ActiveTiles.Count == 0)
            tile.transform.position = (Vector3.forward * (m_wrapAroundZ + m_spacing));
        else
            tile.transform.position = ActiveTiles.Last().transform.position + (Vector3.forward * m_spacing);

        tile.Activate(activateWave);

        ActiveTiles.Add(tile);
        m_tilePool.RemoveAt(index);
        
    }

    public void Tick()
    {
        Vector3 shift = Vector3.back * Time.deltaTime * m_speed;
        Camera camera = Camera.main;
        Vector3 screenScale = new Vector3(Screen.width, Screen.height, 1);

        foreach(var tile in ActiveTiles)
        {
            tile.transform.position += shift;
        }

        if(ActiveTiles[0].transform.position.z < m_wrapAroundZ)
        {
            AddTileToEnd(true);

            ActiveTiles[0].Deactivate();
            
            m_tilePool.Add(ActiveTiles[0]);
            ActiveTiles.RemoveAt(0);
        }

        foreach(var tile in ActiveTiles)
        {            
            Vector3 tileVpPos = camera.WorldToViewportPoint(tile.transform.position);
            tile.m_waveOffset.position = Vector3.Scale(tileVpPos, screenScale);
        }
    }
}
