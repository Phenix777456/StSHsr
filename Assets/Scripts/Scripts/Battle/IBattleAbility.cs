using System.Collections;

namespace StSHsr.Battle
{
    public interface IBattleAbility
    {
        IEnumerator Execute(BattlePuppet self, BattlePuppet target);
    }
}
