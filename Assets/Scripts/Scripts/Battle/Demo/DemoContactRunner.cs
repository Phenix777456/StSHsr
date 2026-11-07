using UnityEngine;

public sealed class DemoContactRunner : MonoBehaviour
{
    [SerializeField] private BattleExecutor _executor;
    [SerializeField] private DemoParticipant _first;
    [SerializeField] private DemoParticipant _second;
    [SerializeField] private bool _firstDeclares = true;
    [SerializeField] private bool _secondDeclares = true;

    private ContactDirector _director;

    private void Awake()
    {
        _director = new ContactDirector(_executor);
        _director.Register(_first);
        _director.Register(_second);
    }

    private void OnDestroy()
    {
        _director.Dispose();
    }

    [ContextMenu("Run Contact")]
    public void RunContact()
    {
        if (_firstDeclares)
        {
            _first.DeclareAttack(_second);
        }

        if (_secondDeclares)
        {
            _second.DeclareAttack(_first);
        }

        StartCoroutine(_director.OnContact(_first, _second, LogReport));
    }

    private static void LogReport(BattleReport report)
    {
        Debug.Log($"First landed hit: {report.FirstLandedHit}, Second landed hit: {report.SecondLandedHit}");
    }
}
