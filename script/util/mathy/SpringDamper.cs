using System;
using Godot;


namespace LGWCP.Util.Mathy;


/*
SpringDamp helper classes, minimize calculation cost
*/

public class SpringDamper
{
    protected class DampStepper
    {
        protected float Damping;
        protected float StiffSqrt;
        protected float StiffSqrtInv;
        protected float W;
        protected float WInv;

        public DampStepper(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
        {
            Damping = damping;
            StiffSqrt = stiffSqrt;
            StiffSqrtInv = stiffSqrtInv;
            W = w;
            WInv = wInv;
        }

        public virtual void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime) {}
    }

    protected class DampStepperSmallW : DampStepper
    {
        public DampStepperSmallW(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
            : base(damping, stiffSqrt, stiffSqrtInv, w, wInv) {}
        public override void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime)
        {
            current += currentVel * deltaTime;
            return;
        }
    }

    protected class DampStepperSmallDamping : DampStepper
    {
        public DampStepperSmallDamping(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
            : base(damping, stiffSqrt, stiffSqrtInv, w, wInv) {}
        public override void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime)
        {
            var err = current - target;
            var b = currentVel * WInv;
            Mathy.SinCos(out float s, out float c, W * deltaTime);
            current = target + err * c + b * s;
            currentVel = currentVel * c - err * (W * s);
            return;
        }
    }

    protected class DampStepperOverDamp : DampStepper
    {
        protected float WD;
        protected float WDInv;
        protected float SmoothingTime;
        protected float A1;
        protected float A2;
        protected float ErrRatio;

        public DampStepperOverDamp(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
            : base(damping, stiffSqrt, stiffSqrtInv, w, wInv)
        {
            WD = W * MathF.Sqrt(Mathy.Square(Damping) - 1f);
            WDInv = 1f / WD;
            SmoothingTime = 2.0f * WInv;
            A1 = WD - Damping * W;
            A2 = -(WD + Damping * W);
            ErrRatio = W * Damping - WD;
        }

        public override void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime)
        {
            // var SmoothingTime = 2.0f * WInv;
            float targetAdj = target + targetVel * (Damping * SmoothingTime);
            float err = current - targetAdj;

            // float WD = W * MathF.Sqrt(Mathy.Square(Damping) - 1.0f);
            // float c2 = -(currentVel + (W * Damping - WD) * Err) / (2.0f * WD);
            // float c2 = -(currentVel + (W * Damping - WD) * err) * WDInv * 0.5f;
            float c2 = -(currentVel + ErrRatio * err) * WDInv * 0.5f;
            float c1 = err - c2;
            // float A1 = (WD - Damping * W);
            // float A2 = -(WD + Damping * W);
            float a1Dt = A1 * deltaTime;
            float a2Dt = A2 * deltaTime;
            float e1 = Mathy.ExpApprox3(a1Dt);
            float e2 = Mathy.ExpApprox3(a2Dt);
            current = targetAdj + e1 * c1 + e2 * c2;
            currentVel = e1 * c1 * A1 + e2 * c2 * A2;
        }
    }
    
    protected class DampStepperUnderDamp : DampStepper
    {
        protected float WD;
        protected float WDInv;
        protected float SmoothingTime;
        protected float DampingW;

        public DampStepperUnderDamp(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
            : base(damping, stiffSqrt, stiffSqrtInv, w, wInv)
        {
            WD = W * MathF.Sqrt(1f - Mathy.Square(Damping));
            WDInv = 1f / WD;
            SmoothingTime = 2.0f * WInv;
            DampingW = Damping * W;
        }

        public override void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime)
        {
            // var SmoothingTime = 2.0f * WInv;
            float targetAdj = target + targetVel * (Damping * SmoothingTime);
            float errAdj = current - targetAdj;

            // float WD = W * MathF.Sqrt(1.0f - Mathy.Square(Damping));
            float a = errAdj;
            // float b = (currentVel + errAdj * (Damping * W)) / WD;
            float b = (currentVel + errAdj * Damping * W) / WD;
            Mathy.SinCos(out float s, out float c, WD * deltaTime);
            // float e0 = Damping * W * deltaTime;
            // float e = Mathy.ExpApprox3(-e0);
            float e = Mathy.ExpApprox3(-Damping * W * deltaTime);
            current = e * (a * c + b * s);
            currentVel = -current * DampingW;
            currentVel += e * (b * (WD * c) - a * (WD * s));
            current += targetAdj;
        }
    }

