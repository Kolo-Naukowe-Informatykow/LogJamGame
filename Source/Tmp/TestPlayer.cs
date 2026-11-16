using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Gracz prototypu combatu. Ruch jest "podwodny": przyspieszamy powoli, a po puszczeniu
/// klawiszy płyniemy dalej, bo prędkość wygasa przez opór. Do tego celowanie myszką
/// z ograniczoną prędkością obrotu, strzał, parowanie, dash na ładunkach i atak melee.
/// </summary>
public partial class TestPlayer : CharacterBody2D
{
    public const string GroupName = "player";

    // Ruch w wodzie: niska akceleracja i opór sprawiają, że gracz rozpędza się i skręca ospale.
    [Export] private float _acceleration = 1100f;
    [Export] private float _maxSpeed = 260f;
    [Export] private float _drag = 3.2f;

    // Celowanie: kierunek obraca się w stronę kursora z ograniczoną prędkością kątową.
    [Export] private float _aimTurnSpeedDegrees = 450f;

    // Dash: ładunki ładowane w tle — nie da się nim spamować.
    [Export] private float _dashSpeed = 950f;
    [Export] private float _dashDuration = 0.16f;
    [Export] private int _maxDashCharges = 2;
    [Export] private float _dashRechargeTime = 1.1f;

    [Export] private float _shootCooldown = 0.18f;
    [Export] private int _shootDamage = 10;
    [Export] private float _muzzleDistance = 40f;
    [Export] private float _meleeCooldown = 0.6f;
    [Export] private float _meleeDuration = 0.18f;
    [Export] private float _meleeRange = 130f;
    [Export] private float _meleeArcDegrees = 100f;
    [Export] private int _meleeDamage = 25;
    [Export] private float _meleeKnockback = 420f;

    // Parowanie: krótkie okno po wciśnięciu klawisza. Udane — odbicie pocisku i stun przeciwnika,
    // nieudane — dłuższa kara (cooldown). W oknie i chwilę po nim nie można strzelać ani bić melee.
    [Export] private float _parryDuration = 0.32f;
    [Export] private float _parryRecovery = 0.14f;
    [Export] private float _parryCooldown = 0.35f;
    [Export] private float _parryFailCooldown = 0.7f;
    [Export] private float _parryStunDuration = 0.9f;
    [Export] private float _parryBurstDuration = 0.25f;
    [Export] private float _parryMeleeKnockback = 480f;
    [Export] private float _flashDuration = 0.12f;

    private static readonly Color DamageFlashColor = new(1f, 0.45f, 0.45f);
    private static readonly Color ParryFlashColor = new(0.55f, 1f, 0.95f);

    private Health _health;
    private Sprite2D _sprite;
    private Vector2 _aimDirection = Vector2.Right;
    private Vector2 _dashDirection = Vector2.Right;
    private float _dashTimeLeft;
    private float _dashCharges;
    private float _shootCooldownLeft;
    private float _meleeCooldownLeft;
    private float _meleeTimer;
    private float _parryTimeLeft;
    private float _parryCooldownLeft;
    private float _parryCooldownTotal = 1f;
    private float _parryRecoveryLeft;
    private float _parryBurstLeft;
    private float _flashLeft;
    private Color _flashColor = DamageFlashColor;
    private bool _meleeHeld;
    private bool _parryHeld;
    private bool _dashHeld;
    private bool _isInvulnerable;

    public bool IsInvulnerable => _isInvulnerable;

    public bool IsDead => _health != null && _health.IsDead;

