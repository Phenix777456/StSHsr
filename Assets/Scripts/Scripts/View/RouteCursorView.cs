using UnityEngine;


    public sealed class RouteCursorView : MonoBehaviour
    {
        public void Show(Vector3 worldPosition)
        {
            transform.position = worldPosition;
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
