using System;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class StickerTipTextCtrl : ComponentResource
{
    public override Type ComponentType => typeof(StickerTipTextCtrl);
    public override TickGroupEnum TickGroup => TickGroupEnum.Save;
    public override bool IsRegist => false;

    protected static NodePath NP_TextEdit = "./EntityControl/TextEdit";
    protected TextEdit TextEdit;

    public override void OnEntityReady()
    {
        Holder.TryGetNodeFromEntity<TextEdit>(NP_TextEdit, out TextEdit);
    }

    public void LoadText(string text)
    {
        TextEdit.Text = text;
        if (Holder.TryGetComponent<TipStickerUpdateUri>(out var updateUri))
        {
            updateUri.UpdateUriFromTextEdit();
        }
    }
}
