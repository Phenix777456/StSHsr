using System.Collections;

public interface IBattleAbility
{
    IEnumerator Execute(BattlePuppet self, BattlePuppet target);
}
