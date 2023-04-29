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
    
    List<Tile> m_cityTilePool = new List<Tile>();
    List<Tile> m_outskirtTilePool = new List<Tile>();
    public List<Tile> ActiveTiles { get; } = new List<Tile>();

    int m_cityTilesLeft;

    public override void Awake()
    {
        base.Awake();

        foreach(var tile in GetComponentsInChildren<Tile>())
        {
            if(tile.m_isCity)
                m_cityTilePool.Add(tile);
            else
                m_outskirtTilePool.Add(tile);
                
            tile.Deactivate();
        }

        for(int i = 0; i < m_numActiveTiles; ++i)
            AddTileToEnd(false);
    }

    public void StartCity(int tiles)
    {
        m_cityTilesLeft = tiles;
    }

    public void ReparentWaves(Transform parent)
    {
        foreach(Tile tile in m_cityTilePool)
            tile.m_waveOffset.SetParent(parent);
    }

    void AddTileToEnd(bool isCityTile)
    {
        List<Tile> pool = isCityTile ? m_cityTilePool : m_outskirtTilePool;

        int index = Random.Range(0, pool.Count);
        Tile tile = pool[index];

        if(ActiveTiles.Count == 0)
            tile.transform.position = (Vector3.forward * m_wrapAroundZ);
        else
            tile.transform.position = ActiveTiles.Last().transform.position + (Vector3.forward * m_spacing);

        tile.Activate(isCityTile);

        ActiveTiles.Add(tile);
        pool.RemoveAt(index);
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
            Tile tile = ActiveTiles[0];
            tile.Deactivate();
            
            List<Tile> pool = tile.m_isCity ? m_cityTilePool : m_outskirtTilePool;
            pool.Add(tile);
            ActiveTiles.RemoveAt(0);

            AddTileToEnd(m_cityTilesLeft > 0);
            --m_cityTilesLeft;
        }

        foreach(var tile in ActiveTiles)
        {            
            Vector3 tileVpPos = camera.WorldToViewportPoint(tile.transform.position);
            if(tile.m_waveOffset != null)
                tile.m_waveOffset.position = Vector3.Scale(tileVpPos, screenScale);
        }
    }

    public bool IsLevelComplete()
    {
        return m_cityTilesLeft == 0 && ActiveTiles.TrueForAll(a => a.m_isCity);
    }
}
