using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BackgroundClipboardPaste : ComponentResource
{
    public override Type ComponentType => typeof(BackgroundClipboardPaste);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_BgPanel = "./UICanvasLayer/BackgroundPanel/GridContainer/PanelMain";
    protected Control BgPanel;
    protected Node2D Entity;
    protected DragArea DragArea;
    protected OnSaveGame OnSaveGame;
    protected StickerBuilder StickerBuilder;

    protected StringName SN_UiPaste = "ui_paste";
    protected StringName SN_MouseLeft = "mouse_left";

    protected StringBuilder FileToBuilder = new();
    protected bool MousePressedLock = false;

    protected List<string> ClipBoardFiles = new();

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetNodeFromEntity<Control>(NP_BgPanel, out BgPanel);
        // Add tags here.
        Holder.AddTagNoCheck(this, TagEnum.FgHover);
        return true;
    }

    public override void OnEntityReady()
    {
        Holder.TryGetComponent<DragArea>(out DragArea);
        Holder.TryGetComponent<OnSaveGame>(out OnSaveGame);
        Holder.TryGetComponent<StickerBuilder>(out StickerBuilder);
        BgPanel.GuiInput += OnGuiInput;
        // BgPanel.FocusEntered += OnFocusEntered;
        // BgPanel.FocusExited += OnFocusExited;
    }

    public override bool OnHolderTryRemove()
    {
        BgPanel.GuiInput -= OnGuiInput;

        return base.OnHolderTryRemove();
    }

    private void OnFocusEntered()
    {
        Holder.UnblockByTag(TagEnum.ShowDragArea);
    }

    private void OnFocusExited()
    {
        Holder.BlockByTag(TagEnum.ShowDragArea);
    }


    protected void OnGuiInput(InputEvent @event)
    {
        /*
        if (Input.IsActionJustPressed("mouse_left"))
        {
            BgPanel.GrabFocus();
        }
        */

        // Paste command

        // Set ime position
        /*
        Vector2 mousePos = Entity.GetGlobalMousePosition();
        Window window = Entity.GetWindow();
        // window.SetImePosition(new Vector2I((int)mousePos.X, (int)mousePos.Y));
        */
        using(@event)
        {
            if (Input.IsActionJustReleased(SN_UiPaste))
            // if (Input.IsActionJustReleased(SN_UiPaste)
            //     && MousePressedLock)
            {
                // Start paste from clip board
                OnClipboardRead();
                OnSaveGame.Save();
            }
        }
    }

    // protected void OnFocusEntered
    protected void OnClipboardRead()
    {
        User32Native.GetCopiedFiles(ref ClipBoardFiles);
        if (Holder.TryGetComponent<WindowOnFilesDropped>(out var onFilesDropped))
        {
            foreach (var file in ClipBoardFiles)
            {
                onFilesDropped.HandleFileToSticker(file);
            }
        }
        else if (DisplayServer.ClipboardHasImage())
        {
            Image image = DisplayServer.ClipboardGetImage();
            HandleClipboardReadImage(image);
        }
        else if (DisplayServer.ClipboardHas())
        {
            var text = DisplayServer.ClipboardGet();
            HandleClipboardReadFallbackPlainText(text);
        }
    }

    protected void HandleClipboardGetFile(Image image)
    {
        
    }

    protected void HandleClipboardReadImage(Image image)
    {
        // 0. Build target file path
        FileToBuilder.Clear();
        FileToBuilder.Append(OS.GetUserDataDir());
        FileToBuilder.Append(NameSave.ImageFolder);
        FileToBuilder.Append("Clipboard");


        // 1. Check file path valid
        FileToBuilder.Append(DateTime.Now.ToString("_yyyyMMdd_HHmmssff"));
        FileToBuilder.Append(".png");
        if (File.Exists(FileToBuilder.ToString()))
        {
            // Too short interval between 2 drops.
            return;
        }

        string fileTo = FileToBuilder.ToString();

        Directory.CreateDirectory(fileTo.GetBaseDir());
        image.SavePng(fileTo);

        StickerBuilder.BuildStickerPhoto(fileTo, out var photo);

        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        photo.Position = mousePos;
    }

    protected void HandleClipboardReadFallbackPlainText(string text)
    {
        StickerBuilder.BuildStickerTip(text, out var tip);
        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        tip.Position = mousePos;
    }
}
