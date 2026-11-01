using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerAnimator : MonoBehaviour
{
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int DirectionXParameter = Animator.StringToHash("DirectionX");
    private static readonly int DirectionYParameter = Animator.StringToHash("DirectionY");

    [SerializeField, Min(0.01f)] private float _attackDuration = 0.3f;

    private Animator _animator;
    private WaitForSeconds _attackWait;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _attackWait = new WaitForSeconds(_attackDuration);
    }

    public IEnumerator PlayAttack(Vector2Int direction)
    {
        _animator.SetFloat(DirectionXParameter, direction.x);
        _animator.SetFloat(DirectionYParameter, direction.y);
        _animator.SetTrigger(AttackTrigger);

        yield return _attackWait;
    }
}
