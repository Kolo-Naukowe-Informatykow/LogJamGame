using Godot;

namespace LogJamGame.UI.Screens.SplashScreen;

public partial class SplashScreen : Control
{
    [Export] public PackedScene NextScene { get; set; }

    [Export] public NodePath LogoPath { get; set; } = "MarginContainer/Logo";

    [Export(PropertyHint.Range, "0,10,0.05")]
    public float FadeInSeconds { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0,10,0.05")]
    public float HoldSeconds { get; set; } = 1.5f;

    [Export(PropertyHint.Range, "0,10,0.05")]
    public float FadeOutSeconds { get; set; } = 0.6f;

    [Export(PropertyHint.Range, "0,2,0.05")]
    public float SkipFadeOutSeconds { get; set; } = 0.15f;

    private Control _logo;
    private Tween _tween;
    private bool _isLeaving;

    public override void _Ready()
    {
        _logo = GetNode<Control>(LogoPath);

        Color modulate = _logo.Modulate;
        modulate.A = 0f;
        _logo.Modulate = modulate;

        _tween = CreateTween();
        _tween.TweenProperty(_logo, "modulate:a", 1.0f, FadeInSeconds)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        _tween.TweenInterval(HoldSeconds);
        _tween.TweenProperty(_logo, "modulate:a", 0.0f, FadeOutSeconds)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        _tween.TweenCallback(Callable.From(GoToNextScene));
    }

    public override void _Input(InputEvent @event)
    {
        if (_isLeaving || !IsSkipInput(@event))
        {
            return;
        }

        GetViewport().SetInputAsHandled();
        Skip();
    }

    private static bool IsSkipInput(InputEvent @event)
    {
        // Ruch myszy celowo pominięty
        return @event switch
        {
            InputEventKey key => key.Pressed && !key.Echo,
            InputEventMouseButton button => button.Pressed,
            InputEventJoypadButton joypadButton => joypadButton.Pressed,
            InputEventScreenTouch touch => touch.Pressed,
            _ => false,
        };
    }

    private void Skip()
    {
        _isLeaving = true;
        _tween?.Kill();

        if (SkipFadeOutSeconds <= 0f)
        {
            GoToNextScene();
            return;
        }

        _tween = CreateTween();
        _tween.TweenProperty(_logo, "modulate:a", 0.0f, SkipFadeOutSeconds);
        _tween.TweenCallback(Callable.From(GoToNextScene));
    }

    private void GoToNextScene()
    {
        _isLeaving = true;

        if (NextScene == null)
        {
            GD.PushWarning("SplashScreen: nie ustawiono NextScene w inspektorze — zostaję na splashu.");
            SetProcessInput(false);
            return;
        }

        GetTree().ChangeSceneToPacked(NextScene);
    }
}
