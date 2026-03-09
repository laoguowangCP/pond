using Godot;
using System;
using LGWCP.NiceGD;
using System.Collections.Generic;

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

    protected static Dictionary<string, int> KVLocaleItemId = new() {
        {"zh", 0},
        {"en", 1},
        {"ja", 2}
    };
    protected static Dictionary<int, string> KVItemIdLocale = new() {
        {0, "zh"},
        {1, "en"},
        {2, "ja"}
    };
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryGetNodeFromEntity<OptionButton>(NP_OptionButton, out OptionButton);
        OptionButton.ItemSelected += OnItemSelected;
        SelectItemByLocale();
        return true;
    }

    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
        Holder.TryGetComponent<OnStickerRemove>(out OnStickerRemove);
        OnStickerRemove.StickerRemove += UnregistLangSticker;
    }

    public override bool OnHolderTryRemove()
    {
        OnStickerRemove.StickerRemove -= UnregistLangSticker;
        return base.OnHolderTryRemove();
    }

    private void SelectItemByLocale()
    {
        string locale = TranslationServer.GetLocale();
        if (KVLocaleItemId.TryGetValue(locale, out int id))
        {
            OptionButton.Select(id);
        }
    }


    private void OnItemSelected(long index)
    {
        /*
        if (index == 0)
        {
            TranslationServer.SetLocale("zh");
        }
        else if (index == 1)
        {
            TranslationServer.SetLocale("en");
        }
        else if (index == 2)
        {
            TranslationServer.SetLocale("ja");
        }
        */
        if (KVItemIdLocale.TryGetValue((int)index, out var locale))
        {
            TranslationServer.SetLocale(locale);
        }
        Nice.I.TryTick(TickGroupEnum.SetLocale, new From(this));
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

