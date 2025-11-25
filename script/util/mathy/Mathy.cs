using System;
using System.Runtime.CompilerServices;
using Godot;


namespace LGWCP.Util.Mathy;

/*

Math lib dedicated for godot. Helper functions for vector, and many fastapprox algorithm.
"Kinda math, but not so math."

*/

public static class Mathy
{

#region FastApprox

    // Aligned with godot math
    public const float EPSILON = 1e-6f;
    public const float PI = Mathf.Pi;
    public const float PI_INV = 1f / PI;
    public const float PI_HALF = PI / 2f;
    public const float PI_TWO = PI * 2f;
    public const float PI_SQ = PI * PI;
    public const float PI_SQ_INV = 1f / PI_SQ;
    public const float PI_SQRT = 1.7724539f;
    public const float PI_SQRT_INV = 1f / PI_SQRT;

    public static void SpringDamp(
        ref float current,
        ref float currentVel,
        float target,
        float targetVel,
        float damping,
        float stiffSqrt,
        float stiffSqrtInv,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        float w = stiffSqrt * PI_TWO;
        float wInv = stiffSqrtInv * PI_INV * 0.5f;
        // Handle special cases
        if (w < EPSILON) // no strength which means no damping either
        {
            current += currentVel * deltaTime;
            return;
        }
        else if (damping < EPSILON) // No damping at all
        {
            float err = current - target;
            float b = currentVel * wInv;
            SinCos(out float S, out float C, w * deltaTime);
            current = target + err * C + b * S;
            currentVel = currentVel * C - err * (w * S);
            return;
        }

        // Target velocity turns into an offset to the position
        float smoothingTime = 2.0f * wInv;
        float targetAdj = target + targetVel * (damping * smoothingTime);
         float errAdj = current - targetAdj;

        // Handle the cases separately
        if (damping > 1.0f) // Overdamped
        {
            float wd = w * MathF.Sqrt(Square(damping) - 1.0f);
            float c2 = -(currentVel + (w * damping - wd) * errAdj) / (2.0f * wd);
            float c1 = errAdj - c2;
            float a1 = (wd - damping * w);
            float a2 = -(wd + damping * w);
            // Note that A1 and A2 will always be negative. We will use an approximation for 1/Exp(-A * DeltaTime).
            float a1Dt = a1 * deltaTime;
            float a2Dt = a2 * deltaTime;
            // This approximation in practice will be good for all DampingRatios
            float e1 = ExpApprox3(a1Dt);
            // As DampingRatio gets big, this approximation gets worse, but mere inaccuracy for overdamped motion is
            // not likely to be important, since we end up with 1 / BigNumber
            float e2 = ExpApprox3(a2Dt);
            current = targetAdj + e1 * c1 + e2 * c2;
            currentVel = e1 * c1 * a1 + e2 * c2 * a2;
        }
        else if (damping < 1.0f) // Underdamped
        {
            float wd = w * MathF.Sqrt(1.0f - Square(damping));
            float a = errAdj;
            float b = (currentVel + errAdj * (damping * w)) / wd;
            SinCos(out float s, out float c, wd * deltaTime);
            float e0 = damping * w * deltaTime;
            // Needs E0 < 1 so DeltaTime < SmoothingTime / (2 * DampingRatio * Sqrt(1 - DampingRatio^2))
            float e = ExpApprox3(-e0);
            current = e * (a * c + b * s);
            currentVel = -current * damping * w;
            currentVel += e * (b * (wd * c) - a * (wd * s));
            current += targetAdj;
        }
        else // Critical damping
        {
            float c1 = errAdj;
            float c2 = currentVel + errAdj * w;
            float e0 = w * deltaTime;
            // Needs E0 < 1 so deltaTime < SmoothingTime / 2 
            float e = ExpApprox3(-e0);
            current = targetAdj + (c1 + c2 * deltaTime) * e;
            currentVel = (c2 - c1 * w - c2 * (w * deltaTime)) * e;
        }
    }

    // -0.5~0.5 usable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExpApprox1(float x)
    { 
        return (6+x*(6+x*(3+x)))*0.16666666f; 
    }

