using Godot;
using System;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class LangStickerRegist : ComponentResource
{
    public override Type ComponentType => typeof(LangStickerRegist);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;

    protected static readonly NodePath NP_OptionButton = "./EntityControl/PanelContainer/VBoxContainer/OptionButton";
    protected OptionButton OptionButton;
    
    protected OnStickerRemove OnStickerRemove;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryGetNodeFromEntity<OptionButton>(NP_OptionButton, out OptionButton);
        OptionButton.ItemSelected += OnItemSelected;
        return true;
    }

    private void OnItemSelected(long index)
    {
        throw new NotImplementedException();
    }


    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
        Holder.TryGetComponent<OnStickerRemove>(out OnStickerRemove);
        OnStickerRemove.StickerRemove += UnregistLangSticker;
    }


    private void UnregistLangSticker()
    {
        if (Nice.I.TryGetRegistedComponentFirst<BgMainPopupMenu>(out var mainPopupMenu))
        {
            mainPopupMenu.LangSticker = null;
        }
    }

    /*
    public override void Tick(TickContext ctx)
    {
        // Do tick
    }
    */

    /*
    public override bool OnHolderTryRemove()
    {
        return base.OnHolderTryRemove();
        // Simply return true, or custom recycle component.
        // Holder = null; return true;
    }
    */

    /*
    public override void OnActivated() {}
    public override void OnDeactivated() {}

    public override bool ShouldDeactivate()
    {
        return false;
    }

    public override bool ShouldActivate()
    {
        // If not blocked, always activated.
        return true;
    }
    */

}

