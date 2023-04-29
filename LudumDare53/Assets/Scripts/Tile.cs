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
        WastelandToCity
    }

    public eType m_type;
    public Transform m_waveOffset;

    public List<Tower> m_possibleTargetTowers = new List<Tower>();
    
    public List<Tower> TargetTowers { get; } = new List<Tower>();

    public void Activate(bool activateWave)
    {
        TargetTowers.Clear();
        TargetTowers.AddRange(m_possibleTargetTowers);

        while(TargetTowers.Count > 2)
            TargetTowers.RemoveAt(Random.Range(0, TargetTowers.Count));

        foreach(var tower in m_possibleTargetTowers)
            tower.Activate(TargetTowers.Contains(tower));

        if(m_waveOffset != null)
        {
            foreach(Transform child in m_waveOffset)
            {
                child.gameObject.SetActive(activateWave);
                if (activateWave)
                {
                    RectTransform childRectTransform = child as RectTransform;
                    CollisionDetector.Instance.Register(CollidableObject.ColliderType.Enemy, childRectTransform);
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
            
        gameObject.SetActive(false);
    }

    private void OnCollisionTriggered(List<CollidableObject> collidables)
    {
        var collidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Enemy);
        if(collidable != null)
            DeactivateChild(collidable.RectTransform);
    }

    private void DeactivateChild(RectTransform rectTransform)
    {
        rectTransform.gameObject.SetActive(false);
        if(CollisionDetector.Instance != null)
            CollisionDetector.Instance.UnRegister(rectTransform);
    }
}
