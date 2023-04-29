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
    public float m_waveOffsetYOffset;
    
    List<Tile> m_tilePool = new List<Tile>();
    List<Tile> m_outskirtTilePool = new List<Tile>();
    public List<Tile> ActiveTiles { get; } = new List<Tile>();

    int m_cityTilesLeft;

    void Awake()
    {
        foreach(var tile in GetComponentsInChildren<Tile>())
        {
            m_tilePool.Add(tile);                
            tile.Deactivate();
        }

        for(int i = 0; i < m_numActiveTiles - 1; ++i)
            AddTileToEnd(Tile.eType.Wasteland);
            
        AddTileToEnd(Tile.eType.WastelandToCity);
    }

    public void StartCity(int tiles)
    {
        m_cityTilesLeft = tiles;
    }

    public void ReparentWaves(Transform parent)
    {
        foreach(Tile tile in m_tilePool)
        {
            if(tile.m_waveOffset)
                tile.m_waveOffset.SetParent(parent);
        }
    }

    Tile GetTile(Tile.eType type)
    {
        int count = 0;

        foreach(var tile in m_tilePool)
        {
            if(tile.m_type == type)
                ++count;
        }

        int index = Random.Range(0, count);
        
        foreach(var tile in m_tilePool)
        {
            if(tile.m_type == type && --count == 0)
                return tile;
        }

        return null;
    }

    void AddTileToEnd(Tile.eType type)
    {
        int index = Random.Range(0, m_tilePool.Count);
        Tile tile = GetTile(type);

        if(ActiveTiles.Count == 0)
            tile.transform.position = (Vector3.forward * m_wrapAroundZ);
        else
            tile.transform.position = ActiveTiles.Last().transform.position + (Vector3.forward * m_spacing);

        tile.Activate(type == Tile.eType.City);

        ActiveTiles.Add(tile);
        m_tilePool.Remove(tile);
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
            
            m_tilePool.Add(tile);
            ActiveTiles.RemoveAt(0);

            if(m_cityTilesLeft > 0)
            {
                bool wasWasteland = ActiveTiles.Last().m_type == Tile.eType.Wasteland;
                AddTileToEnd(wasWasteland ? Tile.eType.WastelandToCity : Tile.eType.City);
                
                if(!wasWasteland)
                    --m_cityTilesLeft;
            }
            else
            {
                bool wasCity = ActiveTiles.Last().m_type == Tile.eType.City;
                AddTileToEnd(wasCity ? Tile.eType.CityToWasteland : Tile.eType.Wasteland);
            }
        }

        foreach(var tile in ActiveTiles)
        {            
            Vector3 tileVpPos = camera.WorldToViewportPoint(tile.transform.position + new Vector3(0, m_waveOffsetYOffset, 0));
            if(tile.m_waveOffset != null)
                tile.m_waveOffset.position = Vector3.Scale(tileVpPos, screenScale);
        }
    }

    public bool IsLevelComplete()
    {
        return m_cityTilesLeft == 0 && ActiveTiles.TrueForAll(a => a.m_type != Tile.eType.City);
    }
}
