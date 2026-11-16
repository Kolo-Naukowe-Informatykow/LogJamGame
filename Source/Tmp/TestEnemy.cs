using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Przeciwnik prototypu. Dwa zachowania wybierane przez <see cref="SetKind"/>:
/// melee dopływa i bije przy kontakcie, shooter trzyma dystans i strzela.
/// Ruch też jest "podwodny" — z oporem i bezwładnością.
/// </summary>
public partial class TestEnemy : CharacterBody2D
{
    public const string GroupName = "enemies";

    [Signal]
    public delegate void DiedEventHandler(Vector2 position);

    [Export] private EnemyKind _kind = EnemyKind.Melee;
    [Export] private float _acceleration = 950f;
    [Export] private float _maxSpeed = 165f;
    [Export] private float _drag = 2.6f;
    [Export] private float _hitRadius = 24f;
    [Export] private float _contactRange = 70f;
    [Export] private float _contactWindup = 0.35f;
    [Export] private int _contactDamage = 8;
    [Export] private float _contactCooldown = 0.9f;
    [Export] private float _preferredDistance = 230f;
    [Export] private float _distanceTolerance = 60f;
    [Export] private float _shootRange = 420f;
    [Export] private float _shootCooldown = 1.5f;
    [Export] private int _projectileDamage = 7;
    [Export] private float _stunDrag = 9f;
    [Export] private float _flashDuration = 0.12f;

    private readonly Color _shooterColor = new(0.7f, 0.5f, 1f);
    private readonly Color _meleeColor = new(1f, 0.4f, 0.35f);
    private readonly Color _projectileColor = new(0.85f, 0.45f, 1f);
    private readonly Color _stunColor = new(0.65f, 0.85f, 1f);
    private readonly Color _windupColor = new(1f, 0.85f, 0.3f);

    private Health _health;
    private Sprite2D _sprite;
    private TestPlayer _player;
    private Color _bodyColor = Colors.White;
    private float _attackCooldownLeft;
    private float _stunTimeLeft;
    private float _windupLeft;
    private float _flashLeft;
    private float _strafeSign = 1f;
    private bool _isDead;

    public float HitRadius => _hitRadius;

    private bool IsStunned => _stunTimeLeft > 0f;

    public override void _Ready()
    {
        AddToGroup(GroupName);

        // Przeciwnicy stoją na własnej warstwie: nie zderzają się z graczem ani ze sobą, tylko ze ścianami.
        CollisionLayer = CollisionLayers.Enemy;
        CollisionMask = CollisionLayers.World;

        _health = GetNodeOrNull<Health>("Health");
        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
        _strafeSign = GD.Randf() < 0.5f ? -1f : 1f;
        _bodyColor = _kind == EnemyKind.Melee ? _meleeColor : _shooterColor;

        if (_sprite != null)
        {
            _sprite.Modulate = _bodyColor;
        }

        if (_health == null)
        {
            GD.PushError($"{GetPath()}: brak węzła \"Health\". Dodaj go w scenie — instrukcja w Source/Tmp/PROTOTYPE_COMBAT.md.");
            SetPhysicsProcess(false);
        }
        else
        {
            _health.Died += OnHealthDied;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            return;
        }

        float dt = (float)delta;
        _attackCooldownLeft = Mathf.Max(0f, _attackCooldownLeft - dt);
        _stunTimeLeft = Mathf.Max(0f, _stunTimeLeft - dt);

        ResolvePlayer();

        if (_player != null && !IsStunned)
        {
            Vector2 toPlayer = _player.GlobalPosition - GlobalPosition;
            float distance = toPlayer.Length();

            Velocity += ComputeSteering(toPlayer, distance) * _acceleration * dt;
            TryAttack(distance, dt);
        }

        // Stunowany przeciwnik traci prędkość szybciej, więc nie sunie przez pół areny.
        float drag = IsStunned ? _stunDrag : _drag;
        Velocity *= Mathf.Exp(-drag * dt);
        Velocity = Velocity.LimitLength(_maxSpeed);
        MoveAndSlide();
        UpdateFlash(dt);
    }

    /// <summary>Ustawia zachowanie przed dodaniem przeciwnika do drzewa scen.</summary>
    public void SetKind(EnemyKind kind)
    {
        _kind = kind;
    }

