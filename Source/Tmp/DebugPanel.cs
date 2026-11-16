using System;
using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Narzędzie debug prototypu: panel z przyciskami do stawiania przeciwników, god mode,
/// slow motion i statystyki. Całe UI powstaje w kodzie, więc scena nie wymaga zmian.
/// </summary>
public partial class DebugPanel : CanvasLayer
{
    private const float StatsRefreshInterval = 0.2f;

    private TestWorld _world;
    private Label _statsLabel;
    private float _statsTimer;
    private bool _godMode;
    private bool _slowMotion;

    public void Initialize(TestWorld world)
    {
        _world = world;
    }

    public override void _Ready()
    {
        var panel = new PanelContainer { Position = new Vector2(16f, 16f) };
        AddChild(panel);

        var column = new VBoxContainer();
        panel.AddChild(column);

        _statsLabel = new Label();
        column.AddChild(_statsLabel);

        column.AddChild(MakeButton("Przeciwnik melee — F1", () => SpawnAtMouse(EnemyKind.Melee)));
        column.AddChild(MakeButton("Przeciwnik strzelec — F2", () => SpawnAtMouse(EnemyKind.Shooter)));
        column.AddChild(MakeButton("Fala x5 przy myszy — F3", () => SpawnWave(5)));
        column.AddChild(MakeButton("Wyczyść arenę — F4", () => _world?.KillAllEnemies()));
        column.AddChild(MakeButton("God mode — F5", ToggleGodMode));
        column.AddChild(MakeButton("Slow motion — F6", ToggleSlowMotion));
        column.AddChild(MakeButton("Reset gracza — F7", () => _world?.TeleportPlayerHome()));

        column.AddChild(new Label
        {
            Text = "WASD — ruch | mysz — celowanie | LPM — strzał\nPPM — melee | Shift — parowanie | Spacja — dash",
        });
    }

    public override void _Process(double delta)
    {
        _statsTimer -= (float)delta;

        if (_statsTimer > 0f)
        {
            return;
        }

        _statsTimer = StatsRefreshInterval;

        string kills = _world == null ? "0" : _world.Kills.ToString();
        string enemies = _world == null ? "0" : _world.EnemyCount.ToString();

        _statsLabel.Text = $"Zabici: {kills}   Przeciwnicy: {enemies}   FPS: {Engine.GetFramesPerSecond()}";
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
        {
            return;
        }

        switch (key.PhysicalKeycode)
        {
            case Key.F1:
                SpawnAtMouse(EnemyKind.Melee);
                break;
            case Key.F2:
                SpawnAtMouse(EnemyKind.Shooter);
                break;
            case Key.F3:
                SpawnWave(5);
                break;
            case Key.F4:
                _world?.KillAllEnemies();
                break;
            case Key.F5:
                ToggleGodMode();
                break;
            case Key.F6:
                ToggleSlowMotion();
                break;
            case Key.F7:
                _world?.TeleportPlayerHome();
                break;
            default:
                return;
        }

        GetViewport().SetInputAsHandled();
    }

    private static Button MakeButton(string text, Action pressed)
    {
        var button = new Button
        {
            Text = text,
            // Bez focusu, żeby Space (dash) i Enter nie klikały przycisku.
            FocusMode = Control.FocusModeEnum.None,
        };

        button.Pressed += pressed;
        return button;
    }

    private void SpawnAtMouse(EnemyKind kind)
    {
        _world?.SpawnEnemyAtMouse(kind);
    }

    private void SpawnWave(int count)
    {
        if (_world == null)
        {
            return;
        }

        Vector2 center = _world.GetGlobalMousePosition();

        for (int i = 0; i < count; i++)
        {
            float angle = Mathf.Tau * i / count;
            var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 140f;
            EnemyKind kind = i % 3 == 2 ? EnemyKind.Shooter : EnemyKind.Melee;
            _world.SpawnEnemy(kind, center + offset);
        }
    }

    private void ToggleGodMode()
    {
        _godMode = !_godMode;
        _world?.SetPlayerInvulnerable(_godMode);
    }

    private void ToggleSlowMotion()
    {
        _slowMotion = !_slowMotion;
        Engine.TimeScale = _slowMotion ? 0.35f : 1.0f;
    }
}
