# Solax.InverterHttpApi

A .NET 8 library for reading and decoding real-time data from Solax inverter **SOLAX X3 Hybrid G4** with WiFi dongle and HTTP API. It provides strongly-typed access to inverter data, including battery, panel, and grid metrics, with multi-language support.

## Features
- Async API for reading inverter data via HTTP
- Decodes raw data into named, typed values
- Support for multiple languages (EN, DE, FR, ES, CZ)
- Strongly-typed enums for units, parameters, and operation modes
- Extensible for additional Solax models

## Installation
Add a reference to the `Solax.InverterHttpApi` project in your .NET 8 solution.

## Usage Example
```csharp
using Solax.InverterHttpApi;

var data = await Reader.GetDataAsync("http://192.168.1.68", "password");
var decoded = new SolaxDataList(data, SolaxDataList.Lang.English);
var batteryPower = decoded.GetValue(SolaxDataList.Parameter.BatteryPower);
Console.WriteLine(batteryPower);
```

## API Overview
- `Reader.GetDataAsync(Uri, password)`: Reads raw data from the inverter dongle
- `SolaxDataList`: Decodes raw data into named values
- `SolaxValue`: Represents a single decoded value
- `BatteryModesDict` / `InverterModesDict`: Maps operation mode codes to names

## Supported Parameters
See `SolaxDataList.Parameter` enum for all supported metrics (e.g., BatteryPower, ExportedPower, NetworkVoltagePhase1, etc).

## License
MIT (see repository for details)

## Contributing
Pull requests and issues are welcome!
