# LogJamGame

> **Nazwa i koncept są tymczasowe.** Gra nie ma jeszcze oficjalnej nazwy ani ustalonego pomysłu — zdecydujemy o tym wspólnie na spotkaniu koła. Na razie w repozytorium jest techniczne rusztowanie projektu.

Otwarty, zespołowy projekt gry tworzony w ramach działalności **Koła Naukowego Informatyków**. Rozwijamy go przez co najmniej semestr, wspólnie, w formie repozytorium z listą zadań do podjęcia.

## Dlaczego to robimy

- **Nauka przez praktykę** — praca w prawdziwym repozytorium: zadania, gałęzie, pull requesty, code review, testy, zgłaszanie błędów.
- **Wejście w gamedev** dla osób, które nie miały wcześniej styczności z silnikiem ani z C# — zadania dzielimy na małe kroki i nikt nie zostaje sam.
- **Współpraca i wymiana wiedzy** — każdy może wziąć zadanie, zrobić review cudzego kodu albo pokazać coś na spotkaniu.
- **Portfolio** — namacalny projekt, w którym każdy zostawia swój ślad (commity, PR-y, review).
- **Tematy na spotkania koła** — wprowadzenie do Godota, C#, GitHuba, code review.

## Stack technologiczny

| Narzędzie | Wersja | Uwagi |
|---|---|---|
| [Godot Engine](https://godotengine.org/download/) | **4.7.2** | koniecznie wariant **.NET** (nie standardowy) |
| [.NET SDK](https://dotnet.microsoft.com/download) | **8.0+** | projekt celuje w `net8.0` |
| [Git](https://git-scm.com/downloads) + [Git LFS](https://git-lfs.com/) | najnowsze | LFS obsługuje duże assety (grafika, audio, modele) |
| VS Code / Rider | dowolny | opcjonalnie, przydatne do C# |

## Jak pracujemy

| Zasada | W skrócie |
|---|---|
| **Zadania** | Tablica w [GitHub Projects](https://github.com/orgs/Kolo-Naukowe-Informatykow/projects). Wybierasz zadanie, przypisujesz się, przenosisz kartę na *In progress*. |
| **Zmiany** | Zawsze przez Pull Request do `main`. Nikt nie pushuje bezpośrednio do `main`. |
| **Review** | Minimum **2 zatwierdzenia**, w tym **co najmniej jedno od osoby nadzorującej projekt**. |
| **Testy** | Po zatwierdzeniu zmiany ktoś realnie uruchamia grę/scenę i sprawdza działanie. Nie każdy task, ale regularnie. |
| **Błędy** | Zgłaszamy jako **issue** na GitHubie (z krokami do powtórzenia). Nie naprawiamy błędów „po cichu”. |
| **AI** | **Kod napisany przez AI oznaczamy komentarzem.** Zabronione jest generowanie assetów (grafiki, dźwięku, modeli, fontów). Projekt ma uczyć — AI nie może zrobić zadania za nas. |

Szczegóły: [CONTRIBUTING.md](CONTRIBUTING.md).

## Dokumentacja projektu

| Plik | Do czego służy |
|---|---|
| [SETUP.md](SETUP.md) | Instalacja narzędzi, klonowanie, uruchomienie gry u siebie, rozwiązywanie problemów |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Jak wziąć zadanie, jak zrobić pull request, review, testy, zgłaszanie błędów |
| [CODE_STYLE.md](CODE_STYLE.md) | Zasady pisania kodu: nazewnictwo, formatowanie, struktura plików, znacznik kodu z AI |
| [AGENTS.md](AGENTS.md) | Zasady dla asystentów AI (Copilot i inne) pracujących w tym repo |

## Szybki start

```bash
git lfs install
git clone https://github.com/Kolo-Naukowe-Informatykow/LogJamGame.git
cd LogJamGame
dotnet build
```

Następnie otwórz folder w **Godot Engine (.NET)** → *Import* → *Import & Edit*, a grę uruchom klawiszem <kbd>F5</kbd>.

> Projekt nie ma jeszcze sceny głównej, więc przy pierwszym <kbd>F5</kbd> Godot zapyta, którą scenę uruchomić. To normalne, dopóki nie powstanie pierwsza scena gry.

Pełna instrukcja (Windows/macOS/Linux, VS Code, debugowanie, typowe błędy): [SETUP.md](SETUP.md).

## Struktura repozytorium

```text
LogJamGame/
├── Assets/                     # Zasoby: grafika, audio, modele 3D, fonty (przez Git LFS)
│   ├── Textures/
│   ├── Models/
│   ├── Audio/
│   └── Fonts/
├── Source/                     # Cały kod źródłowy i sceny
│   ├── Components/             # Reużywalne komponenty (HealthBar, Hitbox, ...)
│   ├── Managers/               # Autoloady / singletony (GameManager, AudioManager, ...)
│   ├── Shaders/                # Shadery
│   └── UI/                     # Menu główne, HUD, elementy UI
├── project.godot               # Plik projektu Godota
├── LogJamGame.sln
├── LogJamGame.csproj
└── README.md
```

Nowe podfoldery w `Source/` tworzymy tematycznie i tylko wtedy, gdy mamy co do nich wrzucić (np. `Source/Player/`, `Source/Enemies/`) — to jedna gra, nie ma podziału na osobne moduły. Konfigurację swojego edytora trzymamy lokalnie: katalog `.vscode/` jest w `.gitignore`, więc nic z Twoich ustawień nie trafi do repozytorium.

Zasady nazewnictwa i tego, co gdzie trafia, opisuje [CODE_STYLE.md](CODE_STYLE.md).

## Licencja

Projekt nie ma jeszcze wybranej licencji (do ustalenia przez koło). Do momentu jej ustalenia kod jest udostępniany wyłącznie na potrzeby pracy w kole naukowym.

