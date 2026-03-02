using System;
using System.Collections.Generic;
using Faster.Collections.Pooled;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class TipStickerChangeFontSize : ComponentResource
{
    public override Type ComponentType => typeof(TipStickerChangeFontSize);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static NodePath NP_TextEdit = "./EntityControl/PanelContainer/VBoxContainer/TextEdit";
    protected TextEdit TextEdit;

    // public static readonly StringName SN_CtrlScrollUp = "ctrl_scroll_up";
    // public static readonly StringName SN_CtrlScrollDown = "ctrl_scroll_down";
    public static readonly StringName SN_FontSize = "font_size";

    public static readonly PooledDictionary<int, int> KVFontSize = new()
    {
        {0, 4}, {1, 5}, {2, 6}, {3, 7}, {4, 8}, {5, 9}, {6, 10}, {7, 11},
        {8, 12}, {9, 13}, {10, 14}, {11, 15}, {12, 16}, {13, 17}, {14, 18}, {15, 19},
        {16, 20}, {17, 21}, {18, 22}, {19, 23}, {20, 24}, {21, 25}, {22, 26}, {23, 27},
        {24, 28}, {25, 29}, {26, 30}, {27, 32}, {28, 36}, {29, 40}, {30, 44}, {31, 48},
        {32, 52}, {33, 56}, {34, 60}, {35, 64}, {36, 72}, {37, 80}, {38, 88}, {39, 96},
        {40, 108}, {41, 120}, {42, 132}, {43, 150}, {44, 180}, {45, 240}, {46, 320}, {47, 400},
        {48, 512}, {49, 640}
    };
    public const int DefaultFontSizeId = 14;
    public int FontSizeId { get; protected set; } = DefaultFontSizeId;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextEdit>(NP_TextEdit, out TextEdit);
        SetFontSizeById();
        TextEdit.GuiInput += OnGuiInput;
    }

    public override bool OnHolderTryRemove()
    {
        TextEdit.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    protected void OnGuiInput(InputEvent @event)
    {
        using (@event)
        {
            if (Input.IsActionJustPressed(Name.SN_CtrlScrollUp))
            {
                ChangeFontSize(1);
            }
            else if (Input.IsActionJustPressed(Name.SN_CtrlScrollDown))
            {
                ChangeFontSize(-1);
            }
        }
    }

    public void SetFontSizeById(int fontSizeId = 16)
    {
        if (KVFontSize.TryGetValue(fontSizeId, out int fontSize))
        {
            TextEdit.AddThemeFontSizeOverride(SN_FontSize, fontSize);
            FontSizeId = fontSizeId;
        }
    }

    protected void ChangeFontSize(int change)
    {
        int fontSizeIdNext = FontSizeId + change;
        if (KVFontSize.TryGetValue(fontSizeIdNext, out int fontSize))
        {
            FontSizeId = fontSizeIdNext;
            TextEdit.AddThemeFontSizeOverride(SN_FontSize, fontSize);
        }
    }
}
