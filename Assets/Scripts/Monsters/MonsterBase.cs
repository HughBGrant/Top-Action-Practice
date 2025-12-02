using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour, IDamageable
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    protected int maxHealth;
    public int MaxHealth { get { return maxHealth; } }
    [SerializeField]
    protected MonsterType type;
    public MonsterType Type { get { return type; } }
    [SerializeField]
    protected Player target;
    public Player Target { get { return target; } set { target = value; } }
    [SerializeField]
    protected float attackRadius;
    public float AttackRadius { get { return attackRadius; } }
    [SerializeField]
    protected float attackRange;
    public float AttackRange { get { return attackRange; } }
    [SerializeField]
    protected float chaseRange;
    public float ChaseRange { get { return chaseRange; } }
    [SerializeField]
    protected Collider attackCollider;
    public Collider AttackCollider { get { return attackCollider; } }
    [SerializeField]
    protected Projectile projectilePrefab;
    public virtual Projectile ProjectilePrefab { get { return projectilePrefab; } }
    [SerializeField]
    protected int rewardScore;
    public int RewardScore { get { return rewardScore; } }
    [SerializeField]
    protected GameObject[] rewardCoins;
    public GameObject[] RewardCoins { get { return rewardCoins; } }

    [SerializeField]
    protected int currentHealth;
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    protected bool isDead;
    public bool IsDead { get { return isDead; } private set { isDead = value; } }
    protected float distance;
    public float Distance { get { return distance; } set { distance = value; } }

    protected IAttackBehavior behavior;
    public virtual IAttackBehavior Behavior { get { return behavior; } set { behavior = value; } }
    protected MonsterStateMachine stateMachine;
    public MonsterStateMachine StateMachine { get { return stateMachine; } set { stateMachine = value; } }

    protected Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }
    protected NavMeshAgent meshAgent;
    public NavMeshAgent MeshAgent { get { return meshAgent; } }
    protected Animator animator;
    public Animator Animator { get { return animator; } }
    protected MeshRenderer[] meshes;
    public MeshRenderer[] Meshes { get { return meshes; } }

    protected Coroutine hitCo;

    protected virtual void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        rigid = GetComponent<Rigidbody>();
        meshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        behavior = new MonsterBehavior(this);

        currentHealth = maxHealth;

        StateMachine = new MonsterStateMachine();
    }
    protected virtual void Start()
    {
        RegisterStates();
    }
    protected virtual void RegisterStates()
    {
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));
        StateMachine.AddState(new DeathState(this));

        StateMachine.ChangeState(MonsterStateType.Idle);
    }
    protected virtual void Update()
    {
        Distance = Vector3.Distance(transform.position, Target.transform.position);

        StateMachine.Update();
    }
    public virtual void TakeDamage(int damage, Vector3 hitPoint, bool isHitGrenade = false)
    {
        currentHealth -= damage;
        hitCo ??= StartCoroutine(FlashOnHit());

        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;

            DeathState deadState = StateMachine.GetState<DeathState>();
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