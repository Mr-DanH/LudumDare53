using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPoint : MonoBehaviour
{
    public void Activate()
    {
        if(Boss.Instance.IsDead)
            return;

        Boss.Instance.transform.SetParent(transform);
        Boss.Instance.transform.localScale = Vector3.one;
        Boss.Instance.transform.localPosition = Vector3.zero;
        Boss.Instance.gameObject.SetActive(true);        
    }
}
