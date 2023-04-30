using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public MeshRenderer m_renderer;
    public Material m_standardMaterial;
    public Material m_targetMaterial;

    public Collider Collider { get { return m_collider; } }
    public bool IsTarget { get; private set; }

    BoxCollider m_collider;
    Vector3 m_baseScale;

    void Awake()
    {
        m_collider = GetComponentInChildren<BoxCollider>();
        m_baseScale = transform.localScale;
    }

    public void Activate(bool isTarget)
    {
        m_collider.enabled = isTarget;
        IsTarget = isTarget;
        m_renderer.material = isTarget ? m_targetMaterial : m_standardMaterial;        
    }

    public void PigeonArrive()
    {
        Activate(false);
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
}
