# Prototyp combatu — instrukcja scen

Kod jest gotowy, ale **sceny ustawiasz w edytorze Godota** — zgodnie z zasadą z
[docs/CODE_STYLE.md](../../docs/CODE_STYLE.md), plików `.tscn` nie edytujemy ręcznie.
Poniżej dokładne kroki: nazwa węzła, typ, rodzic, właściwość, wartość.

Po zmianach w scenach: `dotnet build`, potem F5 w Godocie.

## Co robi który plik

| Plik | Rola |
|---|---|
| `TestWorld.cs` | Arena (ściany budowane w kodzie), tło wody, respawn gracza, spawn przeciwników, licznik zabójstw |
| `TestPlayer.cs` | Ruch „pod wodą", opóźnione celowanie myszką, strzał, melee, parowanie, dash na ładunkach, przyjmowanie obrażeń |
| `TestEnemy.cs` | Dwa typy przeciwnika: `Melee` (dopływa, zamachuje się na pomarańczowo i bije) i `Shooter` (trzyma dystans i strzela); po sparowaniu dostaje stun (niebieski odcień, nie rusza się i nie atakuje) |
| `Projectile.cs` | Pocisk zwalniający w wodzie; dziedziczy prędkość strzelca, można go sparować; tworzony z kodu, **nie wymaga sceny** |
| `CollisionLayers.cs` | Warstwy kolizji prototypu (świat / gracz / przeciwnicy) — dzięki nim gracz i przeciwnicy się nie zderzają |
| `Health.cs` | Komponent zdrowia ze sygnałami `Changed` / `Died` (dziecko gracza i przeciwnika) |
| `HealthBar.cs` | Pasek HP rysowany nad obiektem |
| `EnemyKind.cs` | Enum `EnemyKind.Melee` / `EnemyKind.Shooter` |
| `DebugPanel.cs` | Panel debug (CanvasLayer) — tworzony z kodu przez `TestWorld`, scena go nie potrzebuje |

## Sterowanie

