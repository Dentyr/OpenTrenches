using Godot;

namespace OpenTrenches.Common.Scene;
public static class NodeExtensions
{
    public static void QueueFreeDeferred(this Node node)
    {
        node.CallDeferred(nameof(node.QueueFree));
    }
}