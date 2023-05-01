using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum eType
    {
        City,
        CityToWasteland,
        Wasteland,
        WastelandToCity,
        Boss,
        BossCooldown
    }

    public eType m_type;
    public Transform m_waveOffset;

    public List<Tower> m_possibleTargetTowers = new List<Tower>();
    
    public List<Tower> TargetTowers { get; } = new List<Tower>();

    public List<MeshRenderer> Renderers { get; } = new List<MeshRenderer>();

    Dictionary<MeshRenderer, int> m_meshMaterialIndex;

    public void Init()
    {
        Renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }

    public void CacheMaterialIndex(List<Material> materials)
    {
        if(m_meshMaterialIndex == null)
        {
            m_meshMaterialIndex = new Dictionary<MeshRenderer, int>();

            foreach(var renderer in Renderers)
            {
                int matIndex = materials.IndexOf(renderer.sharedMaterial);
                if(matIndex != -1)
                    m_meshMaterialIndex[renderer] = matIndex;
            }
        }
    }

    public void Activate(List<Material> materials)
    {
        if(materials != null)
        {
            foreach(var renderer in Renderers)
            {
                if(m_meshMaterialIndex.TryGetValue(renderer, out int matIndex))
                    renderer.sharedMaterial = materials[matIndex];
            }

            foreach(var tower in m_possibleTargetTowers)
            {
                tower.m_standardMaterial = materials[0];
                tower.m_targetMaterial = materials[1];
            }
        }

        TargetTowers.Clear();
        TargetTowers.AddRange(m_possibleTargetTowers);

        while(TargetTowers.Count > 2)
            TargetTowers.RemoveAt(Random.Range(0, TargetTowers.Count));

        foreach(var tower in m_possibleTargetTowers)
            tower.Activate(TargetTowers.Contains(tower));

        bool activateWave = m_type == eType.City || m_type == eType.Boss || m_type == eType.BossCooldown;

        if(Boss.Instance.IsDead)
            activateWave = false;

        if(m_waveOffset != null)
        {
            foreach(Transform child in m_waveOffset)
            {
                child.gameObject.SetActive(activateWave);
                if (activateWave)
                {
                    RectTransform childRectTransform = child as RectTransform;

                    if(child.TryGetComponent(out BossPoint bossPoint))
                        bossPoint.Activate();                    

                    CollidableObject.ColliderType colType = (bossPoint != null) ? CollidableObject.ColliderType.Boss : CollidableObject.ColliderType.Enemy;
                    CollisionDetector.Instance.Register(colType, childRectTransform);
                }
            }
        }

        if(activateWave)
            CollisionDetector.Instance.OnCollisionTriggered += OnCollisionTriggered;

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if(m_waveOffset != null)
        {
            foreach(RectTransform child in m_waveOffset)
                DeactivateChild(child);
        }
        
        CollisionDetector.Instance.OnCollisionTriggered -= OnCollisionTriggered;
            
        gameObject.SetActive(false);
    }

    private void OnCollisionTriggered(List<CollidableObject> collidables)
    {
        var enemy = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Enemy);
        if(enemy != null)
            DeactivateChild(enemy.RectTransform);
    }

    private void DeactivateChild(RectTransform rectTransform)
    {
        rectTransform.gameObject.SetActive(false);
        if(CollisionDetector.Instance != null)
            CollisionDetector.Instance.UnRegister(rectTransform);
    }
}