    public override void _Ready()
    {
        AddToGroup(GroupName);

        // Osobna warstwa dla gracza: nie zderza się z przeciwnikami, tylko ze ścianami.
        CollisionLayer = CollisionLayers.Player;
        CollisionMask = CollisionLayers.World;
        _dashCharges = _maxDashCharges;

        _health = GetNodeOrNull<Health>("Health");
        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");

        if (_health == null)
        {
            GD.PushError($"{GetPath()}: brak węzła \"Health\". Dodaj go w scenie — instrukcja w Source/Tmp/PROTOTYPE_COMBAT.md.");
            SetPhysicsProcess(false);
        }
        else
        {
            _health.Died += OnDied;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        TickTimers(dt);

        if (IsDead)
        {
            Velocity = Vector2.Zero;
            return;
        }

        Vector2 input = ReadMoveInput();
        UpdateAim(GetGlobalMousePosition() - GlobalPosition, dt);

        HandleActions(input);

        if (_dashTimeLeft > 0f)
        {
            // Podczas dashu płyniemy jak torpeda — bez oporu i bez sterowania.
            _dashTimeLeft -= dt;
            Velocity = _dashDirection * _dashSpeed;
        }
        else
        {
            Velocity += input * _acceleration * dt;
            Velocity *= Mathf.Exp(-_drag * dt);
            Velocity = Velocity.LimitLength(_maxSpeed);
        }

        MoveAndSlide();
        UpdateFlash(dt);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawLine(Vector2.Zero, _aimDirection * _muzzleDistance * 3f, new Color(1f, 1f, 1f, 0.12f), 2f);
        DrawDashCharges();
        DrawParryVisuals();

        if (_meleeTimer > 0f)
        {
            float progress = 1f - _meleeTimer / _meleeDuration;
            float halfArc = Mathf.DegToRad(_meleeArcDegrees) * 0.5f;
            float center = _aimDirection.Angle();
            float radius = _meleeRange * Mathf.Lerp(0.6f, 1f, progress);
            var color = new Color(1f, 1f, 1f, 0.55f * (1f - progress));

            DrawArc(Vector2.Zero, radius, center - halfArc, center + halfArc, 24, color, 6f);
        }
    }

    /// <summary>Przyjmuje obrażenia z pocisków i kontaktu z przeciwnikiem.</summary>
    public void ApplyDamage(int amount)
    {
        if (_isInvulnerable || _health == null || IsDead)
        {
            return;
        }

        if (_health.TakeDamage(amount))
        {
            _flashColor = DamageFlashColor;
            _flashLeft = _flashDuration;
        }
    }

    public void SetInvulnerable(bool value)
    {
        _isInvulnerable = value;
    }

    /// <summary>Przenosi gracza na pozycję startową i przywraca pełne zdrowie.</summary>
    public void Respawn(Vector2 position)
    {
        GlobalPosition = position;
        Velocity = Vector2.Zero;
        _flashLeft = 0f;

        if (_sprite != null)
        {
            _sprite.Modulate = Colors.White;
        }

        _health?.ResetHealth();
    }

    /// <summary>
    /// Próbuje sparować nadlatujący pocisk. Udane parowanie zamyka okno parowania,
    /// odwraca pocisk w stronę przeciwników i ogłusza strzelca.
    /// </summary>
    public bool TryParryProjectile(Projectile projectile)
    {
        if (projectile == null || !ConsumeParryWindow())
        {
            return false;
        }

        projectile.Reflect(_parryStunDuration);
        return true;
    }

    /// <summary>
    /// Próbuje sparować cios melee. Udane parowanie zamyka okno parowania,
    /// nie zadaje obrażeń, odrzuca przeciwnika i go ogłusza.
    /// </summary>
    public bool TryParryMelee(TestEnemy attacker)
    {
        if (attacker == null || !ConsumeParryWindow())
        {
            return false;
        }

        Vector2 away = attacker.GlobalPosition - GlobalPosition;
        away = away.LengthSquared() > 1f ? away.Normalized() : Vector2.Left;
        attacker.ApplyKnockback(away * _parryMeleeKnockback);
        attacker.ApplyStun(_parryStunDuration);

        return true;
    }

    private static Vector2 ReadMoveInput()
    {
        // Klawisze czytamy wprost, żeby prototyp nie wymagał zmian w Input Map.
        Vector2 input = Vector2.Zero;

        if (Input.IsPhysicalKeyPressed(Key.W))
        {
            input.Y -= 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.S))
        {
            input.Y += 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.A))
        {
            input.X -= 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.D))
        {
            input.X += 1f;
        }

        return input.LengthSquared() > 1f ? input.Normalized() : input;
    }

    private void TickTimers(float dt)
    {
        _shootCooldownLeft = Mathf.Max(0f, _shootCooldownLeft - dt);
        _meleeCooldownLeft = Mathf.Max(0f, _meleeCooldownLeft - dt);
        _meleeTimer = Mathf.Max(0f, _meleeTimer - dt);
        _parryCooldownLeft = Mathf.Max(0f, _parryCooldownLeft - dt);
        _parryRecoveryLeft = Mathf.Max(0f, _parryRecoveryLeft - dt);
        _parryBurstLeft = Mathf.Max(0f, _parryBurstLeft - dt);
        _dashCharges = Mathf.Min((float)_maxDashCharges, _dashCharges + dt / _dashRechargeTime);

        if (_parryTimeLeft > 0f)
        {
            _parryTimeLeft -= dt;

            if (_parryTimeLeft <= 0f)
            {
                // Okno minęło, a nic nie zostało sparowane — to nieudane parowanie, czyli kara.
                FinishParry(false);
            }
        }
    }

    private void UpdateAim(Vector2 toMouse, float dt)
    {
        if (toMouse.LengthSquared() <= 1f)
        {
            // Zerowy wektor zdarza się, gdy kursor leży dokładnie na graczu — wtedy zostaje poprzedni kierunek.
            return;
        }

        // Celownik obraca się ograniczoną prędkością, więc szybkie ruchy myszką go „gubią”.
        float maxStep = Mathf.DegToRad(_aimTurnSpeedDegrees) * dt;
        float step = Mathf.Clamp(Mathf.AngleDifference(_aimDirection.Angle(), toMouse.Angle()), -maxStep, maxStep);
        _aimDirection = _aimDirection.Rotated(step);
    }

    private void HandleActions(Vector2 input)
    {
        // Wszystkie trzy akcje reagują tylko na wciśnięcie — trzymanie klawisza nic nie powtarza.
        bool dashDown = Input.IsPhysicalKeyPressed(Key.Space);
        bool dashPressed = dashDown && !_dashHeld;
        _dashHeld = dashDown;

        bool parryDown = Input.IsPhysicalKeyPressed(Key.Shift) || Input.IsKeyPressed(Key.Shift);
        bool parryPressed = parryDown && !_parryHeld;
        _parryHeld = parryDown;

        bool meleeDown = Input.IsMouseButtonPressed(MouseButton.Right);
        bool meleePressed = meleeDown && !_meleeHeld;
        _meleeHeld = meleeDown;

        if (dashPressed && _dashCharges >= 1f)
        {
            StartDash(input);
        }

        if (parryPressed && _parryCooldownLeft <= 0f && _parryTimeLeft <= 0f)
        {
            StartParry();
        }

        if (GetViewport().GuiGetHoveredControl() != null)
        {
            // Mysz jest nad panelem debug — nie chcemy, żeby klik w przycisk też strzelał.
            return;
        }

        if (IsActionLocked)
        {
            // W oknie parowania i chwilę po nim nie można strzelać ani bić melee.
            return;
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left) && _shootCooldownLeft <= 0f)
        {
            Shoot();
        }

        if (meleePressed && _meleeCooldownLeft <= 0f)
        {
            Swing();
        }
    }

    private void StartDash(Vector2 input)
    {
        _dashDirection = input == Vector2.Zero ? _aimDirection : input.Normalized();
        _dashTimeLeft = _dashDuration;
        _dashCharges -= 1f;
    }

    private void StartParry()
    {
        _parryTimeLeft = _parryDuration;
        _parryRecoveryLeft = 0f;
        _parryBurstLeft = 0f;
    }

    private void FinishParry(bool success)
    {
        _parryTimeLeft = 0f;
        _parryRecoveryLeft = _parryRecovery;
        _parryCooldownTotal = success ? _parryCooldown : _parryFailCooldown;
        _parryCooldownLeft = _parryCooldownTotal;

        if (success)
        {
            _parryBurstLeft = _parryBurstDuration;
        }
    }

    private bool IsActionLocked => _parryTimeLeft > 0f || _parryRecoveryLeft > 0f;

    private bool ConsumeParryWindow()
    {
        if (_parryTimeLeft <= 0f || IsDead)
        {
            return false;
        }

        FinishParry(true);
        _flashColor = ParryFlashColor;
        _flashLeft = _flashDuration;

        return true;
    }

    private void Shoot()
    {
        _shootCooldownLeft = _shootCooldown;

        var projectile = new Projectile();
        GetParent().AddChild(projectile);
        projectile.Launch(
            GlobalPosition + _aimDirection * _muzzleDistance,
            _aimDirection,
            true,
            _shootDamage,
            null,
            Velocity);
    }

    private void Swing()
    {
        _meleeCooldownLeft = _meleeCooldown;
        _meleeTimer = _meleeDuration;

        float halfArc = Mathf.DegToRad(_meleeArcDegrees) * 0.5f;

        foreach (Node node in GetTree().GetNodesInGroup(TestEnemy.GroupName))
        {
            if (node is not TestEnemy enemy)
            {
                continue;
            }

            Vector2 toEnemy = enemy.GlobalPosition - GlobalPosition;

            if (toEnemy.Length() > _meleeRange + enemy.HitRadius)
            {
                continue;
            }

            if (Mathf.Abs(_aimDirection.AngleTo(toEnemy.Normalized())) > halfArc)
            {
                continue;
            }

            enemy.ApplyDamage(_meleeDamage);
            enemy.ApplyKnockback(toEnemy.Normalized() * _meleeKnockback);
        }
    }

    private void DrawParryVisuals()
    {
        if (_parryTimeLeft > 0f)
        {
            // Aktywne okno parowania: pulsujący pierścień wokół gracza. Promień jest większy niż
            // połowa ikony gracza, bo rysowanie własne węzła idzie pod jego dziećmi (sprite'em).
            float progress = 1f - Mathf.Clamp(_parryTimeLeft / _parryDuration, 0f, 1f);
            float alpha = 0.6f + 0.4f * Mathf.Sin(progress * Mathf.Pi);

            DrawCircle(Vector2.Zero, 78f, new Color(0.6f, 1f, 0.95f, 0.1f));
            DrawArc(Vector2.Zero, 72f, 0f, Mathf.Tau, 40, new Color(0.6f, 1f, 0.95f, alpha), 7f);
            DrawArc(Vector2.Zero, 84f, 0f, Mathf.Tau, 40, new Color(0.6f, 1f, 0.95f, alpha * 0.5f), 2f);
        }
        else if (_parryRecoveryLeft > 0f)
        {
            // Chwila po parowaniu: akcje zablokowane, więc pierścień jest ciemniejszy i cienki.
            DrawArc(Vector2.Zero, 72f, 0f, Mathf.Tau, 40, new Color(0.9f, 0.6f, 0.6f, 0.4f), 2f);
        }
        else if (_parryCooldownLeft > 0f)
        {
            // Ładowanie po parowaniu: rosnący łuk pokazuje, kiedy znowu można sparować.
            float ready = 1f - Mathf.Clamp(_parryCooldownLeft / Mathf.Max(_parryCooldownTotal, 0.001f), 0f, 1f);
            DrawArc(Vector2.Zero, 84f, -Mathf.Pi * 0.5f, -Mathf.Pi * 0.5f + Mathf.Tau * ready, 40, new Color(0.6f, 0.9f, 1f, 0.45f), 3f);
        }

        if (_parryBurstLeft <= 0f)
        {
            return;
        }

        // Udane parowanie: rozchodzący się krąg jako potwierdzenie trafienia.
        float burst = 1f - _parryBurstLeft / _parryBurstDuration;
        float radius = Mathf.Lerp(66f, 132f, burst);
        DrawArc(Vector2.Zero, radius, 0f, Mathf.Tau, 48, new Color(0.65f, 1f, 0.95f, 0.9f * (1f - burst)), 6f);
    }

    private void DrawDashCharges()
    {
        if (_maxDashCharges <= 0)
        {
            return;
        }

        var emptyColor = new Color(0.5f, 0.9f, 1f, 0.25f);
        var fullColor = new Color(0.65f, 0.95f, 1f, 0.9f);
        float startX = -(_maxDashCharges - 1) * 18f * 0.5f;

        // Rząd nad graczem, poza sprite'em — mniejsze promienie chowają się pod ikoną.
        for (int i = 0; i < _maxDashCharges; i++)
        {
            var center = new Vector2(startX + i * 18f, -88f);
            float fill = Mathf.Clamp(_dashCharges - i, 0f, 1f);

            DrawArc(center, 6f, 0f, Mathf.Tau, 12, emptyColor, 2f);

            if (fill > 0f)
            {
                DrawCircle(center, 6f * fill, fullColor);
            }
        }
    }

    private void UpdateFlash(float dt)
    {
        if (_flashLeft <= 0f)
        {
            return;
        }

        _flashLeft -= dt;

        if (_sprite != null)
        {
            _sprite.Modulate = _flashLeft > 0f ? _flashColor : Colors.White;
        }
    }

    private void OnDied()
    {
        Velocity = Vector2.Zero;
    }
}
