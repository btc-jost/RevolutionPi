# CLAUDE.md — RevolutionPi (btc-jost fork)

Guidance for Claude Code when working in this submodule. This is a **git submodule** of the main
`NovaalertStatusForwarderService` repo, but it is its **own git repository**
(`https://github.com/btc-jost/RevolutionPi`, currently on branch **`updates`**) — commits here go to the
fork, and the parent repo only records the submodule **gitlink**.

## What this is

A **fork** of the now-**unmaintained** `FrankPfattheicher/RevolutionPi`
(upstream remote: `https://github.com/FrankPfattheicher/RevolutionPi.git`). .NET support library for
data exchange with Kunbus **RevolutionPi** I/O modules / gateways over the `piControl` Linux kernel
driver (`/dev/piControl0`).

Only **`IctBaden.RevolutionPi`** (netstandard2.0) is consumed by the parent service
(`NovaalertStatusForwarder`) via a `ProjectReference`. The parent uses: `PiControl` (Open/Close/Read/Write),
`PiConfiguration` (Open/GetVariable), `RevPiLeds` (SystemLedA1/A2/A3, Watchdog), `LedColor`.

### Target hardware
Confirmed **64-bit (aarch64)** RevPi image. This matters for the P/Invoke widths (see Known issues).

## Layout

- `IctBaden.RevolutionPi/` — **the library** (netstandard2.0, C# 9). Key files:
  - `Interop.cs` — `[DllImport("libc")]` P/Invoke (`open/close/lseek/read/write/ioctl`) + `piControl.h`
    ioctl constants (`KB_*`, `_IO/_IOC`).
  - `PiControl.cs` — driver wrapper (process-image `Read`/`Write` via lseek+read/write; `GetBitValue`/
    `SetBitValue`/`Reset` via ioctl; `ReadVariable`/`ConvertDataToValue`).
  - `RevPiLeds.cs` — packs A1/A2/A3/Watchdog into one process-image LED byte
    (A1=bits0-1, A2=2-3, A3=4-5, Watchdog=bit7).
  - `Configuration/PiConfiguration.cs` — parses `/etc/revpi/config.rsc` (Newtonsoft.Json).
  - `Model/` — `SpiValue` (matches driver `SPIValue {__u16,__u8,__u8}`), `VariableInfo`, `DeviceInfo`, …
- `IctBaden.RevolutionPi.Test/` — **real NUnit tests** (`ConfigurationTests`, embedded `config.json`).
  Currently .NET Framework v4.8; to be modernized to net10 and wired into CI.
- `PiTest.Core/` — net8.0 **interactive console sample** (the live one).
- `PiTest/` — legacy v4.8 console sample, **superseded by PiTest.Core** (prune candidate).
- `IctBaden.RevolutionPi.Standard/` — netstandard2.0 **duplicate** of the lib (prune candidate).
- `VariableServer/` — REST debug tool exposing RevPi variables (`GET /variables`, `/variables/{name}`).
  v4.8 + OWIN/`System.Web.Http` + Mono. **Keep** (useful for debugging); a net10 ASP.NET Core port is
  the way to run it on the modern image.

## Vendor references (authoritative)

- Driver repo: `https://gitlab.com/revolutionpi/piControl` (GitHub mirror:
  `https://github.com/RevolutionPi/piControl`). Header `piControl.h` defines the ioctl ABI.
- Validated: `KB_IOC_MAGIC='K'`; `KB_RESET=_IO('K',12)`, `KB_GET_VALUE=_IO('K',15)`,
  `KB_SET_VALUE=_IO('K',16)` — all `_IO` (void direction). `SPIValue { __u16 i16uAddress; __u8 i8uBit;
  __u8 i8uValue; }` (4 bytes). The fork's ioctl **numbers are correct**; only integer **widths** are wrong.

## Current state (IMPORTANT)

There are **uncommitted working-tree changes** on branch `updates` (not yet committed). They are good and
should be kept:
- SDK-style csproj migration (old `v4.0` ToolsVersion → `Microsoft.NET.Sdk`, netstandard2.0,
  `PackageReference Newtonsoft.Json 13.0.3`).
- `RevPiLeds`: added `SystemLedA3` + `Watchdog`, a `LedByte` read-modify-write helper, ctor null-guard.
- `PiControl.ConvertDataToValue`: `case 3:` → `case 4:` — a real bug fix (4-byte values were falling
  through to the ASCII-string default).

The committed HEAD (`e638b12`) and `origin/master` still have the **pre-migration** state.

## Known issues / planned work

A full, approved plan is saved alongside this file: **`PLAN-fork-hardening.md`**. Summary:

1. **Interop 64-bit width bug** (`Interop.cs` + `PiControl.cs`): `off_t`/`size_t`/`ssize_t` and the
   `ioctl` request are 64-bit on aarch64 but declared 32-bit (`int`/`uint`, `using off_t=System.Int32`).
   Fix: `lseek`→`nint` offset/return; `read`/`write`→`nuint count`/`nint` return; `ioctl_*`→`nuint cmd`.
   `byte[]` (LPArray) marshaling is already correct.
2. **`IOC_VOID` regression**: the working tree has `0x20000000` (BSD value). On Linux/aarch64 `_IO` uses
   `_IOC_NONE = 0`, so this produces wrong ioctl command numbers (e.g. `KB_GET_VALUE` → `0x20004B0F`
   instead of `0x4B0F`). **Should be `0x00000000`** — the repo's own commit `fc4da36 "Fixed IOC_VOID
   value"` already set it to 0; the working-tree change re-introduced the bug. Not hit by the LED path
   (which uses read/write, not ioctl), but a latent library bug.
3. **`RevPiLeds.LedByte` getter NRE**: `_control.Read(...)[0]` throws if `Read` returns `null`. Guard it.
4. **Prune** the `.Standard` duplicate + legacy `PiTest`; delete committed binaries (`NuGet.exe` 6.5 MB,
   `VariableServer.zip` 1 MB), `MigrationBackup/`, `packages/`, `.vs/`, `*.bak`.
5. **Repo hygiene**: files are UTF-8-**with-BOM** + **CRLF** → normalize to UTF-8/LF (this is why the
   parent repo's `dotnet format` gate excludes this submodule via `--exclude ./RevolutionPi/`). Update
   csproj package metadata to btc ownership; set `GeneratePackageOnBuild=false`.
6. **Tests**: modernize `IctBaden.RevolutionPi.Test` to net10/NUnit and add it to the parent
   `NovaalertStatusForwarderService.slnx` so the CI `dotnet test` step (today a no-op) runs it. Add a tiny
   `IPiControl` seam to unit-test `RevPiLeds` bit-packing and `ConvertDataToValue`.

## Conventions & gotchas

- **The parent CI builds this lib with `-warnaserror`** (via the ProjectReference in the parent slnx).
  Keep it warning-clean. Staying on netstandard2.0 / nullable-off keeps that easy; if you enable
  `<Nullable>`, you must resolve every resulting warning or the parent build breaks.
- **The LED byte layout is hardware-specific** and is **not** defined in `piControl.h`. Newer RevPi models
  (Connect 4 / Flat) use more / RGB LEDs with a different process-image layout. Verify against the actual
  device before relying on it.
- `nint`/`nuint` are valid at `LangVersion 9` (the lib's setting), so the Interop width fix needs no TFM bump.
- This submodule has its own solution `RevolutionPi.sln`; the parent builds only the lib via ProjectReference.
