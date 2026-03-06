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
    public static readonly StringName SN_ShiftScrollUp = "shift_scroll_up";
    public static readonly StringName SN_ShiftScrollDown = "shift_scroll_down";
    public static readonly StringName SN_ScrollUp = "scroll_up";
    public static readonly StringName SN_ScrollDown = "scroll_down";
    public static readonly StringName SN_CtrlPlus = "ctrl_plus";
    public static readonly StringName SN_CtrlMinus = "ctrl_minus";
    public static readonly StringName SN_CtrlMouseLeft = "ctrl_mouse_left";
    public static readonly StringName SN_CtrlMouseRight = "ctrl_mouse_right";
    public static readonly StringName SN_ShiftMouseLeft = "shift_mouse_left";
    public static readonly StringName SN_MouseLeft = "mouse_left";

    public static readonly StringName SN_ShowLanguageSticker = "OPT.ShowLanguageSticker";
    public static readonly StringName SN_DismissLanguageSticker = "OPT.DismissLanguageSticker";
    public static readonly StringName SN_DragHereToDelete = "BTN.DragHereToDelete";
    public static readonly StringName SN_HoldAndDrag = "BTN.HoldAndDrag";
    public static readonly StringName SN_CreateNewTip = "BTN.CreateNewTip";
    public static readonly StringName Tooltip_VolumeAdjustOn = "TOOLTIP.VolumeAdjustOn";
    public static readonly StringName ToolTip_VolumeAdjustOff = "TOOLTIP.VolumeAdjustOff";
    public static readonly StringName Tooltip_LoopModeOn = "TOOLTIP.LoopModeOn";
    public static readonly StringName Tooltip_LoopModeOff = "TOOLTIP.LoopModeOff";
}
