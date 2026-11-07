using System;
using UnityEngine;

namespace StSHsr.Battle
{
    public interface IBattleParticipant
    {
        event Action<IBattleParticipant> AttackDeclared;

        BattlePuppet CreatePuppet(Transform slot);
    }
}
