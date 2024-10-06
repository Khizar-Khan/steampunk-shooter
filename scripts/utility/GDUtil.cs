using Godot;
using Godot.Collections;

namespace SteampunkShooter.utility;

public partial class GDUtil : RefCounted
{
    public static Timer CreateTimer(Node parent, float waitTime, string timeoutMethodName = null)
    {
        Timer timer = new Timer
        {
            WaitTime = waitTime,
            OneShot = true
        };

        if (!string.IsNullOrEmpty(timeoutMethodName))
            timer.Connect("timeout", new Callable(parent, timeoutMethodName));

        parent.AddChild(timer);
        return timer;
    }

    public static Dictionary PerformHitScanFromScreenCenter(Node3D nodeInSceneTree, float range)
    {
        Viewport viewport = nodeInSceneTree.GetViewport();
        if (viewport == null)
        {
            GD.PrintErr("Viewport not found.");
            return new Dictionary();
        }
        
        Camera3D camera = viewport.GetCamera3D();
        if (camera == null)
        {
            GD.PrintErr("No active camera found.");
            return new Dictionary();
        }

        Vector2 screenCenter = viewport.GetVisibleRect().Size / 2;
        Vector3 rayOrigin = camera.ProjectRayOrigin(screenCenter);
        Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(screenCenter) * range;

        return nodeInSceneTree.GetWorld3D().DirectSpaceState.IntersectRay(
            new PhysicsRayQueryParameters3D
            {
                From = rayOrigin,
                To = rayEnd
            }
        );
    }
}