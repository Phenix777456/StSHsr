using System.Collections;
using UnityEngine;

public sealed class PathMover : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float _stepTime = 0.15f;

    public IEnumerator MoveTo(Vector3 target)
    {
        Vector3 from = transform.position;

        for (float elapsed = 0f; elapsed < _stepTime; elapsed += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, target, elapsed / _stepTime);

            yield return null;
        }

        transform.position = target;
    }
}
