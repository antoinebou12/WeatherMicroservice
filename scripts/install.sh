#!/bin/bash

# https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x ./dotnet-install.sh
./dotnet-install.sh --version 6.0.408
./dotnet-install.sh
./dotnet-install.sh --runtime dotnet
./dotnet-install.sh --runtime aspnetcore