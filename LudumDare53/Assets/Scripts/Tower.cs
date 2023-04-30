using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public MeshRenderer m_renderer;
    public Material m_standardMaterial;
    public Material m_targetMaterial;
    public Animator m_mailbox;

    public Collider Collider { get { return m_collider; } }
    public bool IsTarget { get; private set; }

    public event System.Action<Tower> OnDisableEvent;

    BoxCollider m_collider;
    Vector3 m_baseScale;

    bool m_init;

    void Init()
    {
        if(m_init)
            return;

        m_collider = GetComponentInChildren<BoxCollider>(true);
        m_baseScale = transform.localScale;

        m_init = true;
    }

    public void Activate(bool isTarget)
    {
        Init();

        m_collider.enabled = isTarget;
        IsTarget = isTarget;
        m_renderer.material = isTarget ? m_targetMaterial : m_standardMaterial;
        m_mailbox.gameObject.SetActive(isTarget);
        m_mailbox.transform.rotation = Quaternion.LookRotation(Vector3.right);
    }

    public void PigeonArrive()
    {
        m_collider.enabled = false;
        IsTarget = false;
        m_renderer.material = m_standardMaterial;
        m_mailbox.SetTrigger("Closed");
        if(gameObject.activeInHierarchy)
            StartCoroutine(SquashAndSqueeze());
    }

    IEnumerator SquashAndSqueeze()
    {
        const float DURATION = 0.5f;
        for(float t = 0; t < DURATION; t += Time.deltaTime)
        {
            float normT = t / DURATION;

            float sin = Mathf.InverseLerp(-1, 1, Mathf.Sin(normT * Mathf.PI) * Mathf.Clamp01(1 - normT));
            float width = Mathf.Lerp(0.5f, 1.5f, sin);
            float height = Mathf.Lerp(0.5f, 1.5f, 1 - sin);
            transform.localScale = Vector3.Scale(m_baseScale, new Vector3(width, height, width));

            yield return null;
        }

        transform.localScale = m_baseScale;
    }

    void OnDisable()
    {
        OnDisableEvent?.Invoke(this);
    }
}
