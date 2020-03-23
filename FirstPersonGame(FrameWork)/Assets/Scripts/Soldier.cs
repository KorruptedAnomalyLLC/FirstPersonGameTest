using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : vp_DamageHandler
{
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    public GameObject Player;

    public float AttackDistance = 10.0f;

    public float FollowDistance = 20.0f;

    [Range(0.0f, 1.0f)]
    public float AttackProbability = 0.5f;

    [Range(0.0f, 1.0f)]
    public float HitAccuracy = 0.5f;

    public float DamagePoints = 2.0f;

    public AudioClip GunSound = null;

    public Transform[] patrolPoints;

    private int currentControlPointIndex = 0;


    protected override void Awake()
    {
        base.Awake();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();

        MoveToNextPatrolPoint();

    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.enabled)
        {
            float dist = Vector3.Distance(Player.transform.position, this.transform.position);

            bool shoot = false;
            bool patrol = false;
            bool follow = (dist < FollowDistance);

            if (follow)
            {
                float random = Random.Range(0.0f, 1.0f);
                if (random > (1.0f - AttackProbability) && dist < AttackDistance)
                {
                    shoot = true;
                }

                _navMeshAgent.SetDestination(Player.transform.position);
            }

            patrol = !follow && !shoot && patrolPoints.Length > 0;

            if ((!follow || shoot) && !patrol)
                _navMeshAgent.SetDestination(transform.position);

            // Patrolling between points if there are patrol points
            if (patrol)
            {
                if (!_navMeshAgent.pathPending && 
                    _navMeshAgent.remainingDistance < 0.5f)
                    MoveToNextPatrolPoint();
            }

            _animator.SetBool("Stab Attack", shoot);
            _animator.SetBool("Run Forward", follow || patrol);

            // TODO: Add a walk animation for patrol

        }
    }


    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            _navMeshAgent.destination = patrolPoints[currentControlPointIndex].position;

            currentControlPointIndex++;
            currentControlPointIndex %= patrolPoints.Length;
        }
    }

    public void EndAttack()
    {
        if (m_Audio != null)
        {
            m_Audio.PlayOneShot(GunSound);
        }

        float random = Random.Range(0.0f, 1.0f);

        // The higher the accuracy is, the more likely the player will be hit
        bool isHit = random > 1.0f - HitAccuracy;

        if (isHit)
        {
            Player.SendMessage("Damage", DamagePoints, 
                SendMessageOptions.DontRequireReceiver);
        }
    }

    public override void Die()
    {
        if (!enabled || !vp_Utility.IsActive(gameObject))
            return;

        if (m_Audio != null)
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot(DeathSound);
        }

        _navMeshAgent.enabled = false;

        _animator.SetBool("Run Forward", false);
        _animator.SetBool("Stab Attack", false);

        _animator.SetTrigger("Die");

        Destroy(GetComponent<vp_SurfaceIdentifier>());

    }
}
