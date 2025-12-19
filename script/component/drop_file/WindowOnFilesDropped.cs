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

    private static readonly string ImageFileFolder = "/save/image/";

    public override void OnEntityReady()
    {
        // Hook when window unfocus, exit current drag.
        var window = Holder.GetTree().Root.GetWindow();
        // window.FocusEntered += OnWindowFocusEntered;
        window.FilesDropped += OnWindowFilesDropped;
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
        string fileExt = file.GetExtension();
        if (file == string.Empty)
        {
            GD.Print("File no extension!");
            return;
        }

        if (ImageExtensions.TryGetValue(fileExt.ToLowerInvariant(), out _))
        {
            // Handle image
            HandleDropFileImage(file);
        }
    }

    protected void HandleDropFileImage(string fileFrom)
    {
        // 0. Build target file path
        StringBuilder fileToBuilder = new();
        fileToBuilder.Append(OS.GetUserDataDir());
        fileToBuilder.Append(ImageFileFolder);
        fileToBuilder.Append(fileFrom.GetFile());

        // 1. Check file path valid
        if (File.Exists(fileToBuilder.ToString()))
        {
            string fileExt = fileFrom.GetExtension();
            fileToBuilder.Remove(fileToBuilder.Length - fileExt.Length - 1, fileExt.Length + 1);
            fileToBuilder.Append(DateTime.Now.ToString("_yyyyMMdd_HHmmssff"));
            fileToBuilder.Append('.');
            fileToBuilder.Append(fileExt);
            GD.Print(fileToBuilder.ToString());
            if (File.Exists(fileToBuilder.ToString()))
            {
                // Not human dragging!
                return;
            }
        }

        string fileTo = fileToBuilder.ToString();

        GD.Print(fileTo.GetBaseDir());
        Directory.CreateDirectory(fileTo.GetBaseDir());
        File.Copy(fileFrom, fileToBuilder.ToString());

        var photoScene = ResourceLoader.Load<PackedScene>("res://scene/sticker/photo.tscn");
        var photo = photoScene.Instantiate<Node2D>();

        Holder.TryGetEntity<Node>(out var root);
        root.AddChild(photo);
        
        if (photo.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
        {
            loadImage.LoadFromFile(fileTo);
        }
    }
}
