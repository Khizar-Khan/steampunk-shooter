using Godot;

namespace SteampunkShooter.utility;

public partial class MathUtil : RefCounted
{
    public static float ExponentialInterpolate(float current, float target, float factor, float delta)
    {
        return current + (target - current) * (1 - Mathf.Exp(-factor * delta));
    }
}