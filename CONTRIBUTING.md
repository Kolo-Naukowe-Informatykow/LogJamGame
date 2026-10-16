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
- [Code review](#code-review)
- [Testowanie po code review](#testowanie-po-code-review)
- [Zasady dotyczące AI](#zasady-dotyczące-ai)
- [Assety i licencje](#assety-i-licencje)
- [Definition of Done](#definition-of-done)

---

## Zanim zaczniesz

1. Uruchom projekt u siebie: [SETUP.md](SETUP.md).
2. Przeczytaj [CODE_STYLE.md](CODE_STYLE.md) — tam są zasady pisania kodu i nazewnictwa.
3. Dołącz do **Discorda koła**. Tam zadajemy pytania „jak to zrobić”, a issues służą do konkretów (błąd, zadanie, decyzja).
4. Sprawdź, czy masz uprawnienia do przypisywania się do zadań na tablicy. Jeśli nie — napisz na Discordzie.

**Nie musisz umieć programować.** Potrzebujemy też osób do testów, dokumentacji, assetów i porządkowania zadań. Zaczynasz od zadania oznaczonego `good first issue`.

## Zadania i tablica

Zadania żyją na **[tablicy GitHub Projects](https://github.com/orgs/Kolo-Naukowe-Informatykow/projects/6)** projektu. Kolumny:

| Kolumna | Znaczenie |
|---|---|
| **Backlog** | Pomysły i zadania jeszcze niegotowe do podjęcia |
| **Todo** | Zadanie gotowe do wzięcia: ma opis, zakres i kryteria akceptacji |
| **In progress** | Ktoś nad nim pracuje (przypisany assignee) |
| **In review** | Otwarty pull request, trwa review |
| **Testing** | Zmiana zatwierdzona, ktoś ją testuje na swoim komputerze |
| **Done** | Zmergowane i przetestowane |

Zasady:

- Bierzesz zadanie → **przypisz się** do issue/karty i przenieś kartę na *In progress*. Nie pracujemy w dwóch nad tym samym.
- Zadanie jest niejasne → pytaj na Discordzie, doprecyzujemy opis (zamiast zgadywać).
- Zadanie jest za duże → podziel je na mniejsze i zgłoś to w komentarzu.
- Otwarty PR → przenieś kartę na *In review* i pilnuj, żeby nie wisiała bez ruchu dłużej niż kilka dni.

## Zgłaszanie błędów (issue)

Znalazłeś błąd? **Załóż issue.** Nie naprawiamy błędów „przy okazji” — także dlatego, że to zadanie dla kogoś, kto chce się nauczyć.

Zanim założysz issue, sprawdź, czy ktoś już go nie zgłosił (przeszukaj issues po słowach kluczowych).

W issue podaj:

- **Co się stało** i co powinno się stać.
- **Kroki do powtórzenia** — numerowana lista, najlepiej od uruchomienia gry.
- **Jak często** — zawsze, za każdym razem po X, tylko czasem.
- **Środowisko** — system, wersja Godota (4.7.2), numer commita lub gałęzi.
- **Materiały** — zrzut ekranu, log z okna *Output*, treść błędu z konsoli.

Etykiety: `bug`, `feature`, `docs`, `good first issue`, `help wanted`, `question`.

Jeśli chcesz sam naprawić zgłoszony błąd — napisz to w komentarzu pod issue i przypisz się. Unikniemy wtedy sytuacji, w której dwie osoby robią to samo.

## Propozycje nowych rzeczy

Pomysł na funkcję, ulepszenie albo usprawnienie repo? Załóż **issue z opisem pomysłu** i poczekaj na akceptację (komentarz od osoby nadzorującej projekt lub decyzja na spotkaniu).

To nie jest biurokracja dla samej siebie: dzięki temu nie powstają duże PR-y, których nie da się przyjąć, bo nie pasują do konceptu gry lub dublują czyjąś pracę.

## Proces zmiany krok po kroku

```bash
# 1. Zaktualizuj main
git switch main
git pull

# 2. Utwórz gałąź dla zadania
git switch -c feature/krotki-opis

# 3. Pracuj i commituj małymi krokami
git add .
git commit -m "Dodaj ruch gracza w osi X"

# 4. Zbuduj i sprawdź, że działa
dotnet build

# 5. Wypchnij gałąź i otwórz pull request
git push -u origin feature/krotki-opis
```

Nazwy gałęzi:

| Prefiks | Do czego |
|---|---|
| `feature/...` | nowa funkcjonalność |
| `fix/...` | naprawa błędu |
| `docs/...` | dokumentacja |
| `refactor/...` | porządki w kodzie bez zmiany działania |
| `chore/...` | konfiguracja, narzędzia, repozytorium |

Zasady:

- **Nigdy nie pushujemy bezpośrednio do `main`.** Zawsze gałąź + pull request.
- Jeden PR = jedna rzecz. Drobne poprawki z review dopisujemy do tego samego PR-a, ale nie dokładamy do niego przypadkowych innych zadań.
- Jeśli `main` poszedł naprzód, zaktualizuj swoją gałąź (`git pull --rebase origin main`) i rozwiąż konflikty u siebie, przed merge.
- Nie zamykamy PR-a bez odpowiedzi na komentarze z review.

## Pull request — wymagania

Każdy PR musi mieć:

- **Minimum 2 zatwierdzenia**, w tym **co najmniej jedno od osoby nadzorującej projekt** (bardziej doświadczonej). Autor nie zatwierdza własnego PR-a i nie merge'uje go bez wymaganych zatwierdzeń — nawet przy zmianie „jednej linijki”.
- **Powiązanie z issue** — w opisie `Closes #123` albo `Refs #123`, żeby zadanie zamknęło się po merge'u.
- **Opis** wypełniony według poniższego szablonu.
- **Zielony build** — `dotnet build` bez błędów i bez nowych ostrzeżeń.
- **Brak plików, których nie powinno być**: `.godot/`, `bin/`, `obj/`, wyeksportowane buildy, lokalne ścieżki, sekrety.

### Szablon opisu PR (do skopiowania)

```markdown
## Co
<!-- Co się zmieniło, w jednym akapicie. -->

## Dlaczego
<!-- Po co to robimy. Powiązane issue: Closes #123 -->

## Jak testować
1. ...
2. ...

## Zrzuty / nagrania
<!-- Dla zmian widocznych w UI. -->

## Checklist
- [ ] Build przechodzi (`dotnet build`)
- [ ] Sprawdzone ręcznie w Godocie (opisałem, co sprawdziłem)
- [ ] Zgodne z CODE_STYLE.md
- [ ] Nie dodałem plików z `.godot/`, `bin/`, `obj/`
- [ ] Jeśli użyłem AI do kodu — oznaczyłem to w kodzie i w tym opisie
- [ ] Zaktualizowałem dokumentację, jeśli zmiana tego wymaga
```

### Merge

- Merge'ujemy **Squash and merge**, żeby historia `main` była czytelna: jedna zmiana = jeden commit.
- Tytuł PR-a staje się treścią commita, dlatego ma być konkretny („Dodaj pauzę w menu”, nie „poprawki”).
- Po merge'u: usuń gałąź, przenieś kartę na *Done*.

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
- Jeśli licencja wymaga atrybucji (np. CC-BY) — dopisujemy asset do `Assets/CREDITS.md` (utwórz ten plik, jeśli jeszcze nie istnieje).
- Duże assety (grafika, audio, modele, fonty) przechodzą przez Git LFS automatycznie, zgodnie z `.gitattributes`. Jeśli dodajesz nowy typ pliku, dodaj go w tym samym PR-ze: `git lfs track "*.ext"`.
- Nie wrzucamy assetów z niejasnym pochodzeniem ani „znalezionych w internecie”.

## Definition of Done

Zadanie jest skończone, gdy:

- [ ] działa zgodnie z opisem zadania, na cudzym komputerze też,
- [ ] `dotnet build` przechodzi bez błędów i bez nowych ostrzeżeń,
- [ ] kod jest zgodny z [CODE_STYLE.md](CODE_STYLE.md),
- [ ] PR ma wymagane zatwierdzenia (2, w tym opiekun), a komentarze z review są rozwiązane,
- [ ] zmiana została przetestowana, a wynik opisany w PR-ze,
- [ ] dokumentacja (README/SETUP/CODE_STYLE) jest zaktualizowana, jeśli zmiana tego wymaga,
- [ ] karta na tablicy jest w kolumnie *Done*, a gałąź usunięta.

---

Pytania, których nie ma w tym dokumencie, są sygnałem, że dokument wymaga dopisania — zgłoś to jako issue z etykietą `docs`.
