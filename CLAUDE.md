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

- `IctBaden.RevolutionPi/` — **the library** (**net10.0**, **nullable enabled**, no external deps —
  System.Text.Json only). Key files:
  - `Interop.cs` — `[DllImport("libc")]` P/Invoke (`open/close/lseek/read/write/ioctl`) + `piControl.h`
    ioctl constants (`KB_*`, `_IO/_IOC`). aarch64-correct widths (`nint`/`nuint`); `IOC_VOID = 0`.
  - `PiControl.cs` — driver wrapper (process-image `Read`/`Write` via lseek+read/write; `GetBitValue`/
    `SetBitValue`/`Reset` via ioctl; `ReadVariable`/`ConvertDataToValue`). Implements `IPiControl`.
  - `IPiControl.cs` — `Read`/`Write` seam so `RevPiLeds` bit-packing is unit-testable with a fake buffer.
  - `RevPiLeds.cs` — packs A1/A2/A3/Watchdog into one process-image LED byte
    (A1=bits0-1, A2=2-3, A3=4-5, Watchdog=bit7). Takes `IPiControl`; null-read-guarded.
  - `Configuration/PiConfiguration.cs` — parses `/etc/revpi/config.rsc` with `System.Text.Json.Nodes`.
  - `Model/` — `SpiValue` (matches driver `SPIValue {__u16,__u8,__u8}`, 4 bytes), `SpiVariable`
    (matches `SPIVariable`, 38 bytes incl. pad), `VariableInfo`/`DeviceInfo` (STJ-parsed via
    `DeviceInfo.FromJson`), `JsonScalar` (string-or-number coercion the config relies on), …
- `IctBaden.RevolutionPi.Test/` — **net10 NUnit** tests (SDK-style, `Microsoft.NET.Test.Sdk` + `NUnit` +
  `NUnit3TestAdapter`). In the **parent** `NovaalertStatusForwarderService.slnx`, so the parent CI
  `dotnet test` runs them: `ConfigurationTests`, `ConvertDataToValueTests`, `RevPiLedsTests`,
  `StructLayoutTests`.
- `PiTest.Core/` — net10 **interactive console sample** (the live one; `linux-arm64`).
- `VariableServer/` — REST debug tool exposing RevPi variables (`GET /`, `/variables`,
  `/variables/{name}`, `/variables/{name}/{prop}`). **Ported to ASP.NET Core (net10) minimal API**
  (`Microsoft.NET.Sdk.Web`, top-level `Program.cs`, System.Text.Json camelCase, response compression +
  permissive CORS). Defaults to `http://*:8000` (override via `--urls`). Startup tolerates a missing
  driver/config so it runs off-device. In `RevolutionPi.sln` only — **not** in the parent slnx/CI.

> Pruned in the hardening pass: `IctBaden.RevolutionPi.Standard` (netstandard2.0 duplicate of the lib),
> legacy v4.8 `PiTest` (superseded by `PiTest.Core`), and committed binaries/cruft (`NuGet.exe`,
> `VariableServer.zip`, `*.nuspec`, `build.bat`/`publish.bat`/`SetPacketVersion.ps1`, `MigrationBackup/`).

## Vendor references (authoritative)

- Driver repo: `https://gitlab.com/revolutionpi/piControl` (GitHub mirror:
  `https://github.com/RevolutionPi/piControl`). Header `piControl.h` defines the ioctl ABI.
- Validated: `KB_IOC_MAGIC='K'`; `KB_RESET=_IO('K',12)`, `KB_GET_VALUE=_IO('K',15)`,
  `KB_SET_VALUE=_IO('K',16)` — all `_IO` (void direction). `SPIValue { __u16 i16uAddress; __u8 i8uBit;
  __u8 i8uValue; }` (4 bytes). The fork's ioctl **numbers are correct**; only integer **widths** are wrong.

## Current state

The fork-hardening pass is **done** on branch `updates` (committed). The committed HEAD (`e638b12`) and
`origin/master` predate it. History on `updates`: a **baseline** commit folding in the prior SDK-style
migration (`RevPiLeds` A3/Watchdog, `ConvertDataToValue` case-4 fix), then the **hardening** commit
(Workstreams A–E below). `PLAN-fork-hardening.md` (untracked, kept on disk) was the working plan.

