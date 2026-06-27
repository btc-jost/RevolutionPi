# Variable Server
The Variable Server is a simple REST-API http-server providing access to the Revolution Pi variables.
It is an **ASP.NET Core (net10) minimal-API** application.

## Requirements
* .NET 10 runtime on the RevolutionPi (aarch64).

## Usage
Build / publish from sources:

    dotnet publish VariableServer -c Release -r linux-arm64

Copy the publish output to a folder on the RevolutionPi and start it:

    dotnet VariableServer.dll                 # default http://*:8000
    dotnet VariableServer.dll --urls http://*:9000   # custom port

## API
Open a browser and navigate to http://localhost:8000/    
The server should answer with the default document

**GET /**
```json
{
  "service": "RevolutionPi Variable Server",
  "version": "1.0.0.0",
  "varlist": "GET ~/variables",
  "readvar": "GET ~/variables/{varname}",
  "readprop": "GET ~/variables/{varname}/{propname}"
}
```

To query the server for a list of configured variables use the route /variables.    
For each variable its name, type and length is returned.

**GET /variables**
```json
[
  {
    "name": "RevPiStatus",
    "type": "Input",
    "length": "BYTE"
  },
    ...
  {
    "name": "RS485ErrorLimit2",
    "type": "Input",
    "length": "WORD"
  }
]
```

To query the value of a specific variable use the route /variables/{varname}.    
The variable's name, type, length and value is returned.

**GET /variables/RevPiStatus**
```json
{
  "name": "RevPiStatus",
  "type": "Input",
  "length": "BYTE",
  "value": 1
}
```
If the variable name is unknown, the response code is NotFound (404).

**GET /variables/Unknown**
```json
{
  "error": "Variable not found",
  "name": "Unknown"
}
```
If there is a read error, the response code is ServiceUnavailable (503).

**GET /variables/ReadError**
```json
{
  "error": "Could not read variable",
  "name": "ReadError"
}
```
