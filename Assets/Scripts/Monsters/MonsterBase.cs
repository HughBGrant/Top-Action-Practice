using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterBase : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected MonsterType type;
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected Transform targetTransform;

    protected int currentHealth;
    protected bool isDead;
    protected float distance;

    protected MonsterBehavior behavior;
    protected MonsterStateMachine stateMachine;

    protected Rigidbody rigid;
    protected NavMeshAgent meshAgent;
    protected Animator animator;
    protected MeshRenderer[] meshes;

    protected Coroutine hitCo;
    public Transform TargetTransform { get { return targetTransform; } }
    public MonsterType Type { get { return type; } }
    public bool IsDead { get { return isDead; } private set { isDead = value; } }
    public float Distance { get { return distance; } set { distance = value; } }
    public MonsterBehavior Behavior { get { return behavior; } }
    public MonsterStateMachine StateMachine { get { return stateMachine; } }
    public Rigidbody Rigid { get { return rigid; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent MeshAgent { get { return meshAgent; } }
    public MeshRenderer[] Meshes { get { return meshes; } }

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        meshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();

        currentHealth = maxHealth;

        stateMachine = new MonsterStateMachine();
    }
    protected virtual void RegisterStates()
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
        if (!IsDead)
        {
            stateMachine.Update();
        }
        Distance = Vector3.Distance(transform.position, TargetTransform.position);
        //if (type != MonsterType.Boss)
        //{
        //    meshAgent.SetDestination(TargetTransform.position);
        //    meshAgent.isStopped = !isChasing;
        //}
    }
    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        hitCo ??= StartCoroutine(FlashOnHit());

        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;

            DeadState deadState = stateMachine.GetState<DeadState>();
            deadState.SetDeathInfo(hitPoint, isHitGrenade);
            StateMachine.ChangeState(MonsterStateType.Dead);
        }
    }
    //protected virtual void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying || behavior == null) { return; }

    //    Vector3 start = transform.position;
    //    Vector3 end = start + transform.forward * behavior.AttackRange;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(start, behavior.AttackRadius);
    //    Gizmos.DrawWireSphere(end, behavior.AttackRadius);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(start, end);
    //}
    private IEnumerator FlashOnHit()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }
        yield return YieldCache.WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        hitCo = null;
    }
    public bool ShouldReturnToChase()
    {
        return distance > Behavior.AttackRange;
    }
}