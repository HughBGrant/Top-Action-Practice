using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    protected static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    [SerializeField]
    protected MonsterType type;
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected Transform targetTransform;

    protected int currentHealth;
    protected Rigidbody rigid;
    protected NavMeshAgent meshAgent;
    protected Animator animator;
    protected MeshRenderer[] meshes;
    protected MonsterBehavior behavior;

    protected MonsterStateMachine stateMachine;
    protected bool isDead;

    public Transform TargetTransform { get { return targetTransform; } }
    public Rigidbody Rigid { get { return rigid; } }
    public Animator Animator { get { return animator; } }
    public MonsterStateMachine StateMachine { get { return stateMachine; } }
    public NavMeshAgent MeshAgent { get { return meshAgent; } }
    public MonsterBehavior Behavior { get { return behavior; } }
    public bool IsDead { get { return isDead; } }

    protected bool isChasing;
    protected bool isAttacking;

    protected Coroutine attackCo;
    protected Coroutine hitCo;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        meshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        behavior = new MonsterBehavior(type);

        currentHealth = maxHealth;

        stateMachine = new MonsterStateMachine();

    }
    protected void RegisterStates()
    {
        stateMachine.AddState(new IdleState(this));
        stateMachine.AddState(new ChaseState(this));
        stateMachine.AddState(new AttackState(this));
        stateMachine.AddState(new DeadState(this));

        stateMachine.ChangeState(MonsterStateType.Idle);
    }
    protected virtual void Start()
    {
        RegisterStates();
        //if (type != MonsterType.Boss)
        //{
        //    StartCoroutine(BeginChase());
        //}
    }
    protected virtual void Update()
    {
        stateMachine.Update();

        //if (meshAgent && TargetTransform && type != MonsterType.Boss)
        //{
        //    meshAgent.SetDestination(TargetTransform.position);
        //    meshAgent.isStopped = !isChasing;
        //}
    }
    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            StateMachine.ChangeState(MonsterStateType.Dead);
        }
        //hitCo ??= StartCoroutine(FlashOnHit());

        //if (currentHealth <= 0)
        //{
        //    HandleDeath(hitPoint, isHitGrenade);
        //}
    }
    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying || behavior == null || stateMachine.CurrentType != MonsterStateType.Chase) { return; }

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * behavior.AttackRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, behavior.AttackRadius);
        Gizmos.DrawWireSphere(end, behavior.AttackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
    }

    //private IEnumerator FlashOnHit()
    //{
    //    foreach (MeshRenderer mesh in meshes)
    //    {
    //        mesh.material.color = Color.red;
    //    }
    //    yield return YieldCache.WaitForSeconds(0.1f);

    //    if (currentHealth > 0)
    //    {
    //        foreach (MeshRenderer mesh in meshes)
    //        {
    //            mesh.material.color = Color.white;
    //        }
    //    }
    //    hitCo = null;
    //}

}