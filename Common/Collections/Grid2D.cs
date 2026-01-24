using System;
using System.Collections.Generic;
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

    public Grid2D(int SizeX, int SizeY, Func<T>? Initializer = null)
    {
        this.SizeX = SizeX;
        this.SizeY = SizeY;
        Grid = new T[SizeX][];
        for (int x = 0; x < SizeX; x ++) 
        {
            Grid[x] = new T[SizeY];
            if (Initializer is not null) for (int y = 0; y < SizeY; y ++) Grid[x][y] = Initializer.Invoke();
        }
    }

    public TReturn[][] CopySelect<TReturn>(Func<T, TReturn> selector) => [.. Grid.Select(x => x.Select(selector).ToArray())];
    public T[][] CopyTiles() => [.. Grid.Select(static x => x.ToArray())];

    public IEnumerable<T> GetTiles() => Grid.SelectMany(static x => x);

    public void Fill(T value)
    {
        for (int x = 0; x < SizeX; x ++) for (int y = 0; y < SizeY; y ++) Grid[x][y] = value;
    }
}

public interface IGrid2D<T>
{
    public T this[int x, int y] { get; set; }
    public int SizeX { get; }
    public int SizeY { get; }
}