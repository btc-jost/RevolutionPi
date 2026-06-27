# RevolutionPi .NET Library
This library allows to create software for the RevolutionPi, an Open Source IPC based on Raspberry Pi (Zero)
using C# (C-Sharp) and .NET 10.    

> btc.jost AG fork of the unmaintained [FrankPfattheicher/RevolutionPi](https://github.com/FrankPfattheicher/RevolutionPi),
> modernized to .NET 10 (aarch64 / RevolutionPi).

It supports the  data exchange with RevPi I/O-Modules and Gateways
using the piControl driver.

Visit the RevolutionPi homepage https://revolution.kunbus.de/ for more information.

This code is using information provided by    
Copyright (C) 2016 : KUNBUS GmbH, Heerweg 15C, 73370 Denkendorf, Germany    
See GitHub repository https://github.com/RevolutionPi/piControl for more information.

#### Tools and Samples included in Repository
* PiTest.Core command line sample (similar to piTest provided by Kunbus)
* [Simple REST-API Variable Server](VariableServer.md) (ASP.NET Core, net10)

#### System Requirements
* .NET 10 runtime on the RevolutionPi (aarch64)

#### Development Requirements
* .NET 10 SDK

#### Tests & code coverage
Run the unit tests (`IctBaden.RevolutionPi.Test`, NUnit):

    dotnet test RevolutionPi.sln

Collect coverage (Coverlet) and generate an HTML report (ReportGenerator, restored as a local tool).
Coverage must run in **Debug** — the library uses `DebugType=none` in Release, so there are no PDBs to
instrument:

    dotnet test RevolutionPi.sln -c Debug --collect:"XPlat Code Coverage" --results-directory ./coverage
    dotnet tool restore
    dotnet reportgenerator -reports:"coverage/**/coverage.cobertura.xml" -targetdir:"coverage/report" -reporttypes:Html

Open `coverage/report/index.html`. CI runs the same flow and publishes the report as a build artifact.
The driver/`libc` P/Invoke paths (`Interop`, most of `PiControl`) are not exercised off-device, so overall
coverage is dominated by the testable parsing/LED logic.

#### More Information
* [RevolutionPi Homepage](https://revolution.kunbus.de/)
* [API Reference](ApiReference.md)
* [Kernel module for data exchange with RevPi I/O-Modules and Gateways (on GitHub)](https://github.com/RevolutionPi/piControl)
* Bugs and Comments - please use GitHub issue report

#### Open topics
* **On-device verification / newer hardware.** The `libc` P/Invoke path (`lseek`/`read`/`write`/`ioctl`)
  and the aarch64 width fixes can only be fully verified on a real RevolutionPi — they are not exercised by
  the unit tests or off-device. The system-LED byte layout (A1=bits0-1, A2=2-3, A3=4-5, Watchdog=bit7) is
  hardware-specific and **not** defined in `piControl.h`; newer models (RevPi Connect 4 / Flat) use more /
  RGB LEDs with a different process-image layout, so `RevPiLeds` must be verified against the actual device
  before relying on it there.
* **NuGet packaging.** The library is currently consumed via `ProjectReference`
  (`GeneratePackageOnBuild=false`). Publishing a btc-owned NuGet package is deferred until there is a second
  consumer that needs it.

#
#### License
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 

