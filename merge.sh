#/bin/bash
project="MutagenMerger.CLI/MutagenMerger.CLI.csproj"

files=$(echo "$_data"$(cat "$MO2_Instance/profiles/$profile/modlist.txt" | tac | grep -ve '^-' -ve '^#' -ve '^\*' | sed 's#^\+#'"$MO2_Instance"'/mods/#' | sed  's/.*/:&/g' | tr -d "\n\r"));
cp "$MO2_Instance/profiles/$profile/plugins.txt" "$LocalAppData/$game/Plugins.txt"

unionfs -o allow_other "$files" "$data"

output="${3:-$MO2_Instance}/mods/$1"
mergename="$1.esp"
dotnet run --project "$project" --framework net7.0  --game "$game" --mergefile "$2" --data "$data" --output "$output" --mergename "$mergename" | tee log.log

fusermount -u "$data"
