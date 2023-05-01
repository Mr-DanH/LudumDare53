using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPigeon : BasicPigeon
{
    public override void Tick()
    {
        if(ReturnState == Pigeon.eReturnState.NONE)
        {
            RectTransform target = HitBoxRender.Instance.GetClosest(transform.localPosition);

            if(target != null)
                RotateTowards(target);

            base.Tick();
        }
        else
        {
            base.Tick();
        }
    }
}
