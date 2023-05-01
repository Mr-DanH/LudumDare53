using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollingLevel : Singleton<ScrollingLevel>
{
    public float m_speed = 1;
    public float m_spacing = 10;
    public float m_wrapAroundZ;
    public int m_numActiveTiles = 3;
    public float m_waveOffsetYOffset;
    public float m_waveOffsetZOffset;
    
    List<Tile> m_tilePool = new List<Tile>();
    public List<Tile> ActiveTiles { get; } = new List<Tile>();

    int m_cityTiles;
    int m_cityTilesLeft;

    GameScreen.LevelData m_levelData;

    void Awake()
    {
        foreach(var tile in GetComponentsInChildren<Tile>())
        {
            m_tilePool.Add(tile);
            tile.Init();  
            tile.Deactivate();
        }

        for(int i = 0; i < m_numActiveTiles - 1; ++i)
            AddTileToEnd(Tile.eType.Wasteland);
            
        AddTileToEnd(Tile.eType.WastelandToCity);
    }

    public void Reset()
    {
        foreach(var activeTile in ActiveTiles)
        {
            activeTile.Deactivate();       
            m_tilePool.Add(activeTile);
        }
        ActiveTiles.Clear();
        
        for(int i = 0; i < m_numActiveTiles; ++i)
            AddTileToEnd(Tile.eType.Wasteland);

        m_cityTiles = 0;
        m_cityTilesLeft = 0;
    }

    public void StartCity(int tiles, GameScreen.LevelData levelData)
    {
        m_cityTiles = tiles;
        m_cityTilesLeft = tiles;
        m_levelData = levelData;

        foreach(var tile in m_tilePool)
            tile.CacheMaterialIndex(levelData.m_materials);
            
        foreach(var tile in ActiveTiles)
            tile.CacheMaterialIndex(levelData.m_materials);
    }

    public void ReparentWaves(Transform parent)
    {
        foreach(Tile tile in m_tilePool)
        {
            if(tile.m_waveOffset)
            {
                tile.m_waveOffset.SetParent(parent);
                tile.m_waveOffset.localScale = Vector3.one;
            }
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
            if(tile.m_type == type && index-- == 0)
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

        tile.Activate(type == Tile.eType.City, m_levelData != null ? m_levelData.m_materials : null);

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
            Vector3 tileVpPos = camera.WorldToViewportPoint(tile.transform.position + new Vector3(0, m_waveOffsetYOffset, m_waveOffsetZOffset));
            if(tile.m_waveOffset != null)
                tile.m_waveOffset.position = Vector3.Scale(tileVpPos, screenScale);
        }
    }

    public bool IsLevelComplete()
    {
        return m_cityTilesLeft == 0 && ActiveTiles.TrueForAll(a => a.m_type != Tile.eType.City);
    }

    public float GetProgressThroughLevel()
    {
        float lastZ;

        if(m_cityTilesLeft > 0)
        {
            lastZ = ActiveTiles.Last().transform.position.z + (m_cityTilesLeft * m_spacing);
        }
        else
        {
            lastZ = ActiveTiles.FindLast(a => a.m_type == Tile.eType.City).transform.position.z;
        }

        float frontZ = lastZ - ((m_cityTiles + 1) * m_spacing);

        return Mathf.InverseLerp(frontZ, lastZ, m_wrapAroundZ);
    }
}
