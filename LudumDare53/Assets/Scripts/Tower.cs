using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Material m_standardMaterial;
    public Material m_targetMaterial;

    public Collider Collider { get { return m_collider; } }
    public bool IsTarget { get; private set; }

    MeshRenderer m_renderer;
    BoxCollider m_collider;

    void Awake()
    {
        TryGetComponent(out m_renderer);
        TryGetComponent(out m_collider);
        m_collider.enabled = false;
    }

    public void Activate(bool isTarget)
    {
        IsTarget = false;
        m_renderer.material = isTarget ? m_targetMaterial : m_standardMaterial;        
    }

    public void PigeonArrive()
    {
        Activate(false);
    }
}
