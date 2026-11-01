using UnityEngine;

public sealed class CubeEnemy : Enemy
{
    [SerializeField] private Color _color = Color.red;

    protected override void Awake()
    {
        base.Awake();
        View.SetColor(_color);
    }

    protected override EnemyBehaviour CreateBehaviour()
    {
        return new EnemyBehaviour();
    }
}
