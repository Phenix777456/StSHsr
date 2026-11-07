using System;
using System.Collections;
using UnityEngine;

public sealed class BattleExecutor : MonoBehaviour
{
    [SerializeField] private BattleStage _stage;

    public IEnumerator Run(
        IBattleParticipant first,
        IBattleParticipant second,
        bool firstAttacks,
        bool secondAttacks,
        Action<BattleReport> onFinished)
    {
        BattlePuppet firstPuppet = first.CreatePuppet(_stage.FirstSlot);
        BattlePuppet secondPuppet = second.CreatePuppet(_stage.SecondSlot);
        var report = new BattleReport();

        Action<BattlePuppet> onFirstHit = _ => report.FirstLandedHit = true;
        Action<BattlePuppet> onSecondHit = _ => report.SecondLandedHit = true;
        firstPuppet.HitLanded += onFirstHit;
        secondPuppet.HitLanded += onSecondHit;

        _stage.Show();

        if (firstAttacks && secondAttacks)
        {
            yield return RunParallel(firstPuppet.Attack(secondPuppet), secondPuppet.Attack(firstPuppet));
        }
        else if (firstAttacks)
        {
            yield return firstPuppet.Attack(secondPuppet);
            yield return secondPuppet.PlayHurt();
        }
        else if (secondAttacks)
        {
            yield return secondPuppet.Attack(firstPuppet);
            yield return firstPuppet.PlayHurt();
        }

        _stage.Hide();

        firstPuppet.HitLanded -= onFirstHit;
        secondPuppet.HitLanded -= onSecondHit;
        Destroy(firstPuppet.gameObject);
        Destroy(secondPuppet.gameObject);

        onFinished?.Invoke(report);
    }

    private IEnumerator RunParallel(IEnumerator a, IEnumerator b)
    {
        bool aDone = false;
        bool bDone = false;

        StartCoroutine(Wrap(a, () => aDone = true));
        StartCoroutine(Wrap(b, () => bDone = true));

        yield return new WaitUntil(() => aDone && bDone);
    }

    private static IEnumerator Wrap(IEnumerator routine, Action onDone)
    {
        yield return routine;
        onDone();
    }
}
