using UnityEngine;

public static class AnimID
{
    public static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    public static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    public static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    public static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    public static readonly int JumpHash = Animator.StringToHash("Jump");
    public static readonly int DodgeHash = Animator.StringToHash("Dodge");
    public static readonly int SwapHash = Animator.StringToHash("Swap");
    public static readonly int ReloadHash = Animator.StringToHash("Reload");
    public static readonly int DieHash = Animator.StringToHash("Die");
    public static readonly int GreetHash = Animator.StringToHash("Greet");
    public static readonly int LaunchMissileHash = Animator.StringToHash("LaunchMissile");
    public static readonly int ThrowRockHash = Animator.StringToHash("ThrowRock");
    public static readonly int JumpAttackHash = Animator.StringToHash("JumpAttack");
    public static readonly int SwingHash = Animator.StringToHash("Swing");
    public static readonly int ShootHash = Animator.StringToHash("Shoot");

}
