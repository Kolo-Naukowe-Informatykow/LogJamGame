using System;
using Godot;

namespace LogJamGame.Tmp;

/// <summary>
/// Współdzielony komponent zdrowia. Trzyma punkty życia i informuje o zmianach sygnałami,
/// dzięki czemu pasek zdrowia i logika śmierci nie muszą nic wiedzieć o właścicielu.
/// </summary>
public partial class Health : Node
{
    [Export] private int _maxHealth = 100;

    [Signal]
    public delegate void ChangedEventHandler(int current, int max);

    [Signal]
    public delegate void DiedEventHandler();

    public int Max => _maxHealth;

    public int Current { get; private set; }

    public bool IsDead => Current <= 0;

    public override void _EnterTree()
    {
        Current = _maxHealth;
    }

    /// <summary>Odejmuje punkty życia. Zwraca <c>true</c>, jeśli obrażenia zostały przyjęte.</summary>
    public bool TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
        {
            return false;
        }

        Current = Mathf.Max(0, Current - amount);
        EmitSignal(SignalName.Changed, Current, Max);

        if (IsDead)
        {
            EmitSignal(SignalName.Died);
        }

        return true;
    }

    /// <summary>Przywraca pełne zdrowie (np. po respawnie gracza).</summary>
    public void ResetHealth()
    {
        Current = _maxHealth;
        EmitSignal(SignalName.Changed, Current, Max);
    }
}
