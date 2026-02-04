using System;
using Godot;
using LGWCP.NiceGD;
using LGWCP.Util.Save;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class SaveStickerTipComponent : ComponentResource
{
    public override Type ComponentType => typeof(SaveStickerTipComponent);
    public override TickGroupEnum TickGroup => TickGroupEnum.Save;
    public override bool IsRegist => false;

    protected static NodePath NP_TextEdit = new("./EntityControl/TextEdit");
    protected TextEdit TextEdit;


    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNode<TextEdit>(NP_TextEdit, out TextEdit);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.TryGetNode<TextEdit>(NP_TextEdit, out TextEdit);
    }

    public override void Tick(TickContext ctx)
    {
        if (ctx.From.Is<SaveRoot>(out var save))
        {
            Holder.TryGetEntity<Node2D>(out var entity);
            Holder.TryGetNode<Control>("./EntityControl", out var control);
            SaveStickerTip tipSave = new(false);
            tipSave.Position = entity.Position;
            tipSave.Size = control.Size;
            tipSave.Text = TextEdit.Text;
            save.ListChildren.Add(tipSave);
        }
    }
}
