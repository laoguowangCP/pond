using Godot;
using System;
using LGWCP.NiceGD;
using LGWCP.Extension;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class BgMainPopupMenu : ComponentResource
{
    public override Type ComponentType => typeof(BgMainPopupMenu);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => true;

    [Export]
    protected NodePath NP_PopupMenu;
    public PopupMenu PopupMenu;

    [Export]
    protected NodePath NP_BgPanel;
    protected PanelContainer BgPanel;

    protected Node2D Entity;
    public Node2D LangSticker = null;
    
    public override bool OnHolderTryAdd(ComponentHolder holder)
    {
        Holder = holder;
        // Get nodes here
        // Add tags here.
        Holder.TryGetEntity<Node2D>(out Entity);
        Holder.TryGetNode<PopupMenu>(NP_PopupMenu, out PopupMenu);
        PopupMenu.Visible = false;
        // PopupMenu.Exclusive = true;
        PopupMenu.IdPressed += OnIdPressed;
        Holder.TryGetNode<PanelContainer>(NP_BgPanel, out BgPanel);
        BgPanel.GuiInput += OnGuiInput;
        return true;
    }


    public override void OnEntityReady()
    {
        // Get other components here.
        // Add tick order here.
    }

    public override bool OnHolderTryRemove()
    {
        BgPanel.GuiInput -= OnGuiInput;
        return base.OnHolderTryRemove();
    }

    private void OnIdPressed(long id)
    {
        int index = PopupMenu.GetItemIndex((int)id);
        if (id == 100)
        {
            if (Holder.TryGetComponent<WindowEmbedDesktop>(out var windowEmbed))
            {
                windowEmbed.SwitchEmbed();
            }
        }
        else if (id == 200)
        {
            // TODO: show/hind tutorial stickers
        }
        else if (id == 201)
        {
            // TODO: show/hind language stickers
            if (LangSticker == null)
            {
                if (Holder.TryGetComponent<StickerBuilder>(out var builder))
                {
                    builder.BuildStickerLanguage(out var langSticker);
                    Vector2 mousePos = Entity.GetGlobalMousePosition();
                    if (Holder.TryGetComponent<DragArea>(out var dragArea))
                    {
                        mousePos = dragArea.GetGlobalPositionRegulated(mousePos);
                    }
                    langSticker.Position = mousePos;
                    LangSticker = langSticker;
                }
                // PopupMenu.SetItemText(index, "Dismiss language sticker.");
            }
            else
            {
                if (Holder.TryGetComponent<HandleStickerInSceneTree>(out var handleSticker))
                {
                    handleSticker.RemoveSticker(LangSticker);
                }
                // PopupMenu.SetItemText(index, "Show language sticker.");
            }
        }
        else if (id == 300)
        {
            if (Holder.TryGetComponent<OnSaveGame>(out var onSaveGame))
            {
                bool isChecked = !PopupMenu.IsItemChecked(index);
                PopupMenu.SetItemChecked(index, isChecked);
                onSaveGame.IsRestoreWindowPositionOnStart = isChecked;
            }
        }
        else if (id == 400)
        {
            if (Holder.TryGetComponent<OnSaveGame>(out var onSaveGame))
            {
                bool isChecked = !PopupMenu.IsItemChecked(index);
                PopupMenu.SetItemChecked(index, isChecked);
                onSaveGame.IsShowTooltip = isChecked;
            }
        }
    }


    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Right
                && mouseButton.IsReleased())
            {
                // PopupMenu.Visible = true;
                // Update text
                if (LangSticker == null)
                {
                    PopupMenu.SetItemText(PopupMenu.GetItemIndex(201), Tr(Name.SN_ShowLanguageSticker));
                }
                else
                {
                    PopupMenu.SetItemText(PopupMenu.GetItemIndex(201), Tr(Name.SN_DismissLanguageSticker));
                }

                Vector2 mousePos = BgPanel.GetGlobalMousePosition();
                PopupMenu.Popup(new Rect2I(Mathf.RoundToInt(mousePos.X), Mathf.RoundToInt(mousePos.Y), 0, 0));
                // PopupMenu.Popup();
            }
        }
    }

    // public bool TryAddItemAndRegist(int id, string text, string tooltip)


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

