using System;
using System.IO;
using System.Text.RegularExpressions;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Regexy;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class TipStickerUpdateUri : ComponentResource
{
    public override Type ComponentType => typeof(TipStickerUpdateUri);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;
    protected static readonly NodePath NP_UriBtn = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/LinkButton";
    protected Button UriBtn;
    protected static readonly NodePath NP_FolderBtn = "./EntityControl/PanelContainer/VBoxContainer/HBoxContainer/FolderButton";
    protected Button FolderBtn;
    protected static NodePath NP_TextEdit = "./EntityControl/PanelContainer/VBoxContainer/TextEdit";
    protected TextEdit TextEdit;
    protected string UriStr;

    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<Button>(NP_UriBtn, out UriBtn);
        Holder.TryGetNodeFromEntity<Button>(NP_FolderBtn, out FolderBtn);
        Holder.TryGetNodeFromEntity<TextEdit>(NP_TextEdit, out TextEdit);

        // TextEdit.TextChanged += OnTextEditTextChanged;
        TextEdit.FocusExited += UpdateUriFromTextEdit;
        UriBtn.Pressed += OnUriBtnPressed;
        FolderBtn.Pressed += OnFolderBtnPressed;
        return true;
    }

    public override bool OnHolderTryRemove()
    {
        // TextEdit.TextChanged -= OnTextEditTextChanged;
        TextEdit.FocusExited -= UpdateUriFromTextEdit;
        return base.OnHolderTryRemove();
    }

    public void UpdateUriFromTextEdit()
    {
        // Text get uri
        string text = TextEdit.Text;

        Match match;

        // Uri
        // Windows file
        match = Regexy.WinFileRegex.Match(text);
        if (match.Success)
        {
            var value = match.Value;
            UriBtn.Visible = true;
            UriBtn.Text = value;
            UriBtn.TooltipText = value;
            UriStr = value;
            FolderBtn.Visible = File.Exists(value)
                || Directory.Exists(value);
            return;
        }

        // Link
        match = Regexy.UriRegex.Match(text);
        if (match.Success)
        {
            UriBtn.Visible = true;
            UriBtn.Text = match.Value;
            UriBtn.TooltipText = match.Value;
            UriStr = match.Value;
            return;
        }

        // GD.Print("Uri reg fail.");
        UriBtn.Visible = false;
        FolderBtn.Visible = false;
    }

    protected void OnUriBtnPressed()
    {
        OS.ShellOpen(UriStr);
    }

    private void OnFolderBtnPressed()
    {
        OS.ShellShowInFileManager(UriStr);
    }
}
