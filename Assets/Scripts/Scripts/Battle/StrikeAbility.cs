using System.Collections;
using UnityEngine;

public sealed class StrikeAbility : IBattleAbility
{
    private readonly float _legDuration;
    private readonly float _reach;

    public StrikeAbility(float legDuration = 0.25f, float reach = 0.6f)
    {
        _legDuration = legDuration;
        _reach = reach;
    }

    public IEnumerator Execute(BattlePuppet self, BattlePuppet target)
    {
        Vector3 start = self.transform.position;
        Vector3 hitPoint = Vector3.Lerp(start, target.transform.position, _reach);

        yield return MoveTo(self.transform, start, hitPoint);
        self.NotifyHit();
        yield return MoveTo(self.transform, hitPoint, start);
    }

    private IEnumerator MoveTo(Transform subject, Vector3 from, Vector3 to)
    {
        for (float t = 0f; t < _legDuration; t += Time.deltaTime)
        {
            subject.position = Vector3.Lerp(from, to, t / _legDuration);
            yield return null;
        }

        subject.position = to;
    }
}
