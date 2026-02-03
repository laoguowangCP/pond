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
            GD.Print(FileToBuilder.ToString());
            if (File.Exists(FileToBuilder.ToString()))
            {
                // Too short interval between 2 drops.
                return;
            }
        }

        string fileTo = FileToBuilder.ToString();

        GD.Print(fileTo.GetBaseDir());
        Directory.CreateDirectory(fileTo.GetBaseDir());
        File.Copy(fileFrom, FileToBuilder.ToString());

        var photoScene = ResourceLoader.Load<PackedScene>("res://scene/sticker/photo.tscn");
        var photo = photoScene.Instantiate<Node2D>();

        Holder.TryGetEntity<Node>(out var root);
        root.AddChild(photo);
        
        if (photo.TryGetComponent<StickerPhotoLoadImage>(out var loadImage))
        {
            loadImage.LoadFromFile(fileTo);
        }

        Holder.TryGetEntity<Node2D>(out var entity);
        photo.Position = entity.GetGlobalMousePosition();
    }
}
