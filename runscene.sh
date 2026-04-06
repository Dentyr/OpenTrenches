dotnet build
if [ "$1" = "--clear" ]; then
    godot-mono Core/Scene/ClientRoot.tscn
else
    godot-mono Core/Scene/ClientRoot.tscn --debug-collisions
fi