    protected class DampStepperCriticalDamp : DampStepper
    {
        protected float SmoothingTime;
        protected float DST;

        public DampStepperCriticalDamp(float damping, float stiffSqrt, float stiffSqrtInv, float w, float wInv)
            : base(damping, stiffSqrt, stiffSqrtInv, w, wInv)
        {
            SmoothingTime = 2.0f * WInv;
            DST = Damping * SmoothingTime;
        }

        public override void Step(
            ref float current,
            ref float currentVel,
            float target,
            float targetVel,
            float deltaTime)
        {
            // var SmoothingTime = 2.0f * WInv;
            // float targetAdj = target + targetVel * (Damping * SmoothingTime);
            float targetAdj = target + targetVel * DST;
            float err = current - targetAdj;

            float c1 = err;
            float c2 = currentVel + err * W;
            // float e0 = W * deltaTime;
            // float e = Mathy.ExpApprox3(-e0);
            float e = Mathy.ExpApprox3(-W * deltaTime);
            current = targetAdj + (c1 + c2 * deltaTime) * e;
            currentVel = (c2 - c1 * W - c2 * (W * deltaTime)) * e;
        }
    }

    // public virtual void Step(ref float current, ref float currentVel, float target, float targetVel, float deltaTime) {}
}

public class SpringDamperF : SpringDamper
{
    protected DampStepper Stepper;
    public SpringDamperF(float damping, float stiff)
    {
        var stiffSqrt = MathF.Sqrt(stiff);
        var stiffSqrtInv = 1f / stiffSqrt;
        var w = stiffSqrt * Mathy.PI_TWO;
        var wInv = stiffSqrtInv * Mathy.PI_INV * 0.5f;

        if (w < Mathy.EPSILON)
        {
            Stepper = new DampStepperSmallW(damping, stiffSqrt, stiffSqrtInv, w, wInv);
        }
        else if (damping < Mathy.EPSILON)
        {
            Stepper = new DampStepperSmallDamping(damping, stiffSqrt, stiffSqrtInv, w, wInv);
        }
        else if (damping > 1.0f)
        {
            Stepper = new DampStepperOverDamp(damping, stiffSqrt, stiffSqrtInv, w, wInv);
        }
        else if (damping < 1.0f)
        {
            Stepper = new DampStepperUnderDamp(damping, stiffSqrt, stiffSqrtInv, w, wInv);
        }
        else // damping == 1.0f
        {
            Stepper = new DampStepperCriticalDamp(damping, stiffSqrt, stiffSqrtInv, w, wInv);
        }
    }
    
    public void Step(
        ref float current,
        ref float currentVel,
        float target,
        float targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        Stepper.Step(ref current, ref currentVel, target, targetVel, deltaTime);
    }

    public void StepRad(
        ref float current,
        ref float currentVel,
        float target,
        float targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        target = current + Mathy.DeltaRad(current, target);
        Stepper.Step(ref current, ref currentVel, target, targetVel, deltaTime);
    }
}

