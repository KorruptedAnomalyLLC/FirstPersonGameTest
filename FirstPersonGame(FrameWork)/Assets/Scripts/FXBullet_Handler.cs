using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXBullet_Handler : vp_FXBullet
{
    protected override void DoUFPSDamage()
    {
        //Must be a critical hit if the BoxCollider is hit
        if(m_Hit.collider is BoxCollider)
        {
            Damage = 100;
        }

        //Call base method
        base.DoUFPSDamage();
    }
}
