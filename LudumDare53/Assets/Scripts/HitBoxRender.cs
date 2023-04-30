using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHitBox
{
    public Tower AttachedTower { get; set; }
    public RectTransform HitBox { get; private set; }

    public ActiveHitBox(Tower tower, RectTransform hitBox)
    {
        AttachedTower = tower;
        HitBox = hitBox;
    }
}

public class HitBoxRender : MonoBehaviour
{
    public System.Action OnPigeonArrived;

    public RectTransform m_hitBoxPrefab;

    List<RectTransform> m_pool = new List<RectTransform>();
    List<ActiveHitBox> m_activeHitBoxes = new List<ActiveHitBox>();

    int m_count;

    void Awake()
    {
        m_pool.Add(m_hitBoxPrefab);

        for(int i = 0; i < 19; ++i)
            m_pool.Add(Instantiate(m_hitBoxPrefab, transform));

        foreach(var pooledItem in m_pool)
            pooledItem.gameObject.SetActive(false);

        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;
    }

    public void Tick()
    {
        Camera camera = Camera.main;
        int count = 0;

        Vector3 screenScale = new Vector3(Screen.width, Screen.height, 1);

        foreach(var tile in ScrollingLevel.Instance.ActiveTiles)
        {
            foreach(var tower in tile.TargetTowers)
            {
                if(!tower.IsTarget)
                    continue;

                RectTransform box;

                if(count >= m_activeHitBoxes.Count)
                {
                    box = m_pool[0];
                    box.gameObject.SetActive(true);
                    m_activeHitBoxes.Add(new ActiveHitBox(tower,box));
                    m_pool.RemoveAt(0);

                    box.name = $"{m_count++}-Box";

                    ++count;

                    CollisionDetector.Instance.Register(CollidableObject.ColliderType.Building, box);
                }
                else
                {
                    box = m_activeHitBoxes[count].HitBox;
                    m_activeHitBoxes[count].AttachedTower = tower;
                }

                //Get bounds in viewport space
                Bounds bounds = tower.Collider.bounds;
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

                box.position = Vector3.Scale(vpAvg, screenScale);
                box.sizeDelta = Vector3.Scale(vpMax - vpMin, screenScale);

                ++count;
            }
        }

        while(count < m_activeHitBoxes.Count)
        {
            RectTransform box = m_activeHitBoxes[count].HitBox;
            box.gameObject.SetActive(false);
            m_pool.Add(box);
            m_activeHitBoxes.RemoveAt(count);

            CollisionDetector.Instance.UnRegister(box);
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
}
