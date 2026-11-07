using System;
using UnityEngine;

public interface IBattleParticipant
{
    event Action<IBattleParticipant> AttackDeclared;

    BattlePuppet CreatePuppet(Transform slot);
}
