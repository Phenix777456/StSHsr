using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolBase<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T _prefab;
    [SerializeField, Min(1)] private int _defaultCapacity = 5;
    [SerializeField, Min(1)] private int _maxSize = 50;

    protected ObjectPool<T> Pool { get; private set; }

    protected virtual void Awake()
    {
        Pool = new ObjectPool<T>(
            CreateFunc,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true,
            _defaultCapacity,
            _maxSize);
    }

    protected virtual void OnDestroy()
    {
        Pool.Dispose();
    }

    protected virtual T CreateFunc()
    {
        T item = Instantiate(_prefab, transform);
        item.gameObject.SetActive(false);

        return item;
    }

    protected virtual void ActionOnGet(T item)
    {
    }

    protected virtual void ActionOnRelease(T item)
    {
        item.gameObject.SetActive(false);
    }

    protected virtual void ActionOnDestroy(T item)
    {
        Destroy(item.gameObject);
    }
}
