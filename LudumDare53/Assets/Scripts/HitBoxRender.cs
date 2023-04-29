using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxRender : MonoBehaviour
{
    public RectTransform m_hitBoxPrefab;

    List<RectTransform> m_pool = new List<RectTransform>();
    List<RectTransform> m_activeHitBoxes = new List<RectTransform>();

    void Awake()
    {
        m_pool.Add(m_hitBoxPrefab);

        for(int i = 0; i < 19; ++i)
            m_pool.Add(Instantiate(m_hitBoxPrefab, transform));

        foreach(var pooledItem in m_pool)
            pooledItem.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        Camera camera = Camera.main;
        int count = 0;

        Vector3 screenScale = new Vector3(Screen.width, Screen.height, 1);

        foreach(var tile in ScrollingLevel.Instance.ActiveTiles)
        {
            BoxCollider[] colliders = tile.GetComponentsInChildren<BoxCollider>();

            foreach(var collider in colliders)
            {
                RectTransform box;

                if(count >= m_activeHitBoxes.Count)
                {
                    box = m_pool[0];
                    box.gameObject.SetActive(true);
                    m_activeHitBoxes.Add(box);
                    m_pool.RemoveAt(0);

                    ++count;
                }
                else
                {
                    box = m_activeHitBoxes[count];
                }

                //Get bounds in viewport space
                Bounds bounds = collider.bounds;
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
            RectTransform box = m_activeHitBoxes[count];
            box.gameObject.SetActive(false);
            m_pool.Add(box);
            m_activeHitBoxes.RemoveAt(count);
        }
    }
}
