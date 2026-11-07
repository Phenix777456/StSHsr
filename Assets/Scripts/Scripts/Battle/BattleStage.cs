using UnityEngine;

public sealed class BattleStage : MonoBehaviour
{
    [SerializeField] private Transform _firstSlot;
    [SerializeField] private Transform _secondSlot;
    [SerializeField] private Camera _stageCamera;

    public Transform FirstSlot => _firstSlot;
    public Transform SecondSlot => _secondSlot;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        _stageCamera.enabled = true;
    }

    public void Hide()
    {
        _stageCamera.enabled = false;
    }
}
