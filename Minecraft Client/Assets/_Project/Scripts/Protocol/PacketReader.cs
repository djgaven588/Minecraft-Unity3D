﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Provides utilities for reading packet data
/// </summary>
public static class PacketReader
{
    /// <summary>
    /// Reads a VarInt from the given reader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The int representation of the VarInt</param>
    public static int ReadVarInt(in BinaryReader reader)
    {
        int value, numRead = 0, result = 0;
        byte read;
        while (true)
        {
            read = reader.ReadByte();
            value = (read & 0x7F);
            result |= (value << (7 * numRead));

            numRead++;
            if (numRead > 5)
                throw new UnityException("VarInt too big!");
            if ((read & 0x80) != 128) break;
        }

        return result;
    }

    /// <summary>
    /// Reads a string from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read string</param>
    public static string ReadString(in BinaryReader reader)
    {
        return reader.ReadString();
    }

    /// <summary>
    /// Reads a GUID from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read GUID</param>
    public static Guid ReadGUID(in BinaryReader reader)
    {
        return new Guid(reader.ReadBytes(16));
    }

    /// <summary>
    /// Reads a boolean from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read boolean</param>
    public static bool ReadBoolean(in BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    /// <summary>
    /// Reads a float from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read float</param>
    public static float ReadSingle(in BinaryReader reader)
    {
        byte[] floatData = reader.ReadBytes(4);
        return BitConverter.ToSingle(floatData.ReverseIfLittleEndian(), 0);
    }

    public static double ReadDouble(in BinaryReader reader)
    {
        byte[] doubleData = reader.ReadBytes(8);
        return BitConverter.ToDouble(doubleData.ReverseIfLittleEndian(), 0);
    }

    /// <summary>
    /// Reads a byte from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read byte</param>
    public static byte ReadByte(in BinaryReader reader)
    {
        return reader.ReadByte();
    }

    /// <summary>
    /// Reads a Int16 from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <returns>The read Int16</returns>
    public static short ReadInt16(in BinaryReader reader)
    {
        byte[] shortData = reader.ReadBytes(2);
        return BitConverter.ToInt16(shortData.ReverseIfLittleEndian(), 0);
    }

    /// <summary>
    /// Reads an unsigned Int16 from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <returns>The read unsigned Int16</returns>
    public static ushort ReadUInt16(in BinaryReader reader)
    {
        byte[] uShortData = reader.ReadBytes(2);
        return BitConverter.ToUInt16(uShortData.ReverseIfLittleEndian(), 0);
    }

    /// <summary>
    /// Reads an int32 (int) from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <returns>The read int32</returns>
    public static int ReadInt32(in BinaryReader reader)
    {
        byte[] intData = reader.ReadBytes(4);
        return BitConverter.ToInt32(intData.ReverseIfLittleEndian(), 0);
    }

    /// <summary>
    /// Reads an int64 (long) from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <returns>The read int64</returns>
    public static long ReadInt64(in BinaryReader reader)
    {
        byte[] intData = reader.ReadBytes(8);
        return BitConverter.ToInt64(intData.ReverseIfLittleEndian(), 0);
    }

    /// <summary>
    /// Reads a byte[] from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="count">The amount of bytes to read</param>
    /// <returns>The read byte[]</returns>
    public static byte[] ReadBytes(in BinaryReader reader, int count)
    {
        return reader.ReadBytes(count);
    }

    /// <summary>
    /// Reads a Position from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="result">The read position (3 ints)</param>
    public static Position ReadPosition(in BinaryReader reader)
    {
        return new Position(reader);
    }

    /// <summary>
    /// Reads a rotation from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="rotation">The read rotation (3 floats as Vector3)</param>
    public static Vector3 ReadRotation(in BinaryReader reader)
    {
        float x = ReadSingle(reader);
        float y = ReadSingle(reader);
        float z = ReadSingle(reader);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Reads SlotData from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="slotData">The read slot data (inventory slot)</param>
    public static void ReadSlotData(in BinaryReader reader, out SlotData slotData)
    {
        bool present = ReadBoolean(reader);
        if (!present)
            slotData = new SlotData(present, 0, 0, new NbtHandler.Tag());
        else
        {
            int itemID = ReadVarInt(reader);
            byte count = ReadByte(reader);
            NbtHandler.Tag nbt = NbtHandler.ProcessStream(reader);
            slotData = new SlotData(present, itemID, count, nbt);
        }
    }
    
    /// <summary>
    /// Reads a Particle from the given BinaryReader
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="particle">The read particle (type and extra data)</param>
    public static Particle ReadParticle(in BinaryReader reader)
    {
        return new Particle(reader);
    }
}
