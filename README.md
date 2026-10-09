# PocketTopo 1.372 — powtarzalna dekompilacja

Lokalne repozytorium badawcze zawierające oryginalne wejście, skrypt
odtwarzający dekompilację oraz uzyskany kod C# i IL. Skrypt korzysta z
ILSpyCmd i przypiętych bibliotek referencyjnych .NET 2.0. Nie wymaga Wine,
SDK .NET ani uruchamiania PocketTopo. Narzędzia pobiera do lokalnego cache;
nie instaluje ich globalnie.

## Szybki start

Wymagania: Python 3.9 lub nowszy, `curl`, dostęp do sieci przy pierwszym
odtworzeniu. Odtworzenie i zgodność bajtów sprawdzono na macOS arm64.
Skrypt wybiera także przypięte środowiska dla macOS x64, Linux arm64/x64
i Windows arm64/x64; tych platform nie przetestowano w tej sesji.

Uruchom z katalogu tego repozytorium:

```sh
python3 scripts/decompile.py --check
```

Polecenie weryfikuje oryginalne archiwum, pobiera pakiety o przypiętych
wersjach i sumach, odtwarza C# oraz IL w katalogu tymczasowym, a następnie
porównuje każdy plik z `decompiled/`. Kod wyjścia `0` oznacza identyczność
bajt w bajt, wraz z manifestem i listą plików. Różnice lub diagnostyka
dekompilatora kończą się kodem `1`. Zapisany wynik pozostaje dostępny.

Przegląd integralności bez narzędzi i bez sieci:

```sh
python3 scripts/decompile.py --verify
```

Ponowne odtworzenie bez pobierania, po zapełnieniu cache:

```sh
python3 scripts/decompile.py --check --offline
```

Zapis świeżego wyniku do nowego katalogu:

```sh
python3 scripts/decompile.py --output /tmp/pockettopo-new-result
```

Katalog `--output` musi jeszcze nie istnieć. Domyślne uruchomienie bez
argumentów tworzy `decompiled/`, gdy go brakuje; gdy wynik już istnieje,
odtwarza go i porównuje, tak jak `--check`. Ścieżki wejścia i domyślnego
wyniku są ustalane względem skryptu, więc można uruchamiać go także z
innego katalogu. `--cache PATH` wskazuje alternatywny cache pakietów.

## Zawartość

```text
inputs/PocketTopo-1.372.zip  oryginalny pakiet autora, zachowane bajty
tools.lock.json            wersje, adresy, SHA-256/SHA-512 narzędzi i wejść
scripts/decompile.py       pobranie, weryfikacja, dekompilacja i porównanie
decompiled/csharp/         główny program, projekt .csproj i zasoby
decompiled/localizations/  zasoby de, fr, pl, sl
decompiled/PocketTopo.il   cały kod IL głównego EXE
decompiled/manifest.json   pochodzenie, skrót skryptu i lockfile, statystyki
decompiled/SHA256SUMS      sumy wszystkich plików wynikowych
decompiled/logs/           stdout/stderr poszczególnych wywołań ILSpy
analysis/ANALIZA.txt        mapa kodu i pierwsze obserwacje
tests/test_decompile.py    testy integralności i bezpiecznego odtwarzania
.cache/downloads/          pobrane narzędzia, pomijane przez Git
```

`decompiled/` jest wynikiem generowanym. Do zmian algorytmów lub budowania
portu warto utworzyć osobny katalog roboczy, zachowując ten wynik jako
odniesienie. Skrypt porównuje rzeczywiste pliki, a nie tylko deklarowane
sumy. Odrzuca też zmienione archiwa, brakujące i dodatkowe wyniki oraz
niebezpieczne ścieżki w paczkach.

## Wejście i narzędzia

- [PocketTopo autora, wersja 1.372](https://paperless.bheeb.ch/PocketTopo13.html).
  Archiwum pobrano z
  [oficjalnego adresu](https://paperless.bheeb.ch/download/PocketTopo.zip)
  i zachowano w `inputs/`.
- [ILSpyCmd](https://github.com/icsharpcode/ILSpy): `9.1.0.7988`.
- Środowisko wykonawcze .NET: `8.0.31`; archiwa i ich SHA-512 pochodzą z
  [metadanych wydań Microsoft](https://builds.dotnet.microsoft.com/dotnet/release-metadata/8.0/releases.json).
- [Microsoft.NETFramework.ReferenceAssemblies.net20](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net20/1.0.3): `1.0.3`.

SHA-256 oryginalnego `PocketTopo.exe`:

```text
048cf57b874238ac142f1e4d3446af639ce3760b4a91724ea00bcc320eb5c371
```

SHA-256 zachowanego archiwum:

```text
2f06a51ed7e5cf3755339cd3c9e645779fb7295eaf66643c83c92ab65b12f3b9
```

W `tools.lock.json` są również skróty wszystkich czterech bibliotek
tłumaczeń. Przeniesienie repozytorium nie wymaga istniejącej instalacji
PocketTopo. Pakiety narzędzi pozostają dostępne w cache do pracy offline;
nie są częścią historii Git.

## Jak powstaje wynik

Skrypt sprawdza wejście i pakiety, rozpakowuje je do nowego katalogu
tymczasowego i uruchamia ILSpyCmd przez pobrane środowisko .NET. Dla
głównego EXE używa `-p --nested-directories` do projektu C# oraz `-il`
do IL. Następnie dekompiluje cztery biblioteki tłumaczeń. `-r` wskazuje
przypięte biblioteki .NET 2.0, a `--disable-updatecheck` wyłącza dodatkowe
sprawdzanie aktualizacji.

Tekstowe wyniki otrzymują zakończenia linii LF; zasoby binarne pozostają
bez zmian. Manifest nie zawiera daty wykonania ani ścieżek tymczasowych,
aby kolejne przebiegi były porównywalne. Zawiera SHA-256 skryptu i lockfile,
więc zmiana procedury jest widoczna w pochodzeniu wyników.

Pierwsza próba dekompilacji używała bibliotek .NET 2.0 z instalacji Wine.
Przejście na przypięty pakiet referencyjny zmieniło tylko nazwy zmiennych
lokalnych w trzech plikach interfejsu: `MainForm.cs`, `MapView.cs`,
`SketchView.cs`. Wynik tego repozytorium pochodzi z opisanej tu procedury.

## Stan wyniku

Główny EXE: 56 plików C#, 15 469 linii, 73 typy, 849 metod — 829 z ciałem
IL, 16 abstrakcyjnych i 4 importy natywne. Wszystkie sześć wywołań
dekompilatora zakończyło się kodem `0`, bez diagnostyki.

To rekonstrukcja z kompilacji. Nie odzyskuje komentarzy autora, historii
projektu ani wszystkich oryginalnych nazw zmiennych. Wygenerowanego
projektu nie rekompilowano; zgodność bajtów dekompilacji nie jest testem
działania odbudowanej aplikacji. Implementacje systemowe `coredll.dll`
oraz firmware DistoX znajdują się poza wejściowym EXE. Pochodzenie
odtworzonego kodu pozostaje związane z programem Beat Heeba.

## Testy skryptu

```sh
python3 -m unittest discover -s tests -v
```

Testy nie pobierają pakietów. Pełny test integracyjny to `--check`, który
naprawdę uruchamia przypięty dekompilator i porównuje cały wynik.
