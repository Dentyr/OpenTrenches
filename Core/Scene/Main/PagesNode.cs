using Godot;
using System;

public partial class PagesNode : Control
{
    /// <summary>
    /// Hides all children except for target of type <typeparamref name="T"/> and returns it if found, otherwise returns null.
    /// </summary>
    public T Raise<T>() where T : CanvasItem
    {
        HideAll();
        T found = GetPage<T>();
        found.Visible = true;
        return found;
    }

    /// <summary>
    /// Finds the first child of type <typeparamref name="T"/>, or throws if failed
    /// </summary>
    public T GetPage<T>() where T : CanvasItem
    {
        foreach(var child in GetChildren())
            if (child is T item) return item;
        throw new Exception("Failed to find page");
    }

    public void HideAll()
    {
        foreach(var child in GetChildren())
            if (child is CanvasItem item) 
                item.Visible = false;
    }
}
