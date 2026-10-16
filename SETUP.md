# Uruchomienie projektu na własnym komputerze

Krok po kroku: od pustego systemu do działającej gry. Jeśli coś nie działa, przejdź do sekcji [Rozwiązywanie problemów](#6-rozwiązywanie-problemów).

Nie musisz umieć programować, żeby uruchomić projekt — wystarczy przejść poniższe kroki. Jeśli utkniesz, zapytaj na Discordzie koła.

---

## 1. Co musisz zainstalować

| Narzędzie | Wersja | Skąd |
|---|---|---|
| **Godot Engine (.NET)** | 4.7.2 | [godotengine.org/download](https://godotengine.org/download/) |
| **.NET SDK** | 8.0 lub nowszy | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| **Git** | dowolna aktualna | [git-scm.com/downloads](https://git-scm.com/downloads) |
| **Git LFS** | najnowsza | [git-lfs.com](https://git-lfs.com/) |
| VS Code (opcjonalnie) | — | [code.visualstudio.com](https://code.visualstudio.com/download) |

> ⚠️ **Najważniejsze:** na stronie Godota wybierz wariant **„.NET”** (dawniej „Mono”), a nie standardowy. Standardowa wersja nie obsługuje C# i projekt się w niej nie uruchomi.

Przydatne skróty instalacji (Windows):

```powershell
winget install Microsoft.DotNet.SDK.8
winget install GitHub.GitLFS
```

Na macOS możesz użyć Homebrew, a na Linuksie package managera swojej dystrybucji — ale pamiętaj, żeby Godot był w wersji .NET.

### Sprawdź, czy wszystko jest na miejscu

```bash
git --version
git lfs version
dotnet --list-sdks
```

`dotnet --list-sdks` musi pokazać co najmniej `8.0.x`.

---

## 2. Pobranie repozytorium

Git LFS instalujemy **raz na maszynę**, przed klonowaniem:

```bash
git lfs install
git clone https://github.com/Kolo-Naukowe-Informatykow/LogJamGame.git
cd LogJamGame
```

Jeśli sklonowałeś repozytorium przed instalacją Git LFS i widzisz pliki assetów jako „pointery” (krótkie pliki tekstowe zamiast obrazków), pobierz je komendą:

```bash
git lfs pull
```

---

## 3. Pierwsze uruchomienie w Godocie

1. Uruchom **Godot Engine (.NET)** (nie otwieraj konkretnego projektu).
2. Kliknij **Import**, wskaż folder repozytorium (ten z plikiem `project.godot`) i wybierz **Import & Edit**.
3. Poczekaj, aż Godot zaimportuje zasoby — przy pierwszym otwarciu to normalnie kilkanaście sekund.
4. Zbuduj kod C#:
   - w edytorze Godota: ikona **młotka** / <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>B</kbd>, lub
   - w terminalu, będąc w folderze projektu: `dotnet build`
5. Uruchom grę: <kbd>F5</kbd> (albo przycisk ▶).

> Na tym etapie projekt nie ma jeszcze sceny głównej, więc Godot zapyta, którą scenę uruchomić. To normalne — gra jest dopiero tworzona.

### Gdzie wpisać edytor zewnętrzny

Żeby Godot otwierał skrypty w VS Code: **Editor → Editor Settings → Dotnet → Editor → External Editor → Visual Studio Code**.
Na Windows dodatkowo przydatne jest wskazanie pliku `code.cmd`.

---

## 4. VS Code (opcjonalnie)

Nie narzucamy edytora — równie dobrze działa Rider, Visual Studio czy sam edytor Godota. Jeśli używasz VS Code, skonfiguruj go **u siebie**: katalog `.vscode/` jest w `.gitignore`, więc Twoje ustawienia nie trafią do repozytorium i nikt nie narzuci Ci swojego setupu.

1. Otwórz **folder repozytorium** w VS Code (nie pojedynczy plik `.sln`).
2. Zainstaluj rozszerzenia (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd>):
   - **C#** (`ms-dotnettools.csharp`) — IntelliSense i debugowanie C#,
   - **Godot Tools** (`godotengine.godot-tools`) — obsługa projektu Godota,
   - **EditorConfig** (`editorconfig.editorconfig`) — pilnuje formatowania z `.editorconfig`.
3. Utwórz u siebie `.vscode/tasks.json`, żeby budować projekt z VS Code (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>B</kbd>):

   ```json
   {
     "version": "2.0.0",
     "tasks": [
       {
         "label": "build",
         "command": "dotnet",
         "type": "process",
         "args": ["build"],
         "problemMatcher": "$msCompile"
       }
     ]
   }
   ```

4. Utwórz u siebie `.vscode/launch.json`, żeby uruchamiać i debugować grę (<kbd>F5</kbd>):

   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": "Uruchom grę (Godot)",
         "type": "coreclr",
         "request": "launch",
         "preLaunchTask": "build",
         "program": "${env:GODOT4}",
         "args": [],
         "cwd": "${workspaceFolder}",
         "stopAtEntry": false
       },
       {
         "name": "Uruchom grę bez budowania (Godot)",
         "type": "coreclr",
         "request": "launch",
         "program": "${env:GODOT4}",
         "args": ["--path", "${workspaceFolder}"],
         "cwd": "${workspaceFolder}"
       }
     ]
   }
   ```

   Konfiguracja używa zmiennej środowiskowej `GODOT4`, która musi wskazywać na plik wykonywalny **Godota w wersji .NET**. Ustawienie zmiennej (Windows, tylko dla Twojego konta):

   ```powershell
   setx GODOT4 "C:\Ścieżka\Do\Godot_v4.7.2-stable_mono_win64.exe"
   ```

   Po zmianie zmiennej zrestartuj VS Code. Alternatywnie wpisz ścieżkę wprost w polu `program` w `launch.json` (zamiast `${env:GODOT4}`).

Breakpointy działają normalnie — klikasz w lewy margines w pliku `.cs`.

---

## 5. Praca bez edytora Godota (szybkie komendy)

```bash
dotnet build                 # kompilacja kodu C#
dotnet build -warnaserror    # to samo, ale ostrzeżenia traktowane jak błędy (przed PR-em)
godot --path .               # uruchomienie gry, jeśli masz godot w PATH
```

Minimalna wersja procesu przed oddaniem PR-a:

1. `git switch -c feature/moje-zadanie`
2. praca → `dotnet build` (musi przejść bez błędów),
3. uruchom scenę w Godocie i sprawdź, że nic się nie zepsuło,
4. commit + push + pull request ([CONTRIBUTING.md](CONTRIBUTING.md)).

---

## 6. Rozwiązywanie problemów

| Problem | Co zrobić |
|---|---|
| **`Godot.NET.Sdk/4.7.2` not found** / błąd przy `dotnet build` | Brak internetu przy pierwszym buildzie albo problem z NuGet. Uruchom `dotnet restore` i spróbuj ponownie. |
| **Brak plików `.cs` w projekcie / „C# script not found”** | Otworzyłeś złą wersję Godota. Pobierz wariant **.NET** i powtórz krok 3. |
| **Zmiany w kodzie nie widać w grze** | Nie zbudowałeś projektu. Zbuduj go (`dotnet build` lub młotek w Godocie) i uruchom ponownie. |
| **Assety wyglądają jak uszkodzone / pliki mają po kilka linijek „pointer”** | Nie masz pobranych plików LFS: `git lfs install` i `git lfs pull`. |
| **Godot ostrzega, że projekt pochodzi z innej wersji** | Projekt jest robiony w Godot 4.7.2. Zaktualizuj silnik; nie „ratuj” projektu downgrade'em. |
| **`Unable to find package Godot.NET.Sdk`** | Sprawdź plik `nuget.config` / ustawienia proxy i połączenie z `api.nuget.org`. |
| **Windows: Git pokazuje zmienione pliki bez powodu (końcówki linii)** | Ustaw `git config --global core.autocrlf input` (nie ignorujemy `.gitattributes`). |
| **Coś dziwnie się sypie, a wcześniej działało** | Zamknij Godota, usuń katalog `.godot/` (lokalny cache — jest w `.gitignore`) i otwórz projekt ponownie. |
| **VS Code: „The .NET Core SDK cannot be located”** | Zainstaluj .NET SDK 8+ i zrestartuj VS Code. |

Jeśli problemu nie ma na liście — załóż **issue** na GitHubie (opis w [CONTRIBUTING.md](CONTRIBUTING.md)) albo zapytaj na Discordzie. Świeży kontrybutor z nowym problemem to najlepsze źródło informacji, czego brakuje w tym poradniku.

---

## 7. Czego nie wrzucamy do repozytorium

`.godot/`, `bin/`, `obj/`, wyeksportowane buildy oraz osobiste ustawienia edytorów (`.vscode/`, `.vs/`, `.idea/`) — wszystko to już jest w `.gitignore`. Duże assety (grafika, audio, modele, fonty) wchodzą automatycznie przez **Git LFS** zgodnie z `.gitattributes`; nowy typ pliku dodajemy komendą `git lfs track "*.ext"` i commitem `.gitattributes` w tym samym PR-ze.