    // -0.8~0.8 usable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExpApprox2(float x)
    { 
        return (24+x*(24+x*(12+x*(4+x))))*0.041666666f;
    }

    // -1.0~1.0 usable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExpApprox3(float x)
    { 
        return (120+x*(120+x*(60+x*(20+x*(5+x)))))*0.0083333333f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExpApprox4(float x)
    { 
        return 720+x*(720+x*(360+x*(120+x*(30+x*(6+x)))))*0.0013888888f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SinCos(out float s, out float c, float rad)
    {
        (s, c) = MathF.SinCos(rad);
    }

#endregion


#region Helper

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Square(float x)
    {
        return x*x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float v)
    {
        return MathF.Sqrt(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 SqrtPerAxis(Vector2 v)
    {
        return new Vector2(MathF.Sqrt(v.X), MathF.Sqrt(v.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 SqrtPerAxis(Vector3 v)
    {
        return new Vector3(MathF.Sqrt(v.X), MathF.Sqrt(v.Y), MathF.Sqrt(v.Z));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion SqrtPerAxis(Vector4 v)
    {
        return new Quaternion(MathF.Sqrt(v.X), MathF.Sqrt(v.Y), MathF.Sqrt(v.Z), MathF.Sqrt(v.W));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion SqrtPerAxis(Quaternion v)
    {
        return new Quaternion(MathF.Sqrt(v.X), MathF.Sqrt(v.Y), MathF.Sqrt(v.Z), MathF.Sqrt(v.W));
    }

    public static readonly Quaternion QuaternionZero = new Quaternion(0f, 0f, 0f, 0f);

    // Modify local rotation rad to (-pi, pi), during continuous calculation
    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RotModifierRad(ref float r)
    {
        r += (r > PI-EPSILON) ? -PI_TWO+2*EPSILON : (r < -PI+EPSILON) ? PI_TWO-2*EPSILON : 0f;
    }
    */

    // Minimal degree delta between 2 angle (-180~180)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DeltaDegree(float current, float target)
    {
        float delta = target - current;
        delta += (delta > 180f) ? -360f : (delta < -180f) ? 360f : 0f;
        return delta;
    }

    // Minimal rad delta between 2 angle (-pi~pi)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DeltaRad(float current, float target)
    {
        /*
        _ALWAYS_INLINE_ double angle_difference(double p_from, double p_to) {
            double difference = fmod(p_to - p_from, Math_TAU);
            return fmod(2.0 * difference, Math_TAU) - difference;
        }
        _ALWAYS_INLINE_ float angle_difference(float p_from, float p_to) {
            float difference = fmod(p_to - p_from, (float)Math_TAU);
            return fmod(2.0f * difference, (float)Math_TAU) - difference;
        }
        */
        float diff = (target - current) % MathF.Tau;
        return (2f * diff) % MathF.Tau - diff;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 DeltaRad(Vector2 current, Vector2 target)
    {
        float diffX = (target.X - current.X) % MathF.Tau;
        float diffY = (target.Y - current.Y) % MathF.Tau;
        Vector2 delta = new(
            (2f * diffX) % MathF.Tau - diffX,
            (2f * diffY) % MathF.Tau - diffY
        );
        return delta;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 DeltaRad(Vector3 current, Vector3 target)
    {
        float diffX = (target.X - current.X) % MathF.Tau;
        float diffY = (target.Y - current.Y) % MathF.Tau;
        float diffZ = (target.Z - current.Z) % MathF.Tau;
        Vector3 delta = new(
            (2f * diffX) % MathF.Tau - diffX,
            (2f * diffY) % MathF.Tau - diffY,
            (2f * diffZ) % MathF.Tau - diffZ
        );
        return delta;
    }

#endregion


#region Control

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetAccelOrDecel(float from, float to, float accel, float decel)
    {
        bool isAccel;

        if (from == 0.0f)
        {
            // from stillness (from == 0.0f), use accel
            isAccel = true;
        }
        else
        {
            // to stillness (to == 0.0f), use decel
            isAccel = from * to > 0.0f
                && MathF.Abs(from) < MathF.Abs(to);
        }
        
        return isAccel ? accel : decel;
    }

#endregion

}
