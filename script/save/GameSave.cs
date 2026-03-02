using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Godot;


namespace LGWCP.Util.Save;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SaveNodeBase), typeDiscriminator: "SaveNodeBase")]
[JsonDerivedType(typeof(SaveRoot), typeDiscriminator: "SaveRoot")]
[JsonDerivedType(typeof(SaveSticker), typeDiscriminator: "SaveSticker")]
[JsonDerivedType(typeof(SaveStickerTip), typeDiscriminator: "SaveStickerTip")]
[JsonDerivedType(typeof(SaveStickerPhoto), typeDiscriminator: "SaveStickerPhoto")]
[JsonDerivedType(typeof(SaveStickerSound), typeDiscriminator: "SaveStickerSound")]
public interface ISaveNode
{
    public bool IsLeaf { get; set; }
    public List<ISaveNode> ListChildren { get; set; }
    public Dictionary<string, ISaveNode> MapChildren { get; set; }
    public void Clear();
}

public class SaveNodeBase : ISaveNode
{
    public bool IsLeaf { get; set; }
    public List<ISaveNode> ListChildren { get; set; }
    public Dictionary<string, ISaveNode> MapChildren { get; set; }

    public SaveNodeBase(bool isLeaf = false)
    {
        IsLeaf = isLeaf;
        if (!isLeaf)
        {
            ListChildren = new();
            MapChildren = new();
        }
    }

    public bool TryAddListChildrenWithIdx(int idx, ISaveNode saveNode)
    {
        // Expand list
        int requireCnt = idx + 1;
        if (ListChildren.Count < requireCnt)
        {
            /*
            if (ListChildren.Capacity < requireCnt)
            {
                ListChildren.Capacity = requireCnt;
            }
            ListChildren.AddRange(Enumerable.Repeat<ISaveNode>(null, requireCnt - ListChildren.Count));
            */
            CollectionsMarshal.SetCount<ISaveNode>(ListChildren, requireCnt);
        }

        if (ListChildren[idx] == null)
        {
            ListChildren[idx] = saveNode;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Clear()
    {
        if (IsLeaf)
        {
            return;
        }

        foreach (var child in ListChildren)
        {
            child?.Clear();
        }
        ListChildren.Clear();

        foreach (var child in MapChildren.Values)
        {
            child?.Clear();
        }
        MapChildren.Clear();
    }
}

public class SaveRoot : SaveNodeBase
{
    public SaveRoot(bool isLeaf = true) {}
    public string Version = "0.1";
    public string Time;
    public Vector2I InitWindowSize;
    public Vector2I InitWindowPos;
    // public Vector4I WindowRect;
}

public class SaveSticker : SaveNodeBase
{
    public SaveSticker(bool isLeaf = true) {}
    public Vector2 GlobalPosition;
    public Vector2 Size;
}

public class SaveStickerTip : SaveSticker
{
    public SaveStickerTip(bool isLeaf = true) {}
    public int FontSizeId;
    public string Text;
}

public class SaveStickerPhoto : SaveSticker
{
    public SaveStickerPhoto(bool isLeaf = true) {}
    public string ImageFile;
    public Vector2 TextureRectCustomMinimumSize;
    public int HScrollValue;
    public int VScrollValue;
}


public class SaveStickerSound : SaveSticker
{
    public SaveStickerSound(bool isLeaf = true) {}
    public string AudioFile;

    public float TimePlayed;
    public bool IsLoopMode;
    public bool IsVolumeAdjust;
    public float VolumeDb;
}
