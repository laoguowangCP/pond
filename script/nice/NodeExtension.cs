using System.Linq;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Extension;

public static class NodeExtension
{
    #region NICE

    public static bool TryGetComponentHolder(this Node node, out ComponentHolder holder)
    {
        holder = node.GetNodeOrNull<ComponentHolder>(ComponentHolder.NodePath);
        return holder != null;
    }

    public static bool TryGetComponent<T>(this Node node, out T comp)
        where T : IComponent
    {
        var holder = node.GetNodeOrNull<ComponentHolder>(ComponentHolder.NodePath);
        if (holder != null)
        {
            if (holder.TryGetComponent<T>(out comp))
            {
                return true;
            }
        }

        comp = default;
        return false;
    }

    public static bool TryAddComponent(this Node node, IComponent comp)
    {
        var holder = node.GetNodeOrNull<ComponentHolder>(ComponentHolder.NodePath);
        if (holder != null)
        {
            return holder.TryAddComponent(comp);
        }

        return false;
    }

    public static bool TryRemoveComponent<T>(this Node node)
        where T : IComponent
    {
        var holder = node.GetNodeOrNull<ComponentHolder>(ComponentHolder.NodePath);
        if (holder is not null)
        {
            return holder.TryRemoveComponent<T>();
        }
        
        return false;
    }

    #endregion

    public static void StaicSetProcess(Node node, bool isActive)
        => node.SetProcess(isActive);

    public static void StaicSetPhysicsProcess(Node node, bool isActive)
        => node.SetPhysicsProcess(isActive);

    public static void StaicSetProcessInput(Node node, bool isActive)
        => node.SetProcessInput(isActive);

    public static void StaicSetProcessShortcutInput(Node node, bool isActive)
        => node.SetProcessShortcutInput(isActive);

    public static void StaicSetProcessUnhandledKeyInput(Node node, bool isActive)
        => node.SetProcessUnhandledKeyInput(isActive);

    public static void StaicSetProcessUnhandledInput(Node node, bool isActive)
        => node.SetProcessUnhandledInput(isActive);
}
