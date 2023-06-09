#!/bin/bash

cd "$(dirname "$0")" || exit

if [ ! -d "./prefix" ]; then
    mkdir prefix
fi

if [ "$(uname -m)" == "x86_64" ]
then :
    WINEPREFIX=$(pwd)/prefix/ ./wine-lutris-GE-Proton8-8-x86_64/lutris-GE-Proton8-8-x86_64/bin/wine64 barchette.exe
else :
    WINEPREFIX=$(pwd)/prefix/ ./wine-lutris-GE-Proton8-8-x86_64/lutris-GE-Proton8-8-x86_64/bin/wine barchette.exe
fi