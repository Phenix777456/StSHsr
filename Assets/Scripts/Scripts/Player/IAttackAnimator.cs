using System.Collections;
using UnityEngine;

public interface IAttackAnimator
{
    IEnumerator PlayAttack(Vector2Int direction);
}
