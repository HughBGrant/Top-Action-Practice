using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected MonsterType type;
    [SerializeField]
    protected Transform targetTransform;
    [SerializeField]
    protected float attackRadius;
    [SerializeField]
    protected float attackRange;
    [SerializeField]
    protected float chaseRange;
    [SerializeField]
    protected Collider attackCollider;
    [SerializeField]
    protected Projectile projectilePrefab;

    protected int currentHealth;
    protected bool isDead;
    protected float distance;

    protected IAttackBehavior behavior;
    protected MonsterStateMachine stateMachine;

    protected Rigidbody rigid;
    protected NavMeshAgent meshAgent;
    protected Animator animator;
    protected MeshRenderer[] meshes;

    protected Coroutine hitCo;

    public MonsterType Type { get { return type; } }
    public Transform TargetTransform { get { return targetTransform; } }

    public float AttackRadius { get { return attackRadius; } }
    public float AttackRange { get { return attackRange; } }
    public float ChaseRange { get { return chaseRange; } }
    public Collider AttackCollider { get { return attackCollider; } }
    public virtual Projectile ProjectilePrefab { get { return projectilePrefab; } }
    public bool IsDead { get { return isDead; } private set { isDead = value; } }
    public float Distance { get { return distance; } set { distance = value; } }
    public virtual IAttackBehavior Behavior { get { return behavior; } set { behavior = value; } }
    public MonsterStateMachine StateMachine { get { return stateMachine; } set { stateMachine = value; } }
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
        behavior = new MonsterBehavior(this);

        currentHealth = maxHealth;

        StateMachine = new MonsterStateMachine();
    }
    protected virtual void RegisterStates()
    {
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new DeadState(this));

        StateMachine.ChangeState(MonsterStateType.Idle);
    }
    protected virtual void Start()
    {
        RegisterStates();
    }
    protected virtual void Update()
    {
        Distance = Vector3.Distance(transform.position, TargetTransform.position);

        StateMachine.Update();
    }
    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        hitCo ??= StartCoroutine(FlashOnHit());

        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;

            DeadState deadState = StateMachine.GetState<DeadState>();
            deadState.SetDeathInfo(hitPoint, isHitGrenade);
            StateMachine.ChangeState(MonsterStateType.Dead);
        }
    }
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
    public bool IsTargetInAttackRange()
    {
        return Distance < AttackRange;
    }
    public bool IsTargetInChaseRange()
    {
        return Distance < ChaseRange;
    }
}