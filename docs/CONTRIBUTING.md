# Jak kontrybuować

Ten dokument opisuje, jak wygląda praca w projekcie: od wybrania zadania, przez pull request i code review, po testy i zgłaszanie błędów.

Repozytorium ma uczyć pracy w zespole, dlatego część zasad (review, issue, testy) jest sztywna. Nie chodzi o biurokrację — chodzi o to, żeby każdy wiedział, co się dzieje i kto co robi.

## Spis treści

- [Zanim zaczniesz](#zanim-zaczniesz)
- [Zadania i tablica](#zadania-i-tablica)
- [Zgłaszanie błędów (issue)](#zgłaszanie-błędów-issue)
- [Propozycje nowych rzeczy](#propozycje-nowych-rzeczy)
- [Proces zmiany krok po kroku](#proces-zmiany-krok-po-kroku)
- [Pull request — wymagania](#pull-request--wymagania)
- [Automatyczny build (CI)](#automatyczny-build-ci)
- [Code review](#code-review)
- [Testowanie po code review](#testowanie-po-code-review)
- [Zasady dotyczące AI](#zasady-dotyczące-ai)
- [Assety i licencje](#assety-i-licencje)
- [Definition of Done](#definition-of-done)
- [Zabezpieczenia repozytorium (dla administratorów)](#zabezpieczenia-repozytorium-dla-administratorów)

---

## Zanim zaczniesz

1. Uruchom projekt u siebie: [SETUP.md](SETUP.md).
2. Przeczytaj [CODE_STYLE.md](CODE_STYLE.md) — tam są zasady pisania kodu i nazewnictwa.
3. Dołącz do **Discorda koła**. Tam zadajemy pytania „jak to zrobić”, a issues służą do konkretów (błąd, funkcja, decyzja).
4. Sprawdź, czy masz uprawnienia do przypisywania się do zadań na tablicy. Jeśli nie — napisz na Discordzie.

**Nie musisz umieć programować.** Potrzebujemy też osób do testów, dokumentacji, assetów i porządkowania zadań. Zaczynasz od zadania oznaczonego `good first issue`.

## Zadania i tablica

Zadania żyją na **[tablicy GitHub Projects](https://github.com/orgs/Kolo-Naukowe-Informatykow/projects/6)** projektu. Każde zadanie to issue typu **Bug** albo **Feature** — nie ma osobnego typu „Task”. Typ Task jest wyłączany w ustawieniach organizacji; jeśli nadal widzisz go w formularzu, poproś właściciela organizacji o jego wyłączenie.

Nowe issue tworzymy wyłącznie przez szablony w [`.github/ISSUE_TEMPLATE/`](../.github/ISSUE_TEMPLATE/). Szablon ustawia właściwy typ (Bug/Feature) i podpowiada, co warto dopisać — wymagany jest tylko krótki opis. Puste issue jest wyłączone. Projekt ma włączony **auto-add**, więc nowe issue z repozytorium trafiają na tablicę automatycznie.

Główny widok tablicy to **Kanban**. Są też widoki: *Priority board*, *Team items*, *Roadmap* i *My items* (filtr `assignee:@me`).

Kolumny:

| Kolumna | Znaczenie |
|---|---|
| **Backlog** | Pomysły i zadania jeszcze niegotowe do podjęcia |
| **Ready** | Zadanie gotowe do wzięcia: ma opis, zakres i kryteria akceptacji |
| **In progress** | Ktoś nad nim pracuje (przypisany assignee) |
| **In review** | Otwarty pull request, trwa review |
| **Test** | Zmiana zatwierdzona, ktoś ją testuje na swoim komputerze |
| **Done** | Zmergowane i przetestowane |

Statusy zmieniamy ręcznie:

| Moment | Status |
|---|---|
| Zaczynasz pracę / tworzysz gałąź | **In progress** (przypisz się do issue) |
| Otwierasz PR | **In review** (możesz odpiąć się od issue) |
| PR zmergowany | **Test** (issue zostaje otwarte do testów) |
| Testy zakończone | **Done** (zamknij issue) |

Zasady:

- **Backlog → Ready**: gdy zadanie ma już opis i da się je wziąć, przenieś kartę na *Ready*.
- **Bierzesz zadanie**: przypisz się do issue i przenieś kartę na *In progress*. Nie pracujemy w dwóch nad tym samym.
- **Otwierasz PR**: przenieś kartę na *In review*. Możesz odpiąć się od issue, jeśli ktoś inny przejmuje review.
- **Merge**: przenieś kartę na *Test*, a issue zostaw otwarte do testów.
- **Po testach**: zamknij issue — wbudowany workflow ustawi *Done* (albo ustaw je ręcznie).
- **PR zamknięty bez merge**: wróć kartą na *In progress*.
- Zadanie jest niejasne → pytaj na Discordzie, doprecyzujemy opis (zamiast zgadywać).
- Zadanie jest za duże → podziel je na mniejsze i zgłoś to w komentarzu.

## Zgłaszanie błędów (issue)

Znalazłeś błąd? **Załóż issue z szablonu Bug.** Nie naprawiamy błędów „przy okazji” — także dlatego, że to zadanie dla kogoś, kto chce się nauczyć.

Zanim założysz issue, sprawdź, czy ktoś już go nie zgłosił (przeszukaj issues po słowach kluczowych).

Wystarczy krótki opis błędu. Jeśli możesz, dopisz też:

- co powinno się stać,
- kroki do powtórzenia (numerowana lista, najlepiej od uruchomienia gry),
- jak często występuje,
- środowisko (system, wersja Godota 4.7.2, numer commita lub gałęzi),
- materiały (zrzut ekranu, log z okna *Output*, treść błędu z konsoli).

Etykiety: `bug`, `enhancement`, `documentation`, `good first issue`, `help wanted`, `question`. Typ issue (Bug/Feature) to osobne pole — nie myl go z etykietą.

Jeśli chcesz sam naprawić zgłoszony błąd — napisz to w komentarzu pod issue i przypisz się. Unikniemy wtedy sytuacji, w której dwie osoby robią to samo.

## Propozycje nowych rzeczy

Pomysł na funkcję, ulepszenie albo usprawnienie repo? Załóż **issue z szablonu Feature** i poczekaj na akceptację (komentarz od osoby nadzorującej projekt lub decyzja na spotkaniu). Wystarczy krótki opis pomysłu — reszta pól jest opcjonalna.

To nie jest biurokracja dla samej siebie: dzięki temu nie powstają duże PR-y, których nie da się przyjąć, bo nie pasują do konceptu gry lub dublują czyjąś pracę.

## Proces zmiany krok po kroku

```bash
# 1. Zaktualizuj main
git switch main
git pull

# 2. Utwórz gałąź z issue na tablicy (przycisk „Create a branch”)
#    GitHub zaproponuje nazwę <numer>-<tytuł-z-myslnikami>; możesz ją zostawić:
git switch -c 6-temporary-test-scene

# 3. Pracuj i commituj małymi krokami
git add .
git commit -m "Dodaj ruch gracza w osi X"

# 4. Zbuduj i sprawdź, że działa
dotnet build

# 5. Wypchnij gałąź i otwórz pull request
git push -u origin 6-temporary-test-scene
```

Nazwa gałęzi:

| Format | Skąd | Przykład |
|---|---|---|
| `<numer>-<nazwa-z-myslnikami>` | numer issue + tytuł issue, spacje zamienione na myślniki | `6-temporary-test-scene` |

Zasady:

- **Zalecana jest domyślna nazwa GitHuba** — dokładnie taka, jaką proponuje przycisk **Create a branch** na karcie issue: `<numer>-<tytuł-z-myslnikami>`. Nie musisz jej zmieniać.
- **Gałąź najlepiej tworzyć z issue** — wtedy numer jest w nazwie, a GitHub linkuje gałąź do issue. Check `issue-branch` sprawdza, czy numer wskazuje na istniejące, otwarte issue typu Bug albo Feature. Jeśli nie, wyświetla **ostrzeżenie**, ale nie blokuje merge'a.
- **Nigdy nie pushujemy bezpośrednio do `main`.** Zawsze gałąź + pull request.
- Jeden PR = jedna rzecz. Drobne poprawki z review dopisujemy do tego samego PR-a, ale nie dokładamy do niego przypadkowych innych zadań.
- Jeśli `main` poszedł naprzód, zaktualizuj swoją gałąź (`git pull --rebase origin main`) i rozwiąż konflikty u siebie, przed merge.
- Nie zamykamy PR-a bez odpowiedzi na komentarze z review.

### Commity

- **Jeden commit = jedna logiczna zmiana.** Commitujemy małymi krokami, żeby z diffu dało się przeczytać, co się stało i dlaczego.
- **Jasny komunikat w trybie rozkazującym** („Dodaj ruch gracza w osi X”, nie „dodałem”, „zmiany”, „fix”, „poprawki”, „update”).
- **Pierwsza linia to sedno zmiany.** Jeśli potrzebujesz więcej miejsca, po pustej linii dopisz szczegóły (co i dlaczego).
- Do commita trafia tylko to, co chcesz wysłać: `git add` konkretnych plików zamiast `git add .` w ciemno. Przed commitem zerknij na `git status` i `git diff --staged`.
- **Nie commitujemy** plików z `.godot/`, `bin/`, `obj/`, wyeksportowanych buildów, sekretów i lokalnych ścieżek (.gitignore powinno to obejmować).
- Na `main` trafia jeden commit na PR (**Squash and merge**), dlatego tytuł PR-a piszemy tak, jak ma wyglądać komunikat tego commita.

## Pull request — wymagania

Każdy PR musi mieć:

- **Minimum 2 zatwierdzenia**, w tym **co najmniej jedno od osoby nadzorującej projekt** (bardziej doświadczonej). Autor nie zatwierdza własnego PR-a i nie merge'uje go bez wymaganych zatwierdzeń — nawet przy zmianie „jednej linijki”.
- **Powiązanie z issue** — w opisie `Closes #123` albo `Refs #123`, żeby zadanie zamknęło się po merge'u.
- **Nazwa gałęzi** — zalecany format `<numer>-<nazwa-z-myslnikami>` (domyślna nazwa GitHuba). Check `issue-branch` tylko ostrzega, jeśli nie da się powiązać gałęzi z issue.
- **Opis** wypełniony według poniższego szablonu.
- **Zielony build** — `dotnet build` bez błędów i bez nowych ostrzeżeń, potwierdzony przez CI (patrz [Automatyczny build (CI)](#automatyczny-build-ci)).
- **Brak plików, których nie powinno być**: `.godot/`, `bin/`, `obj/`, wyeksportowane buildy, lokalne ścieżki, sekrety.

### Szablon opisu PR

GitHub wstawia ten szablon automatycznie do każdego nowego PR-a ([`.github/PULL_REQUEST_TEMPLATE.md`](../.github/PULL_REQUEST_TEMPLATE.md)) — nie musisz go kopiować ręcznie.

## Checklist
- [ ] CI jest zielone (`Build / dotnet build`) i lokalnie `dotnet build` przechodzi bez błędów
- [ ] Nazwa gałęzi w formacie `<numer>-<nazwa-z-myslnikami>` (check `issue-branch` tylko ostrzega)
- [ ] Sprawdzone ręcznie w Godocie (opisałem powyżej, co sprawdziłem)
- [ ] Zgodne z CODE_STYLE.md
- [ ] Nie dodałem plików z `.godot/`, `bin/`, `obj/`
- [ ] Jeśli użyłem AI do kodu — oznaczyłem to w kodzie (`AI-GENERATED`) i w tym opisie
- [ ] Zaktualizowałem dokumentację, jeśli zmiana tego wymaga
- [ ] PR ma **2 zatwierdzenia**, w tym **co najmniej jedno od osoby nadzorującej**

### Merge

- Merge'ujemy **Squash and merge**, żeby historia `main` była czytelna: jedna zmiana = jeden commit.
- Tytuł PR-a staje się treścią commita, dlatego ma być konkretny („Dodaj pauzę w menu”, nie „poprawki”).
- Po merge'u: usuń gałąź, przenieś kartę na *Done*.

## Automatyczny build (CI)

Każdy push do `main` i każdy pull request uruchamia workflow [`.github/workflows/build.yml`](../.github/workflows/build.yml) na GitHub Actions: pobranie repo → .NET 8 → `dotnet build`. Wynik zobaczysz pod PR-em (check `Build / dotnet build`), przy commicie oraz w zakładce **Actions**.

- **Czerwony build = PR nie jest gotowy do merge'a**, nawet jeśli ma już zatwierdzenia. Napraw błąd u siebie, zrób commit i push — CI uruchomi się ponownie (poprzedni przebieg dla tej samej gałęzi jest anulowany).
- **Zielone CI nie zastępuje testu manualnego.** Workflow tylko kompiluje kod: nie uruchamia gry, nie ma jeszcze testów automatycznych ani sceny głównej.
- Jak czytać błąd: **Actions** → wybierz przebieg → job `dotnet build` → krok `Build`. Komunikat kompilatora jest tam dosłownie taki sam, jak w Twoim terminalu.
- Lokalnie sprawdzisz to samo komendą `dotnet build` (albo ostrzej, przed wysłaniem PR-a: `dotnet build -warnaserror`).

Każdy pull request uruchamia też workflow [`branch-check.yml`](../.github/workflows/branch-check.yml) (check `issue-branch`). Jest to check **ostrzegawczy**: sprawdza, czy nazwa ma format `<numer>-<nazwa-z-myslnikami>` i czy wskazane issue istnieje, jest otwarte i ma typ Bug/Feature. Jeśli coś nie pasuje, w PR pojawi się żółte ostrzeżenie, ale merge nie zostanie zablokowany.

Po każdym pushu do `main` uruchamia się dodatkowo workflow [`release.yml`](../.github/workflows/release.yml): podbija numer wersji (patch), eksportuje grę na Windows, Linux i macOS i publikuje wydanie na GitHubie z changelogiem z commitów. Jak to działa i jak zrobić wydanie minor/major — [RELEASING.md](RELEASING.md).

## Code review

Review to rozmowa o kodzie, nie o osobie. Obowiązują dwie role:

**Recenzent:**

- Czyta kod, uruchamia projekt, sprawdza, czy zmiana robi to, co opisuje PR.
- Komentuje **konkretnie**: co, gdzie i dlaczego. Zamiast „to jest źle” — „ta pętla wykona się 60 razy na sekundę; czy można policzyć to raz w `_Ready`?”.
- Wychwytuje: błędy logiczne, brak obsługi błędów, nazwy niezgodne z [CODE_STYLE.md](CODE_STYLE.md), niepotrzebne zmiany w innych plikach, wycieki/duplikację, brak testów manualnych.
- Drobiazgi formatowania oznacza `nit:` — to nie blokuje zatwierdzenia. Formaty pilnuje `.editorconfig`.
- Zatwierdza (`Approve`) dopiero wtedy, gdy sam by to zmergował.

**Autor:**

- Odpowiada na **każdy** komentarz: naprawione, wyjaśnione albo „wynoszę do osobnego issue”.
- Nie obraża się i nie przyjmuje uwag osobiście — recenzent pracuje dla wspólnego dobra.
- Jeśli nie zgadza się z uwagą, pisze dlaczego. Reviewer może ustąpić albo zgłosimy temat na spotkaniu.
- Nanosi poprawki commitami (nie przepisuje historii, gdy ktoś już zaczął review).

Jeśli PR wisi bez review — przypomnij się na Discordzie, wskaż osobę nadzorującą. Nikt nie ma obowiązku zgadnąć, że czekasz.

## Testowanie po code review

Kod, który przeszedł review, jest **testowany na cudzym komputerze**: budujemy gałąź PR-a i uruchamiamy grę.

- Nie każdy task jest testowany ręcznie — testujemy to, co widać i co da się zepsuć (mechanika, UI, wczytywanie scen, wydajność).
- Regularnie robimy zbiorczą **sesję testową** na spotkaniu: przechodzimy nowe funkcje po kolei i zapisujemy uwagi jako issues.
- Testujemy na tym, co mamy: Windows/macOS/Linux, jeśli to możliwe różne rozdzielczości i pełny ekran.
- Wynik testu zapisujemy w PR-ze, np.: „Testowane na Windows 11, Godot 4.7.2: ruch i kolizje działają, przy skoku na krawędzi platformy postać przenika o ~2 px — zgłoszone w #142”.

Znalazłeś błąd w cudzym PR-ze?

- Jeśli blokuje działanie zmiany → komentarz w PR-ze, autor poprawia.
- Jeśli to osobny problem → **issue** + nowy PR.

## Zasady dotyczące AI

Celem projektu jest nauka, więc **kod piszemy sami**. Asystenci AI (Copilot, ChatGPT itp.) są dozwoleni jako narzędzie, ale nie jako autor zadania:

**Można:**

- pytać AI o wyjaśnienie błędu, dokumentacji, pojęcia z Godota lub C#,
- prosić o wyjaśnienie cudzego kodu,
- używać AI do redakcji dokumentacji,
- poprosić AI o podpowiedź „jak podejść do problemu”, a potem napisać kod samodzielnie.

**Trzeba:**

- **każdy fragment kodu, który napisało AI, oznaczyć komentarzem** — sposób oznaczania (i przykład) jest w [CODE_STYLE.md](CODE_STYLE.md#znacznik-kodu-napisany-przez-ai),
- napisać w opisie PR-a, że użyto AI do kodu,
- samemu rozumieć każdą linię, którą wrzucasz do PR-a. Jeśli nie umiesz wyjaśnić kodu na spotkaniu, to znaczy, że jeszcze nie jest gotowy.

**Nie można:**

- **generować assetów przy pomocy AI** — grafiki, dźwięku, muzyki, modeli 3D, fontów, ikon. Dotyczy to też „poprawiania” istniejących assetów.
- oddawać PR-a wygenerowanego w całości przez AI.
- używać kodu z AI bez sprawdzenia licencji lub bez zrozumienia go.

Uzasadnienie jest proste: chcemy, żeby po semestrze każdy wyszedł z projektu z **wiedzą i umiejętnościami**, a nie z listą commitów, których nie potrafi wytłumaczyć.

## Assety i licencje

- Korzystamy z assetów o wolnych licencjach (np. CC0, CC-BY) albo robimy je sami.
- W opisie PR-a przy dodaniu assetu podajemy **źródło, autora i licencję**.
- **Każdy zewnętrzny asset** — nawet taki, którego licencja nie wymaga atrybucji — dopisujemy do rejestru [`Assets/CREDITS.md`](../Assets/CREDITS.md). Instrukcję i wzór wiersza znajdziesz w tym pliku.
- Jeśli licencja wymaga dołączenia treści, zapisujemy ją w `Assets/Licenses/<NazwaAssetu>-LICENSE.txt` i linkujemy z rejestru.
- Duże assety (grafika, audio, modele, fonty) przechodzą przez Git LFS automatycznie, zgodnie z `.gitattributes`. Jeśli dodajesz nowy typ pliku, dodaj go w tym samym PR-ze: `git lfs track "*.ext"`.
- Nie wrzucamy assetów z niejasnym pochodzeniem ani „znalezionych w internecie”.

## Definition of Done

Zadanie jest skończone, gdy:

- [ ] działa zgodnie z opisem zadania, na cudzym komputerze też,
- [ ] `dotnet build` przechodzi bez błędów i bez nowych ostrzeżeń,
- [ ] kod jest zgodny z [CODE_STYLE.md](CODE_STYLE.md),
- [ ] PR ma wymagane zatwierdzenia (2, w tym opiekun), a komentarze z review są rozwiązane,
- [ ] zmiana została przetestowana, a wynik opisany w PR-ze,
- [ ] dokumentacja (`README`, `docs/SETUP.md`, `docs/CODE_STYLE.md`) jest zaktualizowana, jeśli zmiana tego wymaga,
- [ ] karta na tablicy jest w kolumnie *Done*, a gałąź usunięta.

## Zabezpieczenia repozytorium (dla administratorów)

Zasady z tego dokumentu mają być wymuszone przez GitHuba, żeby nie zależeć od dobrej woli. Docelowe ustawienie gałęzi `main`:

| Ustawienie | Wartość |
|---|---|
| Require a pull request before merging | ✅ |
| └ Required approvals | **2** |
| └ Dismiss stale approvals when new commits are pushed | ✅ |
| └ Require review from Code Owners | ✅ |
| Require status checks to pass (`dotnet build`) | ✅ |
| Require branches to be up to date before merging | ✅ |
| Require conversation resolution before merging | ✅ |
| Block force pushes / uniemożliwienie usunięcia gałęzi | ✅ |
| Do not allow bypassing the above settings | ✅ |

Kto jest „osobą nadzorującą”: właściciele kodu wpisani w [`.github/CODEOWNERS`](../.github/CODEOWNERS) — GitHub automatycznie prosi ich o review w każdym nowym PR-ze. Wymóg „Require review from Code Owners” sprawia, że co najmniej jedno z dwóch zatwierdzeń musi pochodzić od nich.

**Stan:** ochrona gałęzi `main` jest włączona w *Settings → Branches*. Check `issue-branch` jest tylko informacyjny (nie jest wymagany). Repozytorium jest publiczne, a GitHub udostępnia te funkcje dla publicznych repozytoriów bezpłatnie (dla prywatnych część wymagałaby płatnego planu). Ustawienia zmienia wyłącznie administrator i nie wyłączamy ich „na chwilę”, żeby coś szybko wrzucić — od tego jest gałąź i PR.

---

Pytania, których nie ma w tym dokumencie, są sygnałem, że dokument wymaga dopisania — zgłoś to jako issue typu Feature z etykietą `documentation`.
