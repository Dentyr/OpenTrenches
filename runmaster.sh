dotnet build
if [ "$1" = "--debug-view" ]; then
  godot-mono Server/Matchmaking/MasterServer.tscn --headless --debug-view
else
  godot-mono Server/Matchmaking/MasterServer.tscn --headless
fi
