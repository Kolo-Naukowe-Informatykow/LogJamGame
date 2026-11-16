using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Arena prototypu combatu: buduje ściany, rysuje tło "wody", pilnuje respawnu gracza
/// i udostępnia spawn przeciwników dla narzędzia debug.
/// </summary>
public partial class TestWorld : Node2D
{
    private const string EnemyScenePath = "res://Source/Tmp/TestEnemy.tscn";

    [Export] private PackedScene _enemyScene;
    [Export] private Vector2 _arenaSize = new(1600f, 900f);
    [Export] private float _wallThickness = 48f;
    [Export] private int _maxEnemies = 30;
    [Export] private float _playerRespawnDelay = 1.2f;
    [Export] private Color _waterColor = new(0.04f, 0.14f, 0.22f);
    [Export] private Color _gridColor = new(1f, 1f, 1f, 0.045f);
    [Export] private float _gridStep = 100f;

    private TestPlayer _player;
    private Vector2 _playerSpawnPoint;
    private float _respawnLeft = -1f;
    private int _kills;

    public int Kills => _kills;

    public int EnemyCount => GetTree() == null ? 0 : GetTree().GetNodeCountInGroup(TestEnemy.GroupName);

    public bool HasPlayer => _player != null && IsInstanceValid(_player) && !_player.IsQueuedForDeletion();

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup(TestPlayer.GroupName) as TestPlayer;

        if (_player == null)
        {
            GD.PushWarning("TestWorld: nie znaleziono gracza w grupie \"player\".");
        }
        else
        {
            _playerSpawnPoint = _player.GlobalPosition;

            Health playerHealth = _player.GetNodeOrNull<Health>("Health");

            if (playerHealth != null)
            {
                playerHealth.Died += OnPlayerDied;
            }
        }

        BuildArena();
        ConnectEnemySignals();

        var debugPanel = new DebugPanel();
        debugPanel.Initialize(this);
        AddChild(debugPanel);
    }

    public override void _Process(double delta)
    {
        if (_respawnLeft < 0f)
        {
            return;
        }

        _respawnLeft -= (float)delta;

        if (_respawnLeft > 0f || !HasPlayer)
        {
            return;
        }

        _respawnLeft = -1f;
        _player.Respawn(_playerSpawnPoint);
    }

    public override void _Draw()
    {
        var rect = new Rect2(-_arenaSize * 0.5f, _arenaSize);

        DrawRect(rect, _waterColor);

        for (float x = rect.Position.X; x <= rect.End.X; x += _gridStep)
        {
            DrawLine(new Vector2(x, rect.Position.Y), new Vector2(x, rect.End.Y), _gridColor);
        }

        for (float y = rect.Position.Y; y <= rect.End.Y; y += _gridStep)
        {
            DrawLine(new Vector2(rect.Position.X, y), new Vector2(rect.End.X, y), _gridColor);
        }

        DrawRect(rect, new Color(0.5f, 0.85f, 1f, 0.35f), false, 3f);
    }

    /// <summary>Tworzy przeciwnika wskazanego rodzaju na podanej pozycji świata.</summary>
    public TestEnemy SpawnEnemy(EnemyKind kind, Vector2 position)
    {
        if (EnemyCount >= _maxEnemies)
        {
            GD.Print($"TestWorld: limit przeciwników ({_maxEnemies}) osiągnięty.");
            return null;
        }

        PackedScene scene = _enemyScene;

        if (scene == null)
        {
            // Fallback, żeby prototyp działał nawet bez wpisania sceny w inspektorze.
            scene = GD.Load<PackedScene>(EnemyScenePath);
        }

        if (scene == null)
        {
            GD.PushError($"TestWorld: nie znaleziono sceny przeciwnika: {EnemyScenePath}");
            return null;
        }

        var enemy = scene.Instantiate<TestEnemy>();
        enemy.SetKind(kind);
        AddChild(enemy);
        enemy.GlobalPosition = position;
        enemy.Died += OnEnemyDied;

        return enemy;
    }

    public TestEnemy SpawnEnemyAtMouse(EnemyKind kind)
    {
        return SpawnEnemy(kind, GetGlobalMousePosition());
    }

    public void KillAllEnemies()
    {
        foreach (Node node in GetTree().GetNodesInGroup(TestEnemy.GroupName))
        {
            if (node is TestEnemy enemy)
            {
                enemy.ApplyDamage(999999);
            }
        }
    }

    public void SetPlayerInvulnerable(bool value)
    {
        if (HasPlayer)
        {
            _player.SetInvulnerable(value);
        }
    }

    public void TeleportPlayerHome()
    {
        if (HasPlayer)
        {
            _player.Respawn(_playerSpawnPoint);
        }
    }

    private void BuildArena()
    {
        var walls = new StaticBody2D { Name = "ArenaWalls" };
        AddChild(walls);

        Vector2 half = _arenaSize * 0.5f;
        float thickness = _wallThickness;

        AddWall(walls, new Vector2(0f, -half.Y - thickness * 0.5f), new Vector2(_arenaSize.X + thickness * 2f, thickness));
        AddWall(walls, new Vector2(0f, half.Y + thickness * 0.5f), new Vector2(_arenaSize.X + thickness * 2f, thickness));
        AddWall(walls, new Vector2(-half.X - thickness * 0.5f, 0f), new Vector2(thickness, _arenaSize.Y));
        AddWall(walls, new Vector2(half.X + thickness * 0.5f, 0f), new Vector2(thickness, _arenaSize.Y));
    }

    private static void AddWall(Node parent, Vector2 position, Vector2 size)
    {
        parent.AddChild(new CollisionShape2D
        {
            Position = position,
            Shape = new RectangleShape2D { Size = size },
        });
    }

    private void ConnectEnemySignals()
    {
        foreach (Node node in GetTree().GetNodesInGroup(TestEnemy.GroupName))
        {
            if (node is TestEnemy enemy)
            {
                enemy.Died += OnEnemyDied;
            }
        }
    }

    private void OnEnemyDied(Vector2 position)
    {
        _kills++;
    }

    private void OnPlayerDied()
    {
        _respawnLeft = _playerRespawnDelay;
    }
}
