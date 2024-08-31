using System;
using Godot;

namespace SteampunkShooter.utility;

public partial class MathUtil : RefCounted
{
    public static float ExponentialInterpolate(float current, float target, float factor, float delta)
    {
        return current + (target - current) * (1 - Mathf.Exp(-factor * delta));
    }
    
    public static double CalculateMaximumJumpHorizontalDistance(double gravityJumping, double gravityFalling, double horizontalVelocity, double verticalHeight)
    {
        return horizontalVelocity * (Math.Sqrt(2 * verticalHeight / Math.Abs(gravityJumping)) + Math.Sqrt(2 * verticalHeight / Math.Abs(gravityFalling)));;
    }
}