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

    private static readonly HashSet<string> ImageExtensionsSupported = new() { ".bmp", ".dds", ".kts", ".exr", ".hdr", ".jpg", ".jpeg", ".png", ".tga", ".svg", ".webp" };
    private static readonly HashSet<string> AudioExtensionsSupported = new() { ".mp3", ".ogg", ".wav" };

    // private static readonly string ImageFileFolder = "/save/image/";

    protected StringBuilder FileToBuilder = new();

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
            return;
        }

        string file = files[0];
        if (string.IsNullOrEmpty(file))
        {
            // Empty
            return;
        }

        string fileExt = Path.GetExtension(file).ToLowerInvariant();
        if (File.Exists(file)
            && !string.IsNullOrEmpty(fileExt))
        {
            if (ImageExtensionsSupported.TryGetValue(fileExt, out _))
            {
                // Handle image
                HandleDropFileImage(file);
            }
            else if (AudioExtensionsSupported.TryGetValue(fileExt, out _))
            {
                // Handle audio
                HandleDropFileAudio(file);
            }
            else
            {
                // Godot file_dropped not supported
                // Fallback plain text
                HandleDropFileFallbackPlainText(file);
            }
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
        if (!Filey.TryAddTimestampToFileNameIfCollide(ref FileToBuilder))
        {
            return;
        }

        string fileTo = FileToBuilder.ToString();

        Directory.CreateDirectory(fileTo.GetBaseDir());
        File.Copy(fileFrom, fileTo);

        StickerBuilder.BuildStickerPhoto(fileTo, out var photo);

        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        photo.Position = mousePos;
    }

    protected void HandleDropFileAudio(string fileFrom)
    {
        // 0. Build target file path
        FileToBuilder.Clear();
        FileToBuilder.Append(OS.GetUserDataDir());
        FileToBuilder.Append(NameSave.AudioFolder);
        FileToBuilder.Append(fileFrom.GetFile());

        // 1. Check file path valid
        if (!Filey.TryAddTimestampToFileNameIfCollide(ref FileToBuilder))
        {
            return;
        }

        string fileTo = FileToBuilder.ToString();

        Directory.CreateDirectory(fileTo.GetBaseDir());
        File.Copy(fileFrom, fileTo);

        StickerBuilder.BuildStickerSound(fileTo, out var sound);

        Vector2 mousePos = Entity.GetGlobalMousePosition();
        if (Holder.TryGetComponent<DragArea>(out var dragArea))
        {
            mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
        }
        sound.Position = mousePos;
    }
}
