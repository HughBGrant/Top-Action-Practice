public abstract class MonsterState
{
    protected MonsterBase monster;
    public abstract MonsterStateType StateType { get; }

    public MonsterState(MonsterBase monster)
    {
        this.monster = monster;
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}