    /// <summary>Odejmuje HP; przy zerze przeciwnik ginie, znika i emituje sygnał <c>Died</c>.</summary>
    public void ApplyDamage(int amount)
    {
        if (_isDead || _health == null)
        {
            return;
        }

        if (_health.TakeDamage(amount))
        {
            _flashLeft = _flashDuration;
        }
    }

    /// <summary>Dodaje impuls prędkości — używane jako odrzut po ciosie melee.</summary>
    public void ApplyKnockback(Vector2 impulse)
    {
        Velocity += impulse;
    }

    /// <summary>
    /// Ogłusza przeciwnika: przez podany czas nie rusza się i nie atakuje.
    /// Kolejne stuny nie skracają trwającego — liczy się najdłuższy.
    /// </summary>
    public void ApplyStun(float duration)
    {
        if (_isDead || duration <= 0f)
        {
            return;
        }

        // Stun przerywa rozpoczęty zamach — inaczej cios wpadłby tuż po ogłuszeniu.
        _windupLeft = 0f;
        _stunTimeLeft = Mathf.Max(_stunTimeLeft, duration);
    }

    private Vector2 ComputeSteering(Vector2 toPlayer, float distance)
    {
        if (distance <= 0.001f)
        {
            return Vector2.Zero;
        }

        Vector2 direction = toPlayer / distance;

        if (_kind == EnemyKind.Melee)
        {
            return distance > _contactRange * 0.8f ? direction : Vector2.Zero;
        }

        if (distance > _preferredDistance + _distanceTolerance)
        {
            return direction;
        }

        if (distance < _preferredDistance - _distanceTolerance)
        {
            return -direction;
        }

        return new Vector2(-direction.Y, direction.X) * _strafeSign;
    }

    private void TryAttack(float distance, float dt)
    {
        if (_kind == EnemyKind.Melee)
        {
            TickMeleeAttack(distance, dt);
            return;
        }

        if (_attackCooldownLeft > 0f)
        {
            return;
        }

        if (distance <= _shootRange)
        {
            Shoot();
            _attackCooldownLeft = _shootCooldown;
        }
    }

    private void TickMeleeAttack(float distance, float dt)
    {
        if (_windupLeft > 0f)
        {
            // Cios jest zapowiedziany (pomarańczowy odcień) — obrażenia dopiero po zamachu,
            // żeby gracz miał szansę zareagować parowaniem.
            _windupLeft -= dt;

            if (_windupLeft <= 0f && distance <= _contactRange)
            {
                StrikeMelee();
            }

            return;
        }

        if (_attackCooldownLeft > 0f || distance > _contactRange)
        {
            return;
        }

        _windupLeft = _contactWindup;
        _attackCooldownLeft = _contactWindup + _contactCooldown;
    }

    private void StrikeMelee()
    {
        if (_player.TryParryMelee(this))
        {
            // Sparowany cios nie zadaje obrażeń — gracz odrzuca i ogłusza przeciwnika.
            return;
        }

        _player.ApplyDamage(_contactDamage);
    }

    private void Shoot()
    {
        Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

        var projectile = new Projectile();
        GetParent().AddChild(projectile);
        projectile.Launch(
            GlobalPosition + direction * _hitRadius,
            direction,
            false,
            _projectileDamage,
            _projectileColor,
            Velocity,
            this);
    }

    private void ResolvePlayer()
    {
        if (_player != null && IsInstanceValid(_player) && !_player.IsQueuedForDeletion())
        {
            return;
        }

        _player = GetTree().GetFirstNodeInGroup(TestPlayer.GroupName) as TestPlayer;
    }

    private void UpdateFlash(float dt)
    {
        _flashLeft = Mathf.Max(0f, _flashLeft - dt);

        if (_sprite == null)
        {
            return;
        }

        // Błysk = trafienie, niebieski = stun, pomarańczowy = zamach przed ciosem melee.
        _sprite.Modulate = _flashLeft > 0f ? Colors.White
            : IsStunned ? _stunColor
            : _windupLeft > 0f ? _windupColor
            : _bodyColor;
    }

    private void OnHealthDied()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        EmitSignal(SignalName.Died, GlobalPosition);
        QueueFree();
    }
}
