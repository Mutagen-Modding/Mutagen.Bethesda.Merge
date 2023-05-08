#/bin/sh
export LocalAppData="/mnt/mediaSSD/SteamLibrary/steamapps/compatdata/377160/pfx/drive_c/users/steamuser/AppData/Local/"
project="MutagenMerger.CLI/MutagenMerger.CLI.csproj"
game="Fallout4"
data="/mnt/mediaSSD/SteamLibrary/steamapps/common/Fallout 4/Data"
output="/home/monyarm/Downloads/TestMerge"
mergename="TestMerge.esp"
dotnet run --project "$project" --framework net7.0 --game "$game" --mergefile Fallout4Merge.txt --data "$data" --output "$output" --mergename "$mergename" | tee log.log
