# IPMasking

Jednoduchá konzolová aplikace pro práci s IPv4 sítěmi a kontrolu, zda zadaná IP adresa patří do určité sítě.

Projekt obsahuje:

- `Program.cs` – konzolová aplikace, která čte síť v CIDR formátu a následně kontroluje zadané IP adresy.
- `Utilities/NetworkUtils.cs` – pomocná statická třída `NetworkUtils` s metodami pro parsování IPv4, parsování sítí ve tvaru `x.x.x.x/x` a kontrolu náležení adresy do sítě.
- `IPMasking.csproj` – projektové nastavení.
- `IPMasking.Tests` – projekt s unit testy

Požadavky

- .NET 9 SDK
- C# 13

Jak sestavit a spustit

1. Otevřete terminál v adresáři projektu (adresář obsahuje `IPMasking.csproj`).
2. Sestavte projekt:

   `dotnet build`

3. Spusťte aplikaci:

   `dotnet run --project IPMasking.csproj`

Použití

1. Program nejprve vyžaduje zadání sítě ve formátu `x.x.x.x/x` (např. `192.168.1.0/24`).
2. Poté můžete zadávat IP adresy (např. `192.168.1.5`). Program ověří, zda jsou adresy v síti, nebo hlásí neplatný vstup.
3. Prázdný vstup ukončí zadávání IP adres a program skončí.

Příklad interakce

- Vstup: `192.168.1.0/24`
- Kontrolované adresy: `192.168.1.10` → v síti, `10.0.0.1` → mimo síť

Testování

- Testy jsou napsány pomocí xUnit frameworku.
- Jsou v projektu `IPMasking.Tests`, testy spusťíte příkazem:

  `dotnet test`
