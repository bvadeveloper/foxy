using System;
using MemoryPack;
using static Force.Crc32.Crc32CAlgorithm;

namespace Platform.Contract.Profiles.Extensions;

public static class SerializationExtensions
{
    /// <summary>
    /// Serialize to byte array with CRC32 value
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    public static byte[] ToBytes(this IProfile profile) =>
        MemoryPackSerializer.Serialize(profile)
            .AsMemory()
            .AddCrc32();

    private static byte[] AddCrc32(this Memory<byte> memory)
    {
        var buffer = new byte[memory.Length + 4]; // magic numbers for crc32 value in payload
        memory.CopyTo(buffer);
        ComputeAndWriteToEnd(buffer);

        return buffer;
    }

    public static byte[] TrimEndBytes(this byte[] value, int length) =>
        value.AsSpan()[..(value.Length - length)].ToArray();
}