# Wydania gry (releases)

Każdy push do `main` uruchamia workflow [`.github/workflows/release.yml`](../.github/workflows/release.yml), który:

1. podbija numer wersji,
2. eksportuje grę na **Windows**, **Linux** i **macOS**,
3. pakuje buildy w archiwa ZIP (razem z [`Assets/CREDITS.md`](../Assets/CREDITS.md)),
4. generuje changelog z commitów od poprzedniego wydania,
5. tworzy tag `vX.Y.Z` i publikuje **GitHub Release** z archiwami i sumami kontrolnymi SHA-256.

Wydanie jest więc zawsze zbudowane z dokładnie tego commita, który wszedł na `main`.

> **Stan projektu:** dopóki w `project.godot` nie ma sceny głównej (`application/run/main_scene`),
> buildy powstaną, ale uruchomiona gra nie będzie miała czego wyświetlić. Workflow wypisze
> wtedy ostrzeżenie, ale nie przerwie wydania.

## Numeracja wersji

Wersje mają postać `vMAJOR.MINOR.PATCH` (np. `v0.3.1`) i wynikają z tagów Git.

| Sytuacja | Efekt |
|---|---|
| Brak tagów w repozytorium | pierwsze wydanie to `v0.1.0` |
| Push do `main` | `PATCH` + 1 (`v0.1.0` → `v0.1.1`) |
| Ręczne uruchomienie z `bump: minor` | `MINOR` + 1, `PATCH` = 0 (`v0.1.1` → `v0.2.0`) |
| Ręczne uruchomienie z `bump: major` | `MAJOR` + 1, reszta = 0 (`v0.2.5` → `v1.0.0`) |

- **Minor** uruchamiasz ręcznie, gdy wydanie zawiera nową funkcję: **Actions → Release → Run workflow → bump: minor**.
- **Major** zarezerwuj na moment, w którym gra wychodzi z fazy 0.x.
- Numer wersji jest wpisywany do `project.godot` jako `application/config/version` **tylko w buildzie CI** — plik w repozytorium pozostaje bez zmian.
- Tag tworzy workflow. Nie pushuj tagów ręcznie, bo rozjedzie się to z historią wydań.

### Ponowne uruchomienie

Jeśli build padnie przed utworzeniem wydania, kolejny push po prostu podbije wersję jeszcze raz — żaden tag nie został wtedy zapisany.

Jeśli tag już istnieje, a wydanie jest niekompletne, ponowne uruchomienie workflow na tym samym commicie **wznawia** to samo wydanie (nie tworzy kolejnego numeru). Archiwa są wtedy nadpisywane.

## Co trafia do wydania

| Plik | Zawartość |
|---|---|
| `LogJamGame-X.Y.Z-windows-x86_64.zip` | `LogJamGame.exe` + pliki runtime .NET + `CREDITS.md` (+ `Licenses/`, jeśli istnieje) |
| `LogJamGame-X.Y.Z-linux-x86_64.zip` | `LogJamGame.x86_64` + pliki runtime .NET + `CREDITS.md` (+ `Licenses/`, jeśli istnieje) |
| `LogJamGame-X.Y.Z-macos-universal.zip` | `LogJamGame.app` (Universal 2) + `CREDITS.md` (+ `Licenses/`, jeśli istnieje) |
| `SHA256SUMS.txt` | sumy kontrolne archiwów |

Buildy są **niepodpisane**:

- **Windows** — SmartScreen może pokazać ostrzeżenie („Nieznany wydawca”).
- **macOS** — Gatekeeper może zablokować aplikację. Obejście: prawy przycisk na `.app` → **Open**, albo `xattr -dr com.apple.quarantine LogJamGame.app`. Docelowo można dodać podpisywanie certyfikatem Apple (wymaga konta Apple Developer) — to osobne zadanie.

## Changelog

Changelog powstaje w skrypcie [`.github/scripts/generate-changelog.sh`](../.github/scripts/generate-changelog.sh) na podstawie:

```bash
git log <poprzedni-tag>..HEAD --no-merges
```

Każdy commit na `main` (po squash merge'u jest to tytuł PR-a) trafia jako jedna pozycja z linkiem do commita. Ponieważ `main` przyjmuje wyłącznie squash merge, historia jest czytelna i changelog nie zawiera commitów technicznych z gałęzi.

Tytuły PR-ów piszemy więc tak, jak mają wyglądać w changelogu — konkretnie i po polsku ([CONTRIBUTING.md](CONTRIBUTING.md#commity)).

## Wymagania i konfiguracja (dla administratorów)

- **Uprawnienia workflow:** `release.yml` jawnie deklaruje `permissions: contents: write`, co wystarcza nawet przy domyślnym ustawieniu „Read repository contents”. Jeśli jednak reguła organizacji blokuje podnoszenie uprawnień przez workflow, włącz w *Settings → Actions → General → Workflow permissions* opcję „Read and write permissions”. Bez tego tworzenie tagu/wydania skończy się błędem 403. Dodatkowe sekrety nie są potrzebne.
- **Wersja Godota** jest w dwóch miejscach i musi być zgodna: `GODOT_VERSION` w workflow oraz `Godot.NET.Sdk` w `LogJamGame.csproj`. Po podniesieniu wersji silnika zaktualizuj oba.
- **Presety eksportu** są w [`export_presets.cfg`](../export_presets.cfg) — plik można commitować (nie zawiera sekretów). Zmiany najlepiej robić w edytorze Godota (*Project → Export*), a potem zacommitować.
- **Pierwsze uruchomienie** trwa dłużej (pobranie Godota i ~1,2 GB szablonów eksportu); kolejne są szybsze dzięki cache akcji.
- **Koszty:** każde wydanie to kilka minut Actions i miejsce na archiwa. Jeśli okaże się to zbyt dużo, można zmienić trigger workflow (np. tylko tagi) — wymaga to świadomej decyzji opiekunów.

## Ręczny eksport u siebie

Do lokalnego eksportu potrzebujesz **Godota 4.7.2 (.NET)** i zainstalowanych szablonów eksportu (*Editor → Manage Export Templates → Install Selected Templates*, zaznacz Windows, Linux i macOS).

Dla macOS (Universal 2) musisz też włączyć import tekstur ETC2/ASTC — inaczej Godot odmówi eksportu:

1. **Project → Project Settings → Rendering → Textures → VRAM Compression**.
2. Włącz **Import ETC2 ASTC**.
3. Zapisz ustawienia i poczekaj na ponowny import tekstur.

Workflow robi to samo automatycznie w swoim checkoutcie, ale warto mieć tę opcję włączoną również w projekcie, żeby lokalne buildy były identyczne.

W katalogu projektu:

```bash
mkdir -p build/windows build/linux build/macos
godot --headless --path . --import
godot --headless --path . --export-release "Windows Desktop"
godot --headless --path . --export-release "Linux"
godot --headless --path . --export-release "macOS"
```

Na Windowsie w PowerShell katalogi utworzysz tak:

```powershell
New-Item -ItemType Directory -Force build/windows, build/linux, build/macos
```

Ścieżki wynikowe są zapisane w `export_presets.cfg` (`build/<platforma>/...`) i katalogi docelowe **muszą istnieć** — Godot nie tworzy ich sam przy eksporcie z linii komend. Katalog `build/` jest w `.gitignore`.

Na Windowsie eksport `.app` nie zachowuje bitu wykonywalności; dlatego preset macOS eksportuje od razu `.zip`, który ten problem omija. W CI używamy Linuksa, więc archiwa są kompletne.
