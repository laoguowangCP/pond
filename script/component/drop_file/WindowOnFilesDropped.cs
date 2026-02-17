using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;
using LGWCP.Extension;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class WindowOnFilesDropped : ComponentResource
{
    public override Type ComponentType => typeof(WindowOnFilesDropped);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    private static readonly HashSet<string> ImageExtensions = new() { "bmp", "dds", "kts", "exr", "hdr", "jpg", "jpeg", "png", "tga", "svg", "webp" };

    // private static readonly string ImageFileFolder = "/save/image/";

    public StringBuilder FileToBuilder = new();

    protected OnSaveGame OnSaveGame;
    protected StickerBuilder StickerBuilder;
    protected Node2D Entity;

    public override void OnEntityReady()
    {
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.FilesDropped += OnWindowFilesDropped;

        Holder.TryGetComponent<OnSaveGame>(out OnSaveGame);
        Holder.TryGetComponent<StickerBuilder>(out StickerBuilder);
        Holder.TryGetEntity<Node2D>(out Entity);
    }

    public override bool OnHolderTryRemove()
    {
        // Unhook when window unfocus.
        var window = Holder.GetWindow();
        window.FilesDropped -= OnWindowFilesDropped;
        return base.OnHolderTryRemove();
    }
    
    public void OnWindowFilesDropped(string[] files)
    {
        // TODO: Check in drag area

        if (files.Length != 1)
        {
            GD.Print("Too much files dropped!");
            return;
        }

        string file = files[0];
        if (string.IsNullOrEmpty(file))
        {
            // Empty
            return;
        }

        string fileExt = file.GetExtension();
        if (File.Exists(file)
            && !string.IsNullOrEmpty(fileExt)
            && ImageExtensions.TryGetValue(fileExt.ToLowerInvariant(), out _))
        {
            // Handle image
            HandleDropFileImage(file);
        }
        else
        {
            // Godot file_dropped not supported
            // Fallback plain text
            HandleDropFileFallbackPlainText(file);
        }

        OnSaveGame.Save();
    }

    
    protected void HandleDropFileFallbackPlainText(string text)
    {
        StickerBuilder.BuildStickerTip(text, out var tip);
        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        tip.Position = mousePos;
    }

    protected void HandleDropFileImage(string fileFrom)
    {
        // 0. Build target file path
        FileToBuilder.Clear();
        FileToBuilder.Append(OS.GetUserDataDir());
        FileToBuilder.Append(NameSave.ImageFolder);
        FileToBuilder.Append(fileFrom.GetFile());

        // 1. Check file path valid
        if (File.Exists(FileToBuilder.ToString()))
        {
            string fileExt = fileFrom.GetExtension();
            FileToBuilder.Remove(FileToBuilder.Length - fileExt.Length - 1, fileExt.Length + 1);
            FileToBuilder.Append(DateTime.Now.ToString("_yyyyMMdd_HHmmssff"));
            FileToBuilder.Append('.');
            FileToBuilder.Append(fileExt);
            if (File.Exists(FileToBuilder.ToString()))
            {
                // Too short interval between 2 drops.
                return;
            }
        }

        string fileTo = FileToBuilder.ToString();

        Directory.CreateDirectory(fileTo.GetBaseDir());
        File.Copy(fileFrom, fileTo);

        StickerBuilder.BuildStickerPhoto(fileTo, out var photo);

        /*
        var photoScene = ResourceLoader.Load<PackedScene>("res://scene/sticker/photo.tscn");
        var photo = photoScene.Instantiate<Node2D>();

        Holder.TryGetEntity<Node>(out var root);
        root.AddChild(photo);
        
        if (photo.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
        {
            loadImage.LoadFromFile(fileTo);
        }
        */
        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        photo.Position = mousePos;
    }
}
