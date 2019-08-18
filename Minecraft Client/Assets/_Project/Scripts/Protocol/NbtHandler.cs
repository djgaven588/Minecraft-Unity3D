using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class NbtHandler
{
    public static Tag ProcessStream(BinaryReader reader)
    {
        byte startByte = reader.ReadByte();
        Tag output;
        if (startByte == 10)
        {
            // Completely normal NBT data, no funny buisness. Need to back track one byte and then can continue.
            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            ReadNextTag(reader, false, null, out output);
        }
        else if (startByte == 0x1f)
        {
            // We are dealing with GZip data, need to back track one byte and then can continue.
            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            throw new NotImplementedException("GZip compressed NBT data is not yet handled.");
        }
        else if (startByte == 0x78)
        {
            // We are dealing with Zlib data, need to back track one byte and then can continue.
            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            throw new NotImplementedException("Zlib compressed NBT data is not yet handled.");
        }
        else
        {
            throw new Exception("The provided data is not of the correct format! Make sure that what you put in here is NBT data, and not random data. First byte: " + startByte);
        }

        return output;
    }

    private static bool ReadNextTag(BinaryReader reader, bool unnamed, Tag.TypeID? presetId, out Tag result)
    {
        Tag.TypeID id;
        if (presetId != null)
            id = (Tag.TypeID)presetId;
        else
            id = (Tag.TypeID)reader.ReadByte();

        int maxReads = int.MaxValue / 4;
        int currentReads;
        switch (id)
        {
#pragma warning disable IDE0059 // Result is used, but the editor doesn't think that is the case.
            case Tag.TypeID.TagEnd:
                result = new Tag();
                return false;
            case Tag.TypeID.TagByte:
                result = new Tag(id, unnamed ? "" : ReadString(reader), new byte[] { reader.ReadByte() }, null);
                break;
            case Tag.TypeID.TagShort:
                result = new Tag(id, unnamed ? "" : ReadString(reader), reader.ReadBytes(2), null);
                break;
            case Tag.TypeID.TagInt:
                result = new Tag(id, unnamed ? "" : ReadString(reader), reader.ReadBytes(4), null);
                break;
            case Tag.TypeID.TagLong:
                result = new Tag(id, unnamed ? "" : ReadString(reader), reader.ReadBytes(8), null);
                break;
            case Tag.TypeID.TagFloat:
                result = new Tag(id, unnamed ? "" : ReadString(reader), reader.ReadBytes(4), null);
                break;
            case Tag.TypeID.TagDouble:
                result = new Tag(id, unnamed ? "" : ReadString(reader), reader.ReadBytes(8), null);
                break;
            case Tag.TypeID.TagByteArray:
                result = new Tag(id, unnamed ? "" : ReadString(reader), ReadByteArray(reader), null);
                break;
            case Tag.TypeID.TagString:
                result = new Tag(id, unnamed ? "" : ReadString(reader), ReadStringBytes(reader), null);
                break;
            case Tag.TypeID.TagList:
                Queue<Tag> listChildren = new Queue<Tag>();
                currentReads = 0;
                Tag.TypeID childType = (Tag.TypeID)reader.ReadByte();
                while (ReadNextTag(reader, true, childType, out Tag child))
                {
                    listChildren.Enqueue(child);
                    currentReads++;

                    if (currentReads >= maxReads)
                    {
                        throw new Exception("Read next tag continued for too long, this NBT data is corrupted or too long to be considered valid.");
                    }
                }
                result = new Tag(id, ReadString(reader), null, listChildren.ToArray());
                break;
            case Tag.TypeID.TagCompound:
                Queue<Tag> componentChildren = new Queue<Tag>();
                currentReads = 0;
                while (ReadNextTag(reader, false, null, out Tag child))
                {
                    componentChildren.Enqueue(child);
                    currentReads++;

                    if (currentReads >= maxReads)
                    {
                        throw new Exception("Read next tag continued for too long, this NBT data is corrupted or too long to be considered valid.");
                    }
                }
                result = new Tag(id, ReadString(reader), null, componentChildren.ToArray());
                break;
            case Tag.TypeID.TagIntArray:
                result = new Tag(id, ReadString(reader), ReadIntArray(reader), null);
                break;
            case Tag.TypeID.TagLongArray:
                result = new Tag(id, ReadString(reader), ReadLongArray(reader), null);
                break;
            default:
                throw new Exception("NBT data invalid! Invalid type ID: " + id);
#pragma warning restore IDE0059 // Result is used, but the editor doesn't think that is the case.
        }

        throw new Exception("Invalid case, skipped switch case!");
    }

    private static string ReadString(BinaryReader reader)
    {
        ushort length = PacketReader.ReadUInt16(reader);
        return Encoding.UTF8.GetString(reader.ReadBytes(length));
    }

    private static byte[] ReadStringBytes(BinaryReader reader)
    {
        int length = PacketReader.ReadInt32(reader);
        return reader.ReadBytes(length);
    }

    private static byte[] ReadByteArray(BinaryReader reader)
    {
        int length = PacketReader.ReadInt32(reader);
        return reader.ReadBytes(length);
    }

    private static byte[] ReadIntArray(BinaryReader reader)
    {
        int length = PacketReader.ReadInt32(reader);
        return reader.ReadBytes(length * 4);
    }

    private static byte[] ReadLongArray(BinaryReader reader)
    {
        int length = PacketReader.ReadInt32(reader);
        return reader.ReadBytes(length * 8);
    }

    public struct Tag
    {
        public TypeID TagType { get; }
        public string TagName { get; }
        public byte[] TagData { get; }
        public Tag[] Children { get; }

        public Tag(TypeID tagType, string tagName, byte[] tagData, Tag[] children)
        {
            TagType = tagType;
            TagName = tagName;
            TagData = tagData;
            Children = children;
        }

        public enum TypeID : byte
        {
            TagEnd          = 0,
            TagByte         = 1,
            TagShort        = 2,
            TagInt          = 3,
            TagLong         = 4,
            TagFloat        = 5,
            TagDouble       = 6,
            TagByteArray    = 7,
            TagString       = 8,
            TagList         = 9,
            TagCompound     = 10,
            TagIntArray     = 11,
            TagLongArray    = 12
        }
    }

}