| Wejście | Akcja |
|---|---|
| `W` `A` `S` `D` | ruch (z bezwładnością i oporem wody — zakręty są ospałe, ostre skręty robi się dashem) |
| mysz | celowanie (kierunek obraca się z ograniczoną prędkością, więc szybkie ruchy myszką go „gubią") |
| LPM (przytrzymaj) | strzał, co `_shootCooldown` |
| PPM (pojedyncze wciśnięcie) | atak melee (łuk przed graczem, odrzut przeciwnika) — przytrzymanie nic nie powtarza |
| `Shift` (pojedyncze wciśnięcie) | parowanie — okno 0.32 s (pierścień wokół gracza). Udane: pocisk odbija się i leci w przeciwników, a strzelec dostaje stun; cios melee nie zabiera HP, odrzuca i ogłusza. W oknie i chwilę po nim nie można strzelać ani bić melee, a pudło kończy się dłuższą karą (cooldown) |
| `Spacja` (pojedyncze wciśnięcie) | dash w kierunku ruchu (albo celowania, gdy stoisz) — zużywa jeden z ładunków |

Klawisze są czytane bezpośrednio w kodzie, więc **nie trzeba nic dodawać do Input Map**.
Jeśli kiedyś zechcesz przemapować klawisze — dopiero wtedy dodaj akcje w
*Project → Project Settings → Input Map*.

## Panel debug (lewy górny róg ekranu)

| Klawisz / przycisk | Efekt |
|---|---|
| `F1` | przeciwnik `Melee` pod kursorem |
| `F2` | przeciwnik `Shooter` pod kursorem |
| `F3` | fala 5 przeciwników wokół kursora |
| `F4` | zabij wszystkich |
| `F5` | god mode (nietykalność gracza) |
| `F6` | slow motion (`Engine.TimeScale = 0.35`) |
| `F7` | reset gracza na pozycję startową i pełne HP |

Panel pokazuje też liczbę zabójstw, liczbę przeciwników i FPS.

## Warstwy kolizji (dlaczego przeciwnicy nie „przyklejają się" do gracza)

`CollisionLayers.cs` ustawia trzy warstwy: `World = 1`, `Player = 2`, `Enemy = 4`.
`TestPlayer._Ready` i `TestEnemy._Ready` nadpisują wartości ze sceny:

- gracz: `CollisionLayer = Player`, `CollisionMask = World` — koliduje tylko ze ścianami,
- przeciwnik: `CollisionLayer = Enemy`, `CollisionMask = World` — koliduje tylko ze ścianami,
  więc **nie zderza się ani z graczem, ani z innymi przeciwnikami** (dlatego melee może
  przez nich przepłynąć i nie blokuje kolegów),
- pocisk: `CollisionMask = World | Player | Enemy` — trafia we wszystko, co trzeba.

Ważne: ściany areny muszą zostać na domyślnej warstwie `1` (`TestWorld` tworzy je z kodu i nic
w nich nie zmieniamy). Jeśli kiedyś dodasz własne kolizje w scenach, pamiętaj o tych warstwach —
kod nadpisuje to, co ustawisz w inspektorze.

## 1. `TestPlayer.tscn`

Docelowe drzewo:

```text
TestPlayer (CharacterBody2D)          ← skrypt: TestPlayer.cs (bez zmian)
├── Sprite2D                          ← bez zmian
├── CollisionShape2D                  ← bez zmian (opcjonalnie mniejszy)
├── Health          (Node)            ← NOWY
└── HealthBar       (Node2D)          ← NOWY, musi być ostatnim dzieckiem
```

Kroki:

1. Prawy klik na `TestPlayer` → **Add Child Node** → `Node` → nazwij `Health` →
   w inspektorze **Attach Script** → `res://Source/Tmp/Health.cs`.
2. Zaznacz `Health` → w inspektorze `Max Health` = `100`.
3. Prawy klik na `TestPlayer` → **Add Child Node** → `Node2D` → nazwij `HealthBar` →
   **Attach Script** → `res://Source/Tmp/HealthBar.cs`.
4. `HealthBar` musi być **na końcu listy dzieci** `TestPlayer` (poniżej `Health`), żeby
   rysował się nad sprite'em. `Offset Y` (domyślnie `-56`) podnieś/opuść według uznania.
5. (opcjonalnie) `CollisionShape2D` → `Shape` → `Size` = `(96, 96)` — obecne `124×127`
   to rozmiar ikony, więc gracz jest dość „gruby" w kolizjach.
6. Usuń stary węzeł `Movement` (prawy klik → **Delete**) — logika ruchu jest teraz
   w `TestPlayer.cs`. Po tym usuń plik `Source/Tmp/PlayerMovement.cs` (i `.uid`)
   w panelu *FileSystem* — inaczej zostanie martwy skrypt w repo.

## 2. `TestEnemy.tscn`

Docelowe drzewo:

```text
TestEnemy (CharacterBody2D)           ← skrypt: TestEnemy.cs (bez zmian)
├── Sprite2D                          ← bez zmian
├── CollisionShape2D                  ← bez zmian (opcjonalnie mniejszy)
├── Health          (Node)            ← NOWY
└── HealthBar       (Node2D)          ← NOWY, ostatnie dziecko
```

Kroki:

1. Dodaj `Node` o nazwie `Health`, podłącz `res://Source/Tmp/Health.cs`,
   `Max Health` = `40`.
2. Dodaj `Node2D` o nazwie `HealthBar`, podłącz `res://Source/Tmp/HealthBar.cs`,
   ustaw jako ostatnie dziecko, `Offset Y` = `-48`.
3. `Sprite2D` → `Modulate` jest teraz ustawiany **z kodu** zależnie od typu
   (`Melee` — czerwony, `Shooter` — fioletowy), więc wartość w scenie i tak zostanie nadpisana.

Typ przeciwnika ustawia spawner (`EnemyKind` w kodzie). Domyślny typ w scenie to `Melee`
— ten sam `TestEnemy.tscn` obsługuje oba zachowania, różnica jest w inspektorze `Kind`.

## 3. `TestWorld.tscn`

**Wymagane zmiany: brak.** Ściany areny, tło i panel debug powstają w kodzie.

Opcjonalnie, dla lepszego „feelu":

1. `TestPlayer` → `Camera2D` → `Position Smoothing` → **Enabled**, `Speed` = `8`
   (kamera płynnie dogania gracza, co pasuje do wodnej bezwładności).
2. `TestPlayer` → `Camera2D` → `Zoom` = `(1.2, 1.2)`, jeśli chcesz widzieć mniej areny.
3. Zaznacz `TestWorld` i w inspektorze ustaw:
   - `Arena Size` (domyślnie `1600 × 900`, arena jest wyśrodkowana w punkcie `(0, 0)`),
   - `Max Enemies` (limit przeciwników),
   - `Player Respawn Delay`,
   - `Enemy Scene` — przeciągnij `res://Source/Tmp/TestEnemy.tscn`. Jeśli zostawisz puste,
     kod sam wczyta tę scenę po ścieżce.
4. Upewnij się, że instancja `TestPlayer` stoi wewnątrz areny (przy `Arena Size` 1600×900
   pozycja `(194, 183)` jest OK).

## Co sprawdzić ręcznie po zbudowaniu

1. `dotnet build` — bez błędów i nowych ostrzeżeń.
2. F5 → gracz pływa z oporem: rozpędza się wolniej i skręca ospale, po puszczeniu klawiszy
   jeszcze chwilę się ślizga; ostre skręty wygodniej robić dashem.
3. Celowanie ma opóźnienie — szybki ruch myszką wokół gracza nie przestawia linii celowania od razu.
4. Dash: wokół gracza widać dwa ładunki (kropki). Po dashu jeden gaśnie i ładuje się z powrotem;
   przy zerze `Spacja` nic nie robi. Trzymanie `Spacja` nie dashuje w kółko.
5. `F1` / `F2` stawiają przeciwników pod kursorem, nad nimi widać paski HP. Przeciwnik melee
   może wpaść na gracza i go nie popycha.
6. Strzał (LPM) i melee (PPM) zabierają HP; po śmierci przeciwnika licznik „Zabici" rośnie.
   Melee nie powtarza się przy trzymaniu PPM, a przerwa między ciosami jest dłuższa.
7. Strzał w stronę ruchu dolatuje dalej, strzał do tyłu nie jest spowolniony.
8. `Shift`: wokół gracza zapala się pulsujący turkusowy pierścień (widoczny poza ikoną), a po
   udanym parowaniu rozchodzi się drugi krąg. Pocisk odbija się (zmienia kolor), leci w przeciwników,
   a strzelec dostaje stun — świeci na niebiesko i stoi.
9. Przeciwnik melee przed ciosem robi się pomarańczowy (`Contact Windup` ≈ 0.35 s) — w tym oknie
   wciśnij `Shift`: cios nie zabiera HP, a przeciwnik leci w tył i jest ogłuszony.
10. Parowanie kosztuje: przez całe okno i `Parry Recovery` po nim LPM i PPM nic nie robią
    (widać ciemniejszy, cienki pierścień), a po zakończeniu okna wokół gracza rysuje się rosnący
    łuk ładowania — `Shift` zadziała dopiero, gdy łuk się zamknie. Pudło ma dłuższe ładowanie
    (`Parry Fail Cooldown`) niż udane parowanie.
11. `F5` (god mode) — gracz nie dostaje obrażeń; po jego wyłączeniu HP spada.
12. Śmierć gracza → respawn po `Player Respawn Delay` w miejscu startowym.

## Strojenie (wszystko w inspektorze, bez zmian w kodzie)

- `TestPlayer`: `Acceleration`, `Max Speed`, `Drag` (opór wody), `Aim Turn Speed Degrees`
  (jak szybko celownik dogania mysz), `Dash Speed`, `Dash Duration`, `Max Dash Charges`,
  `Dash Recharge Time`, `Shoot Cooldown`, `Melee Range`, `Melee Arc Degrees`, `Melee Cooldown`,
  `Melee Damage`, `Melee Knockback`, `Parry Duration`, `Parry Recovery` (blokada akcji po oknie),
  `Parry Cooldown` (po udanym parowaniu), `Parry Fail Cooldown` (kara za pudło),
  `Parry Stun Duration`, `Parry Burst Duration`, `Parry Melee Knockback`.
- `TestEnemy`: `Acceleration`, `Max Speed`, `Drag`, `Stun Drag` (jak szybko stunowany przeciwnik
  traci prędkość), `Contact Range`, `Contact Windup` (jak długo przeciwnik zamachuje się przed
  ciosem — czas na reakcję), `Contact Damage`,
  `Preferred Distance` (dystans trzymany przez strzelca), `Shoot Range`, `Shoot Cooldown`.
- `Projectile` — wartości domyślne są w kodzie (`Speed`, `Drag`, `Lifetime`, `Max Inherited Speed`
  — ile prędkości strzelca pocisk może przejąć, `Reflect Speed Multiplier` — jak szybki jest
  po sparowaniu). Pocisk powstaje z kodu, więc nie ma go w inspektorze; jeśli pociski mają być
  inne dla gracza i przeciwnika, wystarczy dodać do `Launch` kolejny parametr.

## Czego jeszcze nie ma

- prawdziwych sprite'ów i animacji (assety robią ludzie — nie generujemy ich),
- sprawdzania linii strzału u strzelca (strzela przez ściany),
- pathfindingu i fal przeciwników,
- punktów, ekwipunku, drugiej broni,
- Input Map (klawisze są na sztywno w kodzie).
