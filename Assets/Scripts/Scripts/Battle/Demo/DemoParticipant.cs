using System;
using UnityEngine;

namespace StSHsr.Battle.Demo
{
    public sealed class DemoParticipant : MonoBehaviour, IBattleParticipant
    {
        [SerializeField] private BattlePuppet _puppetPrefab;

        public event Action<IBattleParticipant> AttackDeclared;

        public void DeclareAttack(IBattleParticipant target)
        {
            AttackDeclared?.Invoke(target);
        }

        public BattlePuppet CreatePuppet(Transform slot)
        {
            BattlePuppet puppet = Instantiate(_puppetPrefab, slot.position, slot.rotation);
            puppet.AddAbility(new StrikeAbility());
            return puppet;
        }
    }
}
