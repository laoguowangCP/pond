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

    protected BgMainPopupMenu BgMainPopupMenu;
    public bool IsRestoreWindowPositionOnStart = false;
    public bool IsShowTooltip = true;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        return true;
    }

    public override void OnEntityReady()
    {
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

        Holder.TryGetComponent<BgMainPopupMenu>(out BgMainPopupMenu);
    }

    public void Save()
    {
        if (IsSaving || IsLoading)
        {
            return;
        }

        IsSaving = true;

        SaveRoot.Time = DateTime.Now.ToString("yyyy MM-dd HH:mm:ss.ff");

        SaveRoot.InitWindowSize = DisplayServer.WindowGetSize();
        SaveRoot.InitWindowPos = DisplayServer.WindowGetPosition();
        SaveRoot.IsRestoreWindowPositionOnStart = IsRestoreWindowPositionOnStart;
        SaveRoot.IsShowTooltip = IsShowTooltip;

        SaveRoot.TrLocale = TranslationServer.GetLocale();

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

        Holder.TryGetComponent<StickerBuilder>(out var builder);

        var str = File.ReadAllText(GameSaveFilePath);
        SaveRoot = JsonSerializer.Deserialize<SaveRoot>(str, JsonOptions);

        Vector2I initWindowSize = SaveRoot.InitWindowSize;
        if (initWindowSize.X <= 0 || initWindowSize.Y <= 0)
        {
            initWindowSize = new Vector2I(800, 450);
        }
        DisplayServer.WindowSetSize(initWindowSize);

        IsRestoreWindowPositionOnStart = SaveRoot.IsRestoreWindowPositionOnStart;
        IsShowTooltip = SaveRoot.IsShowTooltip;

        if (IsRestoreWindowPositionOnStart)
        {
            Vector2I initWindowPos = SaveRoot.InitWindowPos;
            if (initWindowPos.Y <= 16)
            {
                initWindowPos.Y = 16;
            }
            DisplayServer.WindowSetPosition(initWindowPos);
        }
        var popupMenu = BgMainPopupMenu.PopupMenu;
        popupMenu.SetItemChecked(popupMenu.GetItemIndex(300), IsRestoreWindowPositionOnStart);
        popupMenu.SetItemChecked(popupMenu.GetItemIndex(400), IsShowTooltip);
        
        // Localization
        string trLocale = SaveRoot.TrLocale;
        if (string.IsNullOrEmpty(trLocale))
        {
            trLocale = OS.GetLocale();
        }
        TranslationServer.SetLocale(trLocale);

        foreach (var save in SaveRoot.ListChildren)
        {
            builder.BuildFromSaveNode(save, out var _);
        }
        SaveRoot.Clear();

        IsLoading = false;
    }
}
