using System;
using Godot;
using LGWCP.NiceGD;

namespace LGWCP.Pond;

[GlobalClass]
[Tool]
public partial class DragEndOnFocusExited : ComponentResource
{
    public override Type ComponentType => typeof(DragEndOnFocusExited);
    public override TickGroupEnum TickGroup => TickGroupEnum.None;
    public override bool IsRegist => false;
    
    public delegate void DragEndHandler();
    public event DragEndHandler DragEnd;

    public void OnFocusExited()
    {
        DragEnd.Invoke();
    }
}