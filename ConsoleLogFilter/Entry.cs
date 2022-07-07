﻿using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace the9ball.ConsoleLogFilter;

/// <summary>
/// ログエントリ
/// </summary>
internal readonly struct Entry
{
    public readonly string CategoryName;
    public readonly LogLevel LogLevel;
    public readonly int _eventId;
    public readonly string Message;

    public readonly EventId EventId => new(_eventId);

    /// <summary></summary>
    public Entry(string categoryName, LogLevel logLevel, EventId eventId, string message)
        : this(categoryName, logLevel, eventId.Id, message) { }

    /// <summary></summary>
    private Entry(string categoryName, LogLevel logLevel, int eventId, string message)
        => (CategoryName, LogLevel, _eventId, Message) = (categoryName, logLevel, eventId, message);

    /// <summary>
    /// Write to stream
    /// </summary>
    public void Write(Stream stream)
    {
        stream.WriteByte((byte)LogLevel);
        Write(stream, _eventId);
        Write(stream, Message);
        Write(stream, CategoryName);
    }

    private void Write(Stream stream, int i)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        MemoryMarshal.Write(buffer, ref i);
        stream.Write(buffer);
    }

    private void Write(Stream stream, string s)
    {
        Write(stream, s.Length);
        stream.Write(MemoryMarshal.Cast<char, byte>(s.AsSpan()));
        stream.Flush();
    }

    /// <summary>
    /// Read from stream
    /// </summary>
    public static Entry Read(Stream stream)
    {
        var logLevel = stream.ReadByte();
        var eventId = ReadInt(stream);
        var message = ReadString(stream);
        var categoryName = ReadString(stream);
        return new(categoryName, (LogLevel)logLevel, eventId, message);
    }

    private static int ReadInt(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        stream.Read(buffer);
        return MemoryMarshal.Read<int>(buffer);
    }

    private static string ReadString(Stream stream)
    {
        var length = ReadInt(stream);

        Span<byte> readBuffer = stackalloc byte[length * 2];
        stream.Read(readBuffer);
        Span<char> chars = MemoryMarshal.Cast<byte, char>(readBuffer);
        return chars.ToString();
    }
}


