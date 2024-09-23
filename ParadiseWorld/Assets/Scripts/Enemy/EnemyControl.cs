using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] float visionRange = 5f;
    [SerializeField] float attackRange = 1f;
    Transform follow;
    NavMeshAgent agent;
    Animator animator;
    SoldierAttack player;
    EmissionModule emission;
    Vector3 followVector;
    internal bool isHit = false;
    bool isRun, isIdle = false;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        follow = FindAnyObjectByType<SoldierControl>().gameObject.transform;
        animator = GetComponent<Animator>();
        player = follow.GetComponent<SoldierAttack>();
        emission = GetComponentInChildren<ParticleSystem>().emission;
    }
    private void FixedUpdate()
    {
        if (!player.isAlive)
        {
            Idle();
            return;
        }
        Actions();
    }
    private void Actions()
    {
        float distance = Vector3.Distance(transform.position, follow.position);
        if (distance <= attackRange || distance <= visionRange)
        {
            isIdle = false;
            transform.LookAt(follow.position);
            if (distance <= attackRange)
                Attack();
            else if (distance <= visionRange)
                Run();
        }
        else
            Idle();
    }
    private void Attack()
    {
        if (isHit) return;
        isHit = true;
        isRun = false;
        emission.rateOverTime = 1;
        animator.StopAnimation(false);
        animator.PlayAnimation("Attack", false, 1);
        agent.SetDestination(transform.position);
        Invoke(nameof(StopAttackAnim), 1.25f);
    }
    private void StopAttackAnim() => isHit = false;
    private void Run()
    {
        agent.SetDestination(follow.position);
        if (isRun) return;
        animator.PlayAnimation("Run", isNeedState: false);
        emission.rateOverTime = 8;
        isRun = true;
    }
    private void Idle()
    {
        if (isIdle) return;
        agent.SetDestination(transform.position); 
        animator.PlayAnimation("Idle", isNeedState: false);
        isRun = false;
        isIdle = true;
    }
}
