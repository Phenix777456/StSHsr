using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StSHsr.Battle
{
    public abstract class BattlePuppet : MonoBehaviour
    {
        private readonly List<IBattleAbility> _abilities = new List<IBattleAbility>();

        public event Action<BattlePuppet> HitLanded;

        public IReadOnlyList<IBattleAbility> Abilities => _abilities;

        public void AddAbility(IBattleAbility ability)
        {
            if (ability != null)
            {
                _abilities.Add(ability);
            }
        }

        public IEnumerator Attack(BattlePuppet target)
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                yield return _abilities[i].Execute(this, target);
            }
        }

        public IEnumerator PlayHurt()
        {
            Vector3 origin = transform.position;
            const float duration = 0.2f;
            const float amplitude = 0.15f;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float offset = Mathf.Sin(t / duration * Mathf.PI * 6f) * amplitude;
                transform.position = origin + Vector3.right * offset;
                yield return null;
            }

            transform.position = origin;
        }

        // Вызывается способностью или Animation Event в момент попадания.
        public void NotifyHit()
        {
            HitLanded?.Invoke(this);
        }
    }
}
