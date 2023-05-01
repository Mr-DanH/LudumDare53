using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Singleton<Boss>
{
    int m_bossHealth;
    const int BOSS_HEALTH_MAX = 10;

    public List<RectTransform> m_messages = new List<RectTransform>();
    public List<RectTransform> m_explosions = new List<RectTransform>();
    int m_messageCount;

    public bool IsDead { get; set; }

    public void Init()
    {
        m_bossHealth = BOSS_HEALTH_MAX;

        foreach(var message in m_messages)
            message.gameObject.SetActive(false);
            
        foreach(var explosion in m_explosions)
            explosion.gameObject.SetActive(false);

        m_messageCount = 0;

        IsDead = false;
    }

    void Start()
    {
        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;        
    }

    private void HandleCollisionTriggered(List<CollidableObject> collidables)
    {
        CollidableObject collidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Boss);

        if (collidable == null)
            return;

        var other = collidables.Find(x=>x.Type != CollidableObject.ColliderType.Boss);
        if(other.Type == CollidableObject.ColliderType.Pigeon)
        {
            --m_bossHealth;

            if(m_messageCount < m_messages.Count)
            {
                var message = m_messages[m_messageCount++];
                message.transform.localPosition = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
                message.gameObject.SetActive(true);
            }

            if(m_bossHealth == 0)
                StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        foreach(var explosion in m_explosions)
        {
            explosion.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }

        gameObject.SetActive(false);

        IsDead = true;
    }

    public float GetNormalisedHealth()
    {
        return (float)m_bossHealth / BOSS_HEALTH_MAX;
    }
}
