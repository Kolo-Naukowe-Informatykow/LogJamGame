using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Pocisk 2D zwalniający w wodzie: opór wykładniczo wygasza prędkość, więc strzał ma
/// ograniczony zasięg. Nie wymaga własnej sceny — tworzymy go z kodu przez <c>new Projectile()</c>.
/// </summary>
public partial class Projectile : Area2D
{
    [Export] private float _speed = 620f;
    [Export] private int _damage = 10;
    [Export] private float _lifetime = 2.5f;
    [Export] private float _drag = 1.6f;
    [Export] private float _radius = 5f;
    [Export] private float _trailLength = 18f;
    [Export] private float _reflectSpeedMultiplier = 1.3f;
    [Export] private float _maxInheritedSpeed = 350f;
    [Export] private Color _color = new(1f, 0.92f, 0.55f);

    private static readonly Color ReflectedColor = new(0.55f, 1f, 0.9f);

    private Vector2 _velocity;
    private float _lifeLeft;
    private Node2D _shooter;
    private bool _fromPlayer;
    private bool _spent;

    public override void _Ready()
    {
        // Pocisk nie jest celem dla innych pocisków — to my wykrywamy trafienia.
        CollisionLayer = 0;
        CollisionMask = CollisionLayers.World | CollisionLayers.Player | CollisionLayers.Enemy;
        Monitorable = false;
        Monitoring = true;

        if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D") == null)
        {
            AddChild(new CollisionShape2D
            {
                Name = "CollisionShape2D",
                Shape = new CircleShape2D { Radius = _radius },
            });
        }

        BodyEntered += OnBodyEntered;
        _lifeLeft = _lifetime;
    }

    /// <summary>
    /// Ustawia tor lotu. Wywołaj po dodaniu pocisku do drzewa scen.
    /// <paramref name="shooterVelocity"/> dodaje tylko składową zgodną z kierunkiem strzału,
    /// więc strzał w stronę ruchu jest szybszy, a strzał w tył nie jest spowalniany.
    /// <paramref name="shooter"/> zapamiętujemy, żeby po sparowaniu ogłuszyć strzelca.
    /// </summary>
    public void Launch(
        Vector2 position,
        Vector2 direction,
        bool fromPlayer,
        int damage = 0,
        Color? color = null,
        Vector2 shooterVelocity = default,
        Node2D shooter = null)
    {
        Vector2 heading = direction.Normalized();
        float carriedSpeed = Mathf.Clamp(shooterVelocity.Dot(heading), 0f, _maxInheritedSpeed);

        GlobalPosition = position;
        Rotation = heading.Angle();
        _velocity = heading * (_speed + carriedSpeed);
        _fromPlayer = fromPlayer;
        _shooter = shooter;

        if (damage > 0)
        {
            _damage = damage;
        }

        if (color.HasValue)
        {
            _color = color.Value;
        }

        QueueRedraw();
    }

    /// <summary>
    /// Odbija pocisk w przeciwną stronę. Po odbiciu trafia w przeciwników,
    /// a strzelec dostaje stun na <paramref name="stunDuration"/>.
    /// </summary>
    public void Reflect(float stunDuration = 0f)
    {
        Vector2 heading = _velocity.LengthSquared() > 1f ? -_velocity.Normalized() : Vector2.Left;

        _fromPlayer = true;
        _velocity = heading * _speed * _reflectSpeedMultiplier;
        _lifeLeft = _lifetime;
        _color = ReflectedColor;
        Rotation = heading.Angle();
        QueueRedraw();

        if (stunDuration > 0f && _shooter is TestEnemy enemy && IsInstanceValid(enemy))
        {
            enemy.ApplyStun(stunDuration);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        _lifeLeft -= dt;

        if (_lifeLeft <= 0f)
        {
            QueueFree();
            return;
        }

        _velocity *= Mathf.Exp(-_drag * dt);
        Position += _velocity * dt;
    }

    public override void _Draw()
    {
        // Rysujemy w lokalnym układzie, więc po obrocie ogon zostaje z tyłu pocisku.
        DrawLine(new Vector2(-_trailLength, 0f), Vector2.Zero, new Color(_color, 0.25f), _radius * 1.2f);
        DrawCircle(Vector2.Zero, _radius, _color);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_spent)
        {
            return;
        }

        if (_fromPlayer && body is TestEnemy enemy)
        {
            enemy.ApplyDamage(_damage);
        }
        else if (!_fromPlayer && body is TestPlayer player)
        {
            if (player.TryParryProjectile(this))
            {
                // Sparowany pocisk leci dalej, ale już jako pocisk gracza.
                return;
            }

            player.ApplyDamage(_damage);
        }
        else if (body is not StaticBody2D)
        {
            // Obce ciała (np. wystrzelone pociski) ignorujemy.
            return;
        }

        _spent = true;
        QueueFree();
    }
}
