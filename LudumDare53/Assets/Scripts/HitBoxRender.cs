using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveHitBox
{
    public Tower AttachedTower { get; set; }
    public RectTransform HitBox { get; private set; }
    public RectTransform VisualBox { get; private set; }

    public float TimeActive { get; set; }

    public ActiveHitBox(Tower tower, RectTransform hitBox, RectTransform visualBox)
    {
        TimeActive = 0;
        AttachedTower = tower;
        HitBox = hitBox;
        VisualBox = visualBox;
    }
}

public class HitBoxRender : Singleton<HitBoxRender>
{
    public System.Action OnPigeonArrived;

    public RectTransform m_hitBoxPrefab;

    List<RectTransform> m_pool = new List<RectTransform>();
    List<ActiveHitBox> m_activeHitBoxes = new List<ActiveHitBox>();

    void Awake()
    {
        m_pool.Add(m_hitBoxPrefab);

        for(int i = 0; i < 19; ++i)
        {
            var gameObject = Instantiate(m_hitBoxPrefab, transform);
            gameObject.name = $"{i}-Box";
            m_pool.Add(gameObject);
        }

        foreach(var pooledItem in m_pool)
            pooledItem.gameObject.SetActive(false);

        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;
    }

    public void Tick()
    {
        Camera camera = Camera.main;
        float lossyScale = 1 / transform.lossyScale.x; 

        Vector3 screenScale = new Vector3(Screen.width, Screen.height, 1);

        //Create new hit boxes
        foreach(var tile in ScrollingLevel.Instance.ActiveTiles)
        {
            foreach(var tower in tile.TargetTowers)
            {
                if(!tower.IsTarget)
                    continue;

                bool hasBox = false;

                foreach(var activeBox in m_activeHitBoxes)
                {
                    if(activeBox.AttachedTower == tower)
                        hasBox = true;
                }

                if(hasBox)
                    continue;

                RectTransform box = m_pool[0];
                m_pool.RemoveAt(0);
                box.gameObject.SetActive(true);
                
                RectTransform visualBox = box.GetChild(0) as RectTransform;
                visualBox.gameObject.SetActive(false);

                m_activeHitBoxes.Add(new ActiveHitBox(tower, box, visualBox));

                tower.OnDisableEvent += OnTowerDisabled;
                
                CollisionDetector.Instance.Register(CollidableObject.ColliderType.Building, box);
            }
        }

        // //Remove dead boxes
        // for(int i = m_activeHitBoxes.Count - 1; i >= 0; --i)
        // {
        //     var activeBox = m_activeHitBoxes[i];
        //     if(!activeBox.AttachedTower.gameObject.activeInHierarchy)
        //     {            
        //         CollisionDetector.Instance.Register(CollidableObject.ColliderType.Building, activeBox.HitBox);
        //         activeBox.HitBox.gameObject.SetActive(false);
        //         m_activeHitBoxes.RemoveAt(i);
        //     }
        // }

        foreach(var activeBox in m_activeHitBoxes)
        {
            //Get bounds in viewport space
            Bounds bounds = activeBox.AttachedTower.Collider.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            
            Vector2 vpMin = Vector2.one * 2;
            Vector2 vpMax = Vector2.one * -1;

            for(int x = 0; x < 2; ++x)
            {
                for(int y = 0; y < 2; ++y)
                {
                    for(int z = 0; z < 2; ++z)
                    {
                        Vector3 worldPos = new Vector3(
                            x == 0 ? min.x : max.x,
                            y == 0 ? min.y : max.y,
                            z == 0 ? min.z : max.z
                            );

                        Vector3 vpPos = camera.WorldToViewportPoint(worldPos);

                        if(vpPos.z < 0)
                            continue;

                        vpMin.x = Mathf.Min(vpMin.x, vpPos.x);
                        vpMin.y = Mathf.Min(vpMin.y, vpPos.y);
                        vpMax.x = Mathf.Max(vpMax.x, vpPos.x);
                        vpMax.y = Mathf.Max(vpMax.y, vpPos.y);
                    }
                }
            }            

            vpMin.x = Mathf.Clamp(vpMin.x, -1, 2);
            vpMin.y = Mathf.Clamp(vpMin.y, -1, 2);
            vpMax.x = Mathf.Clamp(vpMax.x, -1, 2);
            vpMax.y = Mathf.Clamp(vpMax.y, -1, 2);
            
            Vector2 vpAvg = (vpMin + vpMax) * 0.5f;

            activeBox.HitBox.position = Vector3.Scale(vpAvg, screenScale);
            activeBox.HitBox.sizeDelta = Vector3.Scale(vpMax - vpMin, screenScale) * lossyScale;

            if(vpAvg.y < 1)
            {
                activeBox.TimeActive += Time.deltaTime;
                
                if(!activeBox.VisualBox.gameObject.activeSelf)
                    activeBox.VisualBox.gameObject.SetActive(true);
                    
                activeBox.VisualBox.sizeDelta = activeBox.HitBox.sizeDelta * Mathf.Lerp(3, 1, Mathf.Sin(Mathf.Clamp01(activeBox.TimeActive * 2) * Mathf.PI * 0.5f));
            }
        }
    }

    void OnTowerDisabled(Tower tower)
    {
        tower.OnDisableEvent -= OnTowerDisabled;
        
        ActiveHitBox activeHitBox = m_activeHitBoxes.Find(x => x.AttachedTower == tower);
        if(activeHitBox != null && activeHitBox.HitBox != null)
        {
            activeHitBox.HitBox.gameObject.SetActive(false);
            m_pool.Add(activeHitBox.HitBox);
            m_activeHitBoxes.Remove(activeHitBox);  
            if(CollisionDetector.Instance != null)          
                CollisionDetector.Instance.UnRegister(activeHitBox.HitBox);
        }
    }

    private void HandleCollisionTriggered(List<CollidableObject> collidables)
    {
        var collidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Building);

        if (collidable != null)
        {
            CollisionDetector.Instance.UnRegister(collidable);

            collidable.RectTransform.gameObject.SetActive(false);
            m_pool.Add(collidable.RectTransform);

            ActiveHitBox activeHitBox = m_activeHitBoxes.Find(x=>x.HitBox == collidable.RectTransform);
            m_activeHitBoxes.Remove(activeHitBox);
            activeHitBox.AttachedTower.PigeonArrive();

            OnPigeonArrived.Invoke();
        }
    }

    public RectTransform GetClosest(Vector3 pos)
    {
        if(m_activeHitBoxes.Count == 0)
            return null;

        float minDist = float.MaxValue;
        RectTransform minRect = null;

        foreach(var box in m_activeHitBoxes)
        {
            if(!box.VisualBox.gameObject.activeSelf)
                continue;

            float dist = (box.HitBox.localPosition - pos).sqrMagnitude;
            if(dist < minDist)
            {
                minDist = dist;
                minRect = box.HitBox;
            }
        }

        return minRect;
    }
}