## Done in the hardening pass

1. **Interop 64-bit widths** (`Interop.cs` + `PiControl.cs`): `lseek`→`nint` offset/return;
   `read`/`write`→`nuint count`/`nint` return; `ioctl_*`→`nuint cmd`; dropped `using off_t=Int32`.
   `byte[]` (LPArray) marshaling was already correct. Validated against the real driver header
   `D:\source\repos\RevolutionPi\piControl\src\piControl.h`.
2. **`IOC_VOID` reverted to `0x00000000`** — Linux `_IO` uses `_IOC_NONE = 0` (the working tree had the
   BSD `0x20000000`, which produced wrong ioctl command numbers, e.g. `KB_GET_VALUE` → `0x20004B0F`).
3. **`RevPiLeds.LedByte` null-read guard** — `Read` returning `null` now yields `0` instead of an NRE.
4. **`SpiVariable` ABI fix** — was `[MarshalAs(ByValArray)]` on a `string` + missing the `__u8 pad`;
   now `ByValTStr[32]` + explicit pad → 38 bytes (matches `SPIVariable`). Unused today (no
   `KB_FIND_VARIABLE` caller) but now correct. Size-locked by `StructLayoutTests`.
5. **Pruned** the `.Standard` duplicate, legacy `PiTest`, and committed binaries/cruft (see Layout note).
6. **Repo hygiene**: normalized to **UTF-8 (no BOM) / LF** (`.editorconfig` + `.gitattributes`), so the
   parent `dotnet format` gate no longer needs `--exclude ./RevolutionPi/` (dropped from CI). Lib csproj
   metadata → btc ownership, `GeneratePackageOnBuild=false`.
7. **Lib → net10**, **`<Nullable>enable</Nullable>`**, and **dropped Newtonsoft.Json** for
   `System.Text.Json.Nodes` (`JsonScalar` handles the config's string-encoded numbers); `PiTest.Core`
   retargeted net8→**net10** + repointed to the lib. The lib now has **no external package deps**.
8. **Tests**: `IctBaden.RevolutionPi.Test` modernized to net10/NUnit (SDK-style) and added to the parent
   slnx so CI `dotnet test` runs it; added `IPiControl` seam + `ConvertDataToValue`/`RevPiLeds`/struct-size
   coverage.

## Deferred

- Publishing the NuGet package (consumed via ProjectReference; `GeneratePackageOnBuild=false`).
- LED byte layout for newer RevPi models (Connect 4 / Flat use more / RGB LEDs) — hardware-specific, not
  in `piControl.h`; verify on the device.

## Conventions & gotchas

- **The parent CI builds this lib with `-warnaserror`** (via the ProjectReference in the parent slnx) and
  the parent has `<Nullable>enable</Nullable>`. The lib is **nullable-enabled** too — keep both the lib and
  any public API it exposes warning-clean (the parent consumes only ctors / `Open()` / LED props, so the
  blast radius is small, but don't widen nullable holes in the public surface). net10 analyzers like CA2101
  apply — the `libc` string P/Invoke uses `CharSet.Ansi` to stay clean.
- **The LED byte layout is hardware-specific** and is **not** defined in `piControl.h`. Newer RevPi models
  (Connect 4 / Flat) use more / RGB LEDs with a different process-image layout. Verify against the actual
  device before relying on it.
- This submodule has its own solution `RevolutionPi.sln` (lib + `PiTest.Core` + `VariableServer` + test
  project) and its **own CI** (`.github/workflows/ci.yml`, ubuntu): restore → build Release `-warnaserror`
  → `dotnet format --verify-no-changes` → test with coverage → ReportGenerator HTML (uploaded as an
  artifact, summary in the job page), on push to `master`/`updates` and PRs. The parent separately builds
  the lib + test project via its slnx.
- **Code coverage** (Coverlet + ReportGenerator, the latter pinned in `.config/dotnet-tools.json`) must run
  in **Debug** — the lib's Release `DebugType=none` strips the PDBs Coverlet needs. See README → *Tests &
  code coverage*.
