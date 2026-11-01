using UnityEngine;

public sealed class CubeEnemy : Enemy
{
    [SerializeField] private Color _color = Color.red;
    [SerializeField] private EnemyMovePattern _pattern;

    protected override void Awake()
    {
        base.Awake();
        View.SetColor(_color);
    }

    protected override EnemyBehaviour CreateBehaviour()
    {
        return new EnemyBehaviour(_pattern);
    }
}
