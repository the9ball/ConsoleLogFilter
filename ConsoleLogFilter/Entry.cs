using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Unicode;

namespace ConsoleLogFilter;

internal readonly struct Entry
{
    public readonly string CategoryName;
    public readonly LogLevel LogLevel;
    public readonly int _eventId;
    public readonly string Message;

    public readonly EventId EventId => new(_eventId);

    public Entry(string categoryName, LogLevel logLevel, EventId eventId, string message)
        => (CategoryName, LogLevel, _eventId, Message) = (categoryName, logLevel, eventId.Id, message);

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
        Span<byte> buffer = stackalloc byte[s.Length * 3];
        Utf8.FromUtf16(s.AsSpan(), buffer, out var _, out var writtenBytes);
        Write(stream, writtenBytes);
        stream.Write(buffer.Slice(0, writtenBytes));
        stream.Flush();
    }
}

