using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPigeon : BasicPigeon
{
    const float FORWARD_PREDICT_SPEED = 50;

    public override void Tick()
    {
        if(ReturnState == Pigeon.eReturnState.NONE)
        {
            RectTransform target = HitBoxRender.Instance.GetClosest(transform.localPosition);

            if(target != null)
            {
                Vector3 pos = target.localPosition;

                float dist = Mathf.Abs(pos.x - transform.localPosition.x);
                float time = dist / baseSpeed;

                pos.y -= time * ScrollingLevel.Instance.m_speed * FORWARD_PREDICT_SPEED;

                RotateTowards(pos);
            }
            
            base.Tick();
        }
        else
        {
            base.Tick();
        }
    }
}
