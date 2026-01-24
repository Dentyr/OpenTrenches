using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace OpenTrenches.Common.Contracts;

/// <summary>
/// Message pack serialization and deserialization service using custom options
/// </summary>
public static class Serialization
{
    public static readonly MessagePackSerializerOptions StandardOptions = MessagePackSerializerOptions.Standard.WithResolver(OpenTrenchResolver.Instance);

    public static byte[] Serialize<T>(T obj) => MessagePackSerializer.Serialize<T>((T)obj, StandardOptions);
    public static T Deserialize<T>(ReadOnlyMemory<byte> message) => MessagePackSerializer.Deserialize<T>(message, StandardOptions);
    public static bool TryDeserialize<T>(ReadOnlyMemory<byte> message, [NotNullWhen(true)] out T item)
    {
        try
        {
            item = MessagePackSerializer.Deserialize<T>(message, StandardOptions);
            return item is not null;
        }
        catch 
        { 
            item = default!;
            return false; 
        }
    }
}

/// <summary>
/// Composite resolver for all types in this project
/// </summary>
public sealed class OpenTrenchResolver : IFormatterResolver
{
    public static readonly IFormatterResolver Instance = CompositeResolver.Create(StandardResolver.Instance, MessagePackGodot.GodotResolver.Instance);

    public IMessagePackFormatter<T>? GetFormatter<T>() => Instance.GetFormatter<T>();
}

