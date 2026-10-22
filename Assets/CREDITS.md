# Assety i licencje (CREDITS)

Ten plik jest rejestrem **wszystkich zewnętrznych assetów** użytych w grze: grafiki, dźwięków, muzyki, modeli 3D, fontów, shaderów i innych materiałów, których nie wykonali członkowie koła.

Rejestr ma dwa cele:

- **zgodność z licencjami** — część licencji (np. CC-BY, OFL) wymaga podania autora i źródła,
- **porządek w projekcie** — po latach nadal wiadomo, skąd pochodzi każdy plik i na jakich zasadach można go używać.

Plik trafia też do każdego archiwum wydania (obok buildów), żeby atrybucje były dostępne razem z grą.

## Zasady

- Używamy assetów na wolnych licencjach (np. **CC0**, **CC-BY**, **OFL**) albo robimy je sami. Licencje share-alike (np. **CC-BY-SA**) mogą wymagać udostępnienia całej gry na tej samej licencji — takie przypadki uzgadniamy z opiekunami projektu.
- **Zabronione jest generowanie assetów przez AI** (grafika, dźwięk, muzyka, modele, fonty) — szczegóły w [CONTRIBUTING.md](../docs/CONTRIBUTING.md#zasady-dotyczące-ai).
- Nie dodajemy assetów o niejasnym pochodzeniu ani „znalezionych w internecie” bez strony źródłowej.
- Przed dodaniem assetu sprawdź, czy licencja pozwala na: **modyfikację**, **użycie komercyjne** (projekt może być kiedyś sprzedawany) i **dystrybucję w buildzie**. Jeśli nie masz pewności — zapytaj opiekunów projektu.
- Jeśli asset został zmodyfikowany (przycięty, przekonwertowany, zmieniony kolor), **odnotuj to** w kolumnie „Zmiany”. Licencje CC-BY wymagają wskazania zmian.
- Jeden wiersz = jeden asset albo jeden spójny pakiet (np. cały zestaw ikon z jednej paczki). Dla pakietu podaj folder, nie wypisuj dziesiątek plików.

## Jak dodać asset do rejestru

1. Wrzuć plik do odpowiedniego folderu w `Assets/` (`Textures/`, `Audio/`, `Models/`, `Fonts/`). Nazwy plików w PascalCase.
2. Dopisz **jeden wiersz** w tabeli poniżej.
3. W kolumnie **Źródło** podaj **bezpośredni link** do strony assetu (najlepiej do konkretnej wersji lub permalinka), a nie do wyszukiwarki.
4. Jeśli licencja wymaga dołączenia jej treści, zapisz ją jako plik tekstowy w `Assets/Licenses/<NazwaAssetu>-LICENSE.txt` i wpisz ścieżkę w kolumnie **Licencja**.
5. W opisie PR-a napisz, że dodajesz asset, i podaj źródło, autora oraz licencję (wymóg z [CONTRIBUTING.md](../docs/CONTRIBUTING.md#assety-i-licencje)).
6. Jeżeli asset jest widoczny w grze, docelowo powinien pojawić się też w ekranie **Credits** w samej grze — to osobne zadanie, ale wpis w tym pliku jest źródłem prawdy.

### Przykład wypełnionego wiersza

```markdown
| `Assets/Textures/Kenney/Platformer/` | Zestaw kafelków platformowych | Kenney | https://kenney.nl/assets/platformer-pack-redux | CC0 1.0 | Przycięte do 32×32 px | 2026-10-22 |
```

## Rejestr assetów

| Plik / folder | Co to jest | Autor | Źródło | Licencja | Zmiany | Data dodania |
|---|---|---|---|---|---|---|
| — | _Na razie brak zewnętrznych assetów._ | — | — | — | — | — |

## Licencje

- **CC0 1.0** — [treść licencji](https://creativecommons.org/publicdomain/zero/1.0/)
- **CC-BY 4.0** — [treść licencji](https://creativecommons.org/licenses/by/4.0/)
- **CC-BY-SA 4.0** — [treść licencji](https://creativecommons.org/licenses/by-sa/4.0/)
- **SIL Open Font License 1.1** — [treść licencji](https://openfontlicense.org/)

Dla licencji, których nie ma na liście, wpisz identyfikator SPDX i link do treści licencji.

Jeśli w projekcie pojawi się folder `Assets/Licenses/`, jego zawartość również trafi do archiwów wydania razem z tym plikiem.
