using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;


namespace LGWCP.Util.Save;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SaveNodeBase), typeDiscriminator: "SaveNodeBase")]
[JsonDerivedType(typeof(SaveRoot), typeDiscriminator: "SaveRoot")]
[JsonDerivedType(typeof(SaveSticker), typeDiscriminator: "SaveSticker")]
[JsonDerivedType(typeof(SaveStickerTip), typeDiscriminator: "SaveStickerTip")]
[JsonDerivedType(typeof(SaveStickerPhoto), typeDiscriminator: "SaveStickerPhoto")]
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

    public void Clear()
    {
        if (IsLeaf)
        {
            return;
        }

        foreach (var child in ListChildren)
        {
            child.Clear();
        }
        ListChildren.Clear();

        foreach (var child in MapChildren.Values)
        {
            child.Clear();
        }
        MapChildren.Clear();
    }
}

public class SaveRoot : SaveNodeBase
{
    public SaveRoot(bool isLeaf = true) {}
    public string Version = "0.1";
    public string Time;
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
    public string Text;
}

public class SaveStickerPhoto : SaveSticker
{
    public SaveStickerPhoto(bool isLeaf = true) {}
    public string ImageFile;
}

