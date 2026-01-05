using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAlgoT2530.Engine
{
    public static class Vector2Extensions
    {
        public static void Project(this Vector2 source, Vector2 target, out Vector2 parallelVector, out Vector2 perpendicularVector)
        {
            parallelVector = Vector2.Dot(source, target) / target.LengthSquared() * target;
            perpendicularVector = source - parallelVector;
        }

        public static void Scale(ref this Vector2 vector, float desiredMagnitude)
        {
            if (vector.LengthSquared() >= 0.001f)
            {
                vector.Normalize();
                vector *= desiredMagnitude;
            }
        }

        public static void Truncate(ref this Vector2 vector, float maxMagnitude)
        {
            float maxMagnitudeSquared = maxMagnitude * maxMagnitude;
            if (vector.LengthSquared() > maxMagnitudeSquared)
            {
                vector.Normalize();
                vector *= maxMagnitude;
            }
            else if (vector.LengthSquared() < 0.00001f)
            {
                vector = Vector2.Zero;
            }
        }
    }
}
