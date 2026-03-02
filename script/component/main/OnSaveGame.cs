using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Save;
using LGWCP.Util.WinApiNative;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class OnSaveGame : ComponentResource
{
    public override Type ComponentType => typeof(OnSaveGame);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected bool IsSaving = false;
    protected bool IsLoading = false;
    protected string GameSaveFileDir;
    protected string GameSaveFilePath;
    protected JsonSerializerOptions JsonOptions = new();

    protected SaveRoot SaveRoot = new();


    public override void OnEntityReady()
    {
        /*
        // Trying figure out BlitzMap hash problem
        foreach(var (t, c) in Holder.KVComponents)
        {
            GD.Print(t, " ", c);
            GD.Print(Holder.KVComponents.Keys.Contains(t));
            GD.Print(Holder.KVComponents.TryGetValue(t, out var comp));
        }
        */
        GameSaveFileDir = OS.GetUserDataDir() + NameSave.Folder;
        GameSaveFilePath = OS.GetUserDataDir() + NameSave.Folder + NameSave.SaveFile;
        if (!File.Exists(GameSaveFilePath))
        {
            Directory.CreateDirectory(GameSaveFileDir);
            // File.Create(GameSaveFilePath).Close();
        }

        JsonOptions.IncludeFields = true;
        JsonOptions.AllowTrailingCommas = true;
        JsonOptions.WriteIndented = true;
        JsonOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
    }

    public void Save()
    {
        if (IsSaving || IsLoading)
        {
            return;
        }

        IsSaving = true;
        GD.Print("Save!");

        SaveRoot.Time = DateTime.Now.ToString("yyyy MM-dd HH:mm:ss.ff");

        SaveRoot.InitWindowSize = DisplayServer.WindowGetSize();
        SaveRoot.InitWindowPos = DisplayServer.WindowGetPosition();

        // Call save tick
        Nice.I.TryTick(TickGroupEnum.Save, new From(SaveRoot));

        // Serialize file
        string jsonStrGameSave = JsonSerializer.Serialize(SaveRoot, JsonOptions);
        File.WriteAllText(GameSaveFilePath, jsonStrGameSave);

        // Test deserialize
        // var deSave = JsonSerializer.Deserialize<GameSave>(jsonStrGameSave, JsonOptions);
        SaveRoot.Clear();

        IsSaving = false;
    }

    public void Load()
    {
        if (IsSaving || IsLoading)
        {
            return;
        }

        if (!File.Exists(GameSaveFilePath))
        {
            return;
        }
        IsLoading = true;
        GD.Print("Load!");

        Holder.TryGetComponent<StickerBuilder>(out var builder);

        var str = File.ReadAllText(GameSaveFilePath);
        SaveRoot = JsonSerializer.Deserialize<SaveRoot>(str, JsonOptions);

        Vector2I initWindowSize = SaveRoot.InitWindowSize;
        if (initWindowSize.X <= 0 || initWindowSize.Y <= 0)
        {
            initWindowSize = new Vector2I(800, 450);
        }
        DisplayServer.WindowSetSize(initWindowSize);

        /*
        Vector2I initWindowPos = SaveRoot.InitWindowPos;
        if (initWindowPos.Y <= 16)
        {
            initWindowPos.Y = 16;
        }
        DisplayServer.WindowSetPosition(initWindowPos);
        */

        foreach (var save in SaveRoot.ListChildren)
        {
            builder.BuildFromSaveNode(save, out var _);
        }
        SaveRoot.Clear();

        IsLoading = false;
    }
}
