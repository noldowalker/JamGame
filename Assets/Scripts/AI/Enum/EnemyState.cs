namespace AI.Enum
{
    public enum EnemyState
    {
        Undefined = 0,
        
        Idle = 1,
        MoveTowardsPlayer = 2,
        Attacking = 4,
        Punched = 5,
        Kicked = 6,
        Dying = 7,
        SelfDestroy = 8,
    }
}