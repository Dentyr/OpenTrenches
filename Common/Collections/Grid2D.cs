using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenTrenches.Common.Collections;

public class Grid2D<T> : IGrid2D<T>
{
    private T[][] Grid { get; }

    public T this[int x, int y]
    {
        get => Grid[x][y];
        set => Grid[x][y] = value;
    }

    public int SizeX { get; }
    public int SizeY { get; }

    public Grid2D(int SizeX, int SizeY, Func<int, int, T>? Initializer = null)
    {
        this.SizeX = SizeX;
        this.SizeY = SizeY;
        Grid = new T[SizeX][];
        for (int x = 0; x < SizeX; x ++) 
        {
            Grid[x] = new T[SizeY];
            if (Initializer is not null) for (int y = 0; y < SizeY; y ++) Grid[x][y] = Initializer.Invoke(x, y);
        }
    }

    public TReturn[][] CopySelect<TReturn>(Func<T, TReturn> selector) => [.. Grid.Select(x => x.Select(selector).ToArray())];
    public T[][] CopyTiles() => [.. Grid.Select(static x => x.ToArray())];

    public IEnumerable<T> GetGridItems() => Grid.SelectMany(static x => x);

    public void Fill(T value)
    {
        for (int x = 0; x < SizeX; x ++) for (int y = 0; y < SizeY; y ++) Grid[x][y] = value;
    }

    public bool ContainsPosition(int X, int Y)
    {
        return X >= 0 && Y >= 0 && X < SizeX && Y < SizeY;
    }

    public bool TryGet(int x, int y, [NotNullWhen(true)] out T? value)
    {
        if (!ContainsPosition(x, y)) 
        {
            value = default;
            return false;
        }
        else
        {
            value = this[x, y];
            return value is not null;
        }
    }
}

public interface IGrid2D<T>
{
    public T this[int x, int y] { get; set; }
    public int SizeX { get; }
    public int SizeY { get; }
}