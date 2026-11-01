using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class EnemyView : MonoBehaviour
{
    private SpriteRenderer _renderer;

    public void SetSprite(Sprite sprite)
    {
        GetRenderer().sprite = sprite;
    }

    public void SetColor(Color color)
    {
        GetRenderer().color = color;
    }

    private SpriteRenderer GetRenderer()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        return _renderer;
    }
}
