using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler_Monster : vp_DamageHandler
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
    }
    public override void Damage(vp_DamageInfo damageInfo)
    {
        base.Damage(damageInfo);

        _animator.Play("Take Damage", 0, 0);
    }
}
