# Zasady pisania kodu

Dokument obowiązuje w całym repozytorium. Nie chodzi o estetykę — chodzi o to, żeby po otwarciu dowolnego pliku każdy od razu wiedział, gdzie co jest i jak to czytać.

Cześć reguł jest automatycznie pilnowana przez `.editorconfig` (VS Code, Rider i Visual Studio czytają ten plik), ale zasady nazewnictwa i struktury sprawdzamy w code review.

## Spis treści

- [1. Nazewnictwo](#1-nazewnictwo)
- [2. Formatowanie](#2-formatowanie)
- [3. Zasady pracy z Godotem](#3-zasady-pracy-z-godotem)
- [4. Komentarze](#4-komentarze)
- [5. Struktura plików i folderów](#5-struktura-plików-i-folderów)
- [6. Przykładowy plik](#6-przykładowy-plik)

---

## 1. Nazewnictwo

### Foldery, pliki i sceny — PascalCase

| Element | Przykład |
|---|---|
| Folder | `Components/`, `UI/`, `Player/` |
| Scena `.tscn` | `MainMenu.tscn`, `Player.tscn` |
| Skrypt C# `.cs` | `MainMenu.cs`, `PlayerController.cs` |
| Klasa C# | `public partial class PlayerController` |
| Zasób Godota `.tres` | `DefaultTheme.tres` |

Nazwa pliku `.cs` musi być **identyczna** z nazwą klasy — Godot wyszukuje skrypty po nazwie pliku.

### Skrypt przy scenie

Jeżeli kod należy do jednej sceny, plik `.cs` leży **w tym samym folderze** co scena `.tscn`, do której jest przypisany:

```text
Source/Player/
├── Player.tscn             ← scena
└── Player.cs               ← skrypt przypisany do tej sceny
```

Jeśli skrypt jest używany przez wiele scen (np. komponent paska zdrowia), to nie jest „skryptem przy scenie” — należy do `Source/Components/`.

### Elementy kodu C#

| Element | Konwencja | Przykład |
|---|---|---|
| Klasa, struktura, enum, rekord | PascalCase | `PlayerController`, `GameState` |
| Interfejs | `I` + PascalCase | `IDamageable`, `IInteractable` |
| Metoda | PascalCase | `TakeDamage()`, `SpawnEnemy()` |
| Właściwość (property) | PascalCase | `MoveSpeed`, `IsGrounded` |
| Stała (`const`, `static readonly`) | PascalCase | `MaxHealth`, `DefaultGravity` |
| Pole prywatne | `_` + camelCase | `_moveSpeed`, `_playerNode` |
| Pole publiczne | **nie używamy** | zamiast tego właściwość albo `[Export]` |
| Zmienna lokalna, parametr | camelCase | `health`, `targetPosition` |
| Nazwa pliku `.cs` | PascalCase, jak klasa | `PlayerController.cs` |

Dodatkowo:

- **Skrót `Id` piszemy jako `Id`**, nie `ID` (np. `PlayerId`).
- **Interfejsy zaczynamy od wielkiego `I`**, żeby odróżnić je od klas.
- `[Export]` na polu prywatnym też zapisujemy z podkreśleniem — Godot pokaże w inspektorze ładną nazwę bez podkreślenia (`_maxHealth` → „Max Health”):

```csharp
[Export] private float _moveSpeed = 220f;
```

### Pełne nazwy, bez skrótów

Piszemy nazwy opisowe: `health`, nie `hp`; `button`, nie `btn`; `manager`, nie `mgr`; `damage`, nie `dmg`. Wyjątki to utarte skróty: `ui`, `id`, `fps`, `sprite`, `sfx`.

Zmienne jednoliterowe dopuszczamy tylko jako liczniki pętli (`i`, `j`) i w wyrażeniach matematycznych.

### Język

- **Identyfikatory w kodzie po angielsku** (`playerHealth`, `TakeDamage`) — to standard w C# i w Godocie.
- **Komentarze, dokumentacja i komunikaty commitów po polsku.** (głównie dokumentacja)

### Nazwy węzłów i sygnałów

| Element | Konwencja | Przykład |
|---|---|---|
| Nazwa węzła w scenie | PascalCase | `HealthBar`, `StartButton` |
| Sygnał (`[Signal]`) | PascalCase + `EventHandler` | `public delegate void DiedEventHandler();` → sygnał `Died` |
| Nazwa akcji w Input Map | PascalCase | `MoveLeft`, `Jump` |

Godot ucina końcówkę `EventHandler` — delegat `DiedEventHandler` tworzy sygnał o nazwie `Died`.

---

## 2. Formatowanie

Wszystko, co się da, jest już zapisane w `.editorconfig`. Najważniejsze punkty:

- 4 spacje, bez tabulatorów w plikach `.cs`.
- Końce linii `LF`, każdy plik kończy się pustą linią.
- **Nawiasy klamrowe w nowej linii (Allman)** — także dla `else`, `catch`, `finally`.
- Zawsze nawiasy klamrowe w `if`/`for`/`while`, nawet przy jednej instrukcji.
- Namespace zapisujemy **w formie `file-scoped`** (średnik zamiast klamerek).
- Dyrektywy `using` na górze pliku, `System.*` pierwsze.
- Linia nie dłuższa niż ~120 znaków.
- Jedna instrukcja w linii; puste linie tylko do rozdzielenia logicznych bloków.

Kolejność składowych w klasie: pola → konstruktor → właściwości → metody publiczne → metody Godota (`_Ready`, `_Process`, `_PhysicsProcess`, `_Input`) → metody prywatne.

---

## 3. Zasady pracy z Godotem

**Każda klasa dziedzicząca po `Node` musi być `partial`** — Godot generuje drugą część klasy:

```csharp
public partial class PlayerController : CharacterBody2D
```

### Sceny edytujemy w edytorze Godota

Plików `.tscn`, `.tres` i `project.godot` **nie edytujemy ręcznie**. Takie zmiany są trudne w code review (długie, formatowane przez silnik pliki) i łatwo coś przypadkiem zepsuć. Scenę zmieniamy w edytorze Godota, a w PR-ze opisujemy, co zostało zmienione.

### Dostęp do węzłów

- Zamiast długich ścieżek (`GetNode("../../HUD/Score")`) używamy **unikalnych nazw**: w edytorze kliknij węzeł prawym → *Access as Unique Name* (albo <kbd>Ctrl</kbd>+<kbd>U</kbd>). Wtedy dostęp to `GetNode<Label>("%ScoreLabel")`.
- Wynik `GetNode` zapisujemy w polu **raz, w `_Ready`**, a nie w każdej klatce:

```csharp
private Label _scoreLabel;

public override void _Ready()
{
    _scoreLabel = GetNode<Label>("%ScoreLabel");
}
```

- Wartości konfiguracyjne (prędkość, obrażenia, czas) wystawiamy przez `[Export]` i ustawiamy w inspektorze, zamiast wpisywać je w kodzie.

### Sygnały

Sygnały definiujemy i podłączamy w kodzie — nie przez „prawym w edytorze”:

```csharp
[Signal]
public delegate void ScoreChangedEventHandler(int newScore);

// emisja
EmitSignal(SignalName.ScoreChanged, _score);

// podłączenie (w _Ready)
_startButton.Pressed += OnStartPressed;
```

### Pętle i wydajność

- Fizykę i ruch (`MoveAndSlide`, `MoveAndCollide`, kolizje) liczymy w `_PhysicsProcess`.
- `_Process` służy do rzeczy zależnych od klatek: animacje, UI, efekty.
- W `_Process`/`_PhysicsProcess` nie wołamy `GetNode`, nie tworzymy nowych obiektów i nie robimy zapytań do sceny — wszystko, co stałe, ląduje w polach ustawionych w `_Ready`.

### Projektowanie

- **Kompozycja zamiast dziedziczenia.** Zamiast `Enemy : Character : Entity : Node` budujemy małe, samodzielne komponenty i węzły.
- Logikę dzielimy na sceny potomne (`Player.tscn` + `Health.tscn` jako dziecko), a nie na tysiące linijek w jednej klasie.
- Autoloady (w `Source/Managers/`) dodajemy tylko wtedy, gdy coś naprawdę musi być globalne. Zanim dodasz kolejny `*Manager`, zapytaj w PR, czy na pewno.

---

## 4. Komentarze

Komentarz wyjaśnia **dlaczego**, nie **co**:

```csharp
// Źle: zwiększamy licznik o jeden
_score++;

// Dobrze: nie zwiększamy wyniku w trybie treningowym
if (!_isTrainingMode)
{
    _score++;
}
```

- Publiczne klasy i metody w `Source/Components/` dokumentujemy komentarzem XML (`/// <summary>`), bo to kod współdzielony przez wszystkich.
- Kod tymczasowy oznaczamy `// TODO: opis i issue`, np. `// TODO: dodać animację trafienia (#128)`.
- Nie zostawiamy zakomentowanego kodu — od tego jest Git.
- Nie komentujemy oczywistości.

### Znacznik kodu napisany przez AI

Kod, którego **nie napisałeś sam** (wygenerowany przez Copilota, ChatGPT itp.), musi być oznaczony na jednej z dwóch form:

Pojedynczy fragment (metoda, blok, kilka linii):

```csharp
// AI-GENERATED: GitHub Copilot — metoda MoveTowards
private void MoveTowards(Vector2 target)
{
    GlobalPosition = GlobalPosition.MoveToward(target, _moveSpeed * (float)delta);
}
```

Większy blok — znaczniki początku i końca:

```csharp
// === AI-GENERATED START (GitHub Copilot) ===
private void SpawnWave(int waveNumber) { /* ... */ }
private void CleanupEnemies() { /* ... */ }
// === AI-GENERATED END ===
```

Zasady:

- W komentarzu podajemy **narzędzie**, którego użyto (dla recenzentów i dla nauki).
- Znacznik musi zostać w kodzie również po poprawkach — chyba że przepisałeś kod samodzielnie i rozumiesz każdą linię (wtedy dopisz w opisie PR-a, że AI pomogło wcześniej).
- Wszystkie znaczniki znajdujemy komendą: `rg "AI-GENERATED"`.
- Użycie AI do kodu odnotowujemy też w opisie PR-a ([CONTRIBUTING.md](CONTRIBUTING.md#zasady-dotyczące-ai)).
- **Generowanie assetów przez AI jest zabronione** (grafika, dźwięk, modele, fonty) — szczegóły w [CONTRIBUTING.md](CONTRIBUTING.md#assety-i-licencje).

---

## 5. Struktura plików i folderów

```text
LogJamGame/
├── Assets/                     # Zasoby: grafika, audio, modele 3D, fonty (Git LFS)
│   ├── CREDITS.md              # Rejestr zewnętrznych assetów: autor, źródło, licencja
│   ├── Textures/
│   ├── Models/
│   ├── Audio/
│   └── Fonts/
├── Source/                     # Cały kod źródłowy i sceny
│   ├── Components/             # Reużywalne komponenty (HealthBar, Hitbox)
│   ├── Managers/               # Autoloady / singletony (GameManager, AudioManager)
│   ├── Shaders/                # Shadery
│   └── UI/                     # Menu główne, HUD, elementy UI
├── docs/                       # Dokumentacja projektu (SETUP, CONTRIBUTING, CODE_STYLE, RELEASING)
├── project.godot
├── LogJamGame.sln
└── LogJamGame.csproj
```

Gdzie co trafia:

- **Do `Source/Components/`** rzeczy reużywalne, używane w więcej niż jednym miejscu (pasek zdrowia, hitbox).
- **Do `Source/Managers/`** tylko to, co naprawdę musi być globalne (autoloady).
- **Do `Source/UI/`** menu, HUD i elementy interfejsu, **do `Source/Shaders/`** shadery.
- **Nowe podfoldery w `Source/`** tworzymy tematycznie, PascalCase, dopiero gdy mamy co do nich wrzucić — np. `Source/Player/`, `Source/Enemies/`, `Source/Levels/`. Scena i jej skrypt leżą razem, obok siebie.
- To **jedna gra**, nie ma podziału na moduły ani wspólną „platformę” na kilka gier — nie tworzymy folderów na zapas.
- **Do `Assets/`** pliki zasobów. Nie trzymamy grafik ani dźwięków w folderach z kodem.
- **Namespace** w C# odzwierciedla ścieżkę folderu poniżej `Source/`, np. plik `Source/Components/HealthBar.cs` → `namespace LogJamGame.Components;`.

Nie przenosimy istniejących plików ani nie zmieniamy nazw folderów „przy okazji” — to zadanie na osobny PR, po uzgodnieniu.

---

## 6. Przykładowy plik

`Source/Components/HealthBar.cs`

```csharp
using Godot;

namespace LogJamGame.Components;

/// <summary>
/// Prosty pasek zdrowia do podłączenia jako dziecko dowolnej sceny.
/// </summary>
public partial class HealthBar : ProgressBar
{
    [Export] private int _maxHealth = 100;

    private int _currentHealth;

    public bool IsDead => _currentHealth <= 0;

    public override void _Ready()
    {
        _currentHealth = _maxHealth;
        Refresh();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        Refresh();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        Refresh();
    }

    private void Refresh()
    {
        MaxValue = _maxHealth;
        Value = _currentHealth;
    }
}
```

Na co zwrócić uwagę w tym przykładzie:

- PascalCase w nazwie pliku i klasy, `_camelCase` dla pól prywatnych,
- `partial` przy klasie dziedziczącej po `Node`,
- `[Export]` zamiast wartości wpisanej na sztywno,
- dokumentacja XML dla klasy współdzielonej,
- brak wołania `GetNode` w pętli, brak „magicznych liczb” w kodzie.
