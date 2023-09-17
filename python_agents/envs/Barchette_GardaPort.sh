#!/bin/bash

cd "$(dirname "$0")" || exit
cd "./Barchette_GardaPort" || exit

if [ ! -d "./prefix" ]; then
    mkdir prefix
fi

if [ "$(uname -m)" == "x86_64" ]
then :
    WINEPREFIX=$(pwd)/prefix/ ./wine-lutris-GE-Proton8-8-x86_64/lutris-GE-Proton8-8-x86_64/bin/wine64 Barchette_GardaPort.exe
else :
    WINEPREFIX=$(pwd)/prefix/ ./wine-lutris-GE-Proton8-8-x86_64/lutris-GE-Proton8-8-x86_64/bin/wine Barchette_GardaPort.exe
fi