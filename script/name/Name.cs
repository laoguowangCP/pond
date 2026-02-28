using System;
using Godot;

namespace LGWCP.Pond;

public class Name
{
    public static readonly NodePath NP_PhotoStickerTextureRect = "./EntityControl/PanelContainer/VBoxContainer/ScrollContainer/TextureRect";


    // -------- Input map --------
    public static readonly StringName SN_Ctrl = "ctrl";
    public static readonly StringName SN_CtrlScrollUp = "ctrl_scroll_up";
    public static readonly StringName SN_CtrlScrollDown = "ctrl_scroll_down";
    public static readonly StringName SN_CtrlPlus = "ctrl_plus";
    public static readonly StringName SN_CtrlMinus = "ctrl_minus";
    public static readonly StringName SN_CtrlMouseLeft = "ctrl_mouse_left";
    public static readonly StringName SN_CtrlMouseRight = "ctrl_mouse_right";
}
