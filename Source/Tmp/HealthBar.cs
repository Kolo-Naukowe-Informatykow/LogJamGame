using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Pasek zdrowia rysowany nad obiektem. Jeśli w inspektorze nie wskażesz węzła zdrowia,
/// komponent sam znajdzie u rodzica węzeł o nazwie <c>Health</c>.
/// </summary>
public partial class HealthBar : Node2D
{
    [Export] private Health _health;
    [Export] private Vector2 _size = new(64f, 8f);
    [Export] private float _offsetY = -56f;
    [Export] private Color _fullColor = new(0.35f, 0.9f, 0.45f);
    [Export] private Color _emptyColor = new(0.9f, 0.3f, 0.25f);
    [Export] private Color _backgroundColor = new(0f, 0f, 0f, 0.65f);
    [Export] private Color _borderColor = new(0f, 0f, 0f, 0.9f);
    [Export] private bool _hideWhenFull;

    public override void _Ready()
    {
        if (_health == null)
        {
            _health = GetParent()?.GetNodeOrNull<Health>("Health");
        }

        if (_health == null)
        {
            GD.PushError($"{GetPath()}: brak węzła \"Health\" u rodzica, pasek zdrowia nic nie pokaże.");
            return;
        }

        _health.Changed += OnHealthChanged;
        _health.Died += QueueRedraw;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_health == null)
        {
            return;
        }

        float ratio = _health.Max > 0 ? (float)_health.Current / _health.Max : 0f;

        if (_hideWhenFull && ratio >= 1f)
        {
            return;
        }

        var origin = new Vector2(-_size.X * 0.5f, _offsetY);

        DrawRect(new Rect2(origin, _size), _backgroundColor);
        DrawRect(new Rect2(origin, new Vector2(_size.X * ratio, _size.Y)), _emptyColor.Lerp(_fullColor, ratio));
        DrawRect(new Rect2(origin, _size), _borderColor, false, 1f);
    }

    private void OnHealthChanged(int current, int max)
    {
        QueueRedraw();
    }
}
