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

    protected static NodePath NP_TextEdit = "./EntityControl/PanelContainer/VBoxContainer/TextEdit";
    protected TextEdit TextEdit;
    protected TipStickerChangeFontSize ChangeFontSize;


    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        Holder.TryGetNodeFromEntity<TextEdit>(NP_TextEdit, out TextEdit);
        return true;
    }

    public override void OnEntityReady()
    {
        // Holder.TryGetNode<TextEdit>(NP_TextEdit, out TextEdit);
        Holder.TryGetComponent<TipStickerChangeFontSize>(out ChangeFontSize);
    }

    public override void Tick(TickContext ctx)
    {
        if (ctx.From.Is<SaveRoot>(out var save))
        {
            Holder.TryGetEntity<Node2D>(out var entity);
            Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
            SaveStickerTip tipSave = new(false)
            {
                GlobalPosition = entity.GlobalPosition,
                Size = control.Size,
                FontSizeId = ChangeFontSize.FontSizeId,
                Text = TextEdit.Text
            };
            save.TryAddListChildrenWithIdx(entity.GetIndex(), tipSave);
        }
    }

    public void Load(SaveStickerTip save)
    {
        Holder.TryGetEntity<Node2D>(out var entity);
        Holder.TryGetNodeFromEntity<Control>("./EntityControl", out var control);
        entity.GlobalPosition = save.GlobalPosition;
        control.Size = save.Size;
        ChangeFontSize.SetFontSizeById(save.FontSizeId);
        TextEdit.Text = save.Text;
    }
}
