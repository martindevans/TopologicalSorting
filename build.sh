#!/usr/bin/env bash

mono .paket/paket.exe install
dotnet build -o nupkg
