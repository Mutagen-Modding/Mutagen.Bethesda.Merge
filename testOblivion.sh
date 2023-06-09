#/bin/sh
export LocalAppData="/mnt/mediaSSD/SteamLibrary/steamapps/compatdata/22330/pfx/drive_c/users/steamuser/AppData/Local/"
project="MutagenMerger.CLI/MutagenMerger.CLI.csproj"
game="Oblivion"
data="/mnt/mediaSSD/SteamLibrary/steamapps/common/Oblivion/Data"
output="/home/monyarm/Downloads/TestMerge"
mergename="TestMerge.esp"
dotnet run --project "$project" --game "$game" --mergefile OblivionMerge.txt --data "$data" --output "$output" --mergename "$mergename" | tee log.log
