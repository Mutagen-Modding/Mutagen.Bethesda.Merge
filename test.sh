#/bin/sh
export LocalAppData="/mnt/mediaSSD/SteamLibrary/steamapps/compatdata/489830/pfx/drive_c/users/steamuser/AppData/Local/"
project="MutagenMerger.CLI/MutagenMerger.CLI.csproj"
game="SkyrimSE"
data="/mnt/mediaSSD/SteamLibrary/steamapps/common/Skyrim Special Edition/Data"
output="/home/monyarm/Downloads/TestMerge"
mergename="TestMerge.esp"
dotnet run --project "$project" --game "$game" --mergefile SkyrimMerge.txt --data "$data" --output "$output" --mergename "$mergename" | tee log.log
