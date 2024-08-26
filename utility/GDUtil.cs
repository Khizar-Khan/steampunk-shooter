using Godot;

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
}