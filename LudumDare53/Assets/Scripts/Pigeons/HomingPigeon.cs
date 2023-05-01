using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPigeon : BasicPigeon
{
    const float FORWARD_PREDICT_SPEED = 50;

    void HomeOn(Vector3 pos)
    {
        float dist = Mathf.Abs(pos.x - transform.localPosition.x);
        float time = dist / baseSpeed;

        pos.y -= time * ScrollingLevel.Instance.m_speed * FORWARD_PREDICT_SPEED;

        RotateTowards(pos);
    }

    public override void Tick()
    {
        if(ReturnState == Pigeon.eReturnState.NONE)
        {
            RectTransform target = HitBoxRender.Instance.GetClosest(transform.localPosition);

            if(target != null)
            {
                Vector3 pos = target.localPosition;
                HomeOn(pos);
            }
            else if(Boss.Instance.gameObject.activeInHierarchy)
            {
                Vector3 worldPos = Boss.Instance.transform.position;
                Vector3 pos = transform.parent.InverseTransformPoint(worldPos);
                HomeOn(pos);
            }            

            if(transform.localPosition.y < -Screen.height * 0.5f * transform.lossyScale.y)
                ReturnState = Pigeon.eReturnState.RETURNING;
        }
            
        base.Tick();
    }
}