public class SpringDamperV2 : SpringDamper
{
    protected DampStepper[] Steppers = new DampStepper[2];
    public SpringDamperV2(in Vector2 damping, in Vector2 stiff)
    {
        var stiffSqrt = Mathy.SqrtPerAxis(stiff);
        var stiffSqrtInv = Vector2.One / stiff;
        var w = stiffSqrt * Mathy.PI_TWO;
        var wInv = stiffSqrtInv * Mathy.PI_INV * 0.5f;

        for (int i = 0; i < Steppers.Length; ++i)
        {
            if (w[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallW(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallDamping(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] > 1.0f)
            {
                Steppers[i] = new DampStepperOverDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < 1.0f)
            {
                Steppers[i] = new DampStepperUnderDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else // damping == 1.0f
            {
                Steppers[i] = new DampStepperCriticalDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
        }
    }

    public void Step(
        ref Vector2 current,
        ref Vector2 currentVel,
        Vector2 target,
        Vector2 targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        Steppers[0].Step(ref current.X, ref currentVel.X, target.X, targetVel.X, deltaTime);
        Steppers[1].Step(ref current.Y, ref currentVel.Y, target.Y, targetVel.Y, deltaTime);
    }

    public void StepRad(
        ref Vector2 current,
        ref Vector2 currentVel,
        Vector2 target,
        Vector2 targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        target = current + Mathy.DeltaRad(current, target);
        Steppers[0].Step(ref current.X, ref currentVel.X, target.X, targetVel.X, deltaTime);
        Steppers[1].Step(ref current.Y, ref currentVel.Y, target.Y, targetVel.Y, deltaTime);
    }
}

public class SpringDamperV3 : SpringDamper
{
    protected DampStepper[] Steppers = new DampStepper[3];
    public SpringDamperV3(in Vector3 damping, in Vector3 stiff)
    {
        var stiffSqrt = Mathy.SqrtPerAxis(stiff);
        var stiffSqrtInv = Vector3.One / stiff;
        var w = stiffSqrt * Mathy.PI_TWO;
        var wInv = stiffSqrtInv * Mathy.PI_INV * 0.5f;

        for (int i = 0; i < Steppers.Length; ++i)
        {
            if (w[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallW(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallDamping(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] > 1.0f)
            {
                Steppers[i] = new DampStepperOverDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < 1.0f)
            {
                Steppers[i] = new DampStepperUnderDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else // damping == 1.0f
            {
                Steppers[i] = new DampStepperCriticalDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
        }
    }

    public void Step(
        ref Vector3 current,
        ref Vector3 currentVel,
        Vector3 target,
        Vector3 targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        Steppers[0].Step(ref current.X, ref currentVel.X, target.X, targetVel.X, deltaTime);
        Steppers[1].Step(ref current.Y, ref currentVel.Y, target.Y, targetVel.Y, deltaTime);
        Steppers[2].Step(ref current.Z, ref currentVel.Z, target.Z, targetVel.Z, deltaTime);
    }

    public void StepRad(
        ref Vector3 current,
        ref Vector3 currentVel,
        Vector3 target,
        Vector3 targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        target = current + Mathy.DeltaRad(current, target);
        Steppers[0].Step(ref current.X, ref currentVel.X, target.X, targetVel.X, deltaTime);
        Steppers[1].Step(ref current.Y, ref currentVel.Y, target.Y, targetVel.Y, deltaTime);
        Steppers[2].Step(ref current.Z, ref currentVel.Z, target.Z, targetVel.Z, deltaTime);
    }
}


/*
/// <summary>
/// Rotation repeat not implemented.
/// </summary>
public class SpringDamperQuat : SpringDamper
{
    protected DampStepper[] Steppers = new DampStepper[4];
    public SpringDamperQuat(Quaternion damping, Vector4 stiff)
    {
        var stiffSqrt = Mathy.SqrtPerAxis(stiff);
        var stiffSqrtInv = new Quaternion(1f / stiff.X, 1f / stiff.Y, 1f / stiff.Z, 1f / stiff.W);
        var w = stiffSqrt * Mathy.PI_TWO;
        var wInv = stiffSqrtInv * Mathy.PI_INV * 0.5f;

        for (int i = 0; i < Steppers.Length; ++i)
        {
            if (w[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallW(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < Mathy.EPSILON)
            {
                Steppers[i] = new DampStepperSmallDamping(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] > 1.0f)
            {
                Steppers[i] = new DampStepperOverDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else if (damping[i] < 1.0f)
            {
                Steppers[i] = new DampStepperUnderDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
            else // damping == 1.0f
            {
                Steppers[i] = new DampStepperCriticalDamp(damping[i], stiffSqrt[i], stiffSqrtInv[i], w[i], wInv[i]);
            }
        }
    }

    public void Step(
        ref Quaternion current,
        ref Quaternion currentVel,
        Quaternion target,
        Quaternion targetVel,
        float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        Steppers[0].Step(ref current.X, ref currentVel.X, target.X, targetVel.X, deltaTime);
        Steppers[1].Step(ref current.Y, ref currentVel.Y, target.Y, targetVel.Y, deltaTime);
        Steppers[2].Step(ref current.Z, ref currentVel.Z, target.Z, targetVel.Z, deltaTime);
        Steppers[3].Step(ref current.W, ref currentVel.W, target.W, targetVel.W, deltaTime);
    }
}
*/
