using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct SlotData
{
    public bool present;
    public int itemId;
    public byte itemCount;
    public NbtHandler.Tag nbtData;

    public SlotData(bool present, int itemId, byte itemCount, NbtHandler.Tag nbtData)
    {
        this.present = present;
        this.itemId = itemId;
        this.itemCount = itemCount;
        this.nbtData = nbtData;
    }
}
