using System.Runtime.InteropServices.ComTypes;
using GaltonBoard.Core.Utils;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Logic;

public static class ParticleCollider
{
    public static CollisionResult? Check(Particle a, Particle b)
    {
        if (a.Config.IsStatic && b.Config.IsStatic) return null;
        if (a.Type == ParticleEnum.Peg && b.Type == ParticleEnum.Peg) return null;

        var sumOfRadius = a.Config.Radius + b.Config.Radius;
        var vectorDistance = a.Position - b.Position;
        var distance = vectorDistance.Magnitude();
        var normal = vectorDistance.Normalized();
        var isColliding = distance < sumOfRadius;
        if (!isColliding) return null;

        var penetration = sumOfRadius - distance;
        return new CollisionResult(isColliding, normal, (float)penetration);
    }

    public static void Resolve(CollisionResult collision, Particle a, Particle b)
    {
        ResolveUsingMomentum(collision, a, b);
    }

    private static void ResolveUsingMomentum(CollisionResult collision, Particle a, Particle b)
    {
        if (a.Type == ParticleEnum.Ball && b.Type == ParticleEnum.Peg)
        {
            ResolveBallPeg(collision, a, b);
        }
        else if (a.Type == ParticleEnum.Ball && b.Type == ParticleEnum.Ball)
        {
            ResolveBallBall(collision, a, b);
        }
    }

    private static void ResolveBallPeg(CollisionResult collision, Particle a, Particle b)
    {
        var ball = (Ball)a;
        var peg = (Peg)b;

        var dx = ball.Position.X - peg.Position.X;
        var dy = ball.Position.Y - peg.Position.Y;
        var impactAngle = Math.Atan2(dy, dx);

        var vxa = ball.Velocity.X;
        var vya = ball.Velocity.Y;
        var aplha0 = peg.Config.Restitution;

        var impactAngleCos = Math.Cos(impactAngle);
        var impactAngleSin = Math.Sin(impactAngle);

        var vTangen = -vxa * impactAngleSin + vya * impactAngleCos;
        var vRadial = peg.Position.Y > ball.Position.Y
            ? vya * impactAngleSin + vxa * impactAngleCos
            : -aplha0 * (vxa * impactAngleCos + vya * impactAngleSin);

        var vxNew = vRadial * impactAngleCos - vTangen * impactAngleSin;
        var vyNew = vRadial * impactAngleSin + vTangen * impactAngleCos;

        var sumRadius = ball.Config.Radius + peg.Config.Radius;
        var newX = (sumRadius) * impactAngleCos + peg.Position.X;
        var newY = (sumRadius) * impactAngleSin + peg.Position.Y;

        ball.Position = new Vector(newX, newY);
        ball.Velocity = new Vector(vxNew, vyNew);
    }

    private static void ResolveBallBall(CollisionResult collision, Particle a, Particle b)
    {
        var normal = collision.Normal;
        var tangent = new Vector(-normal.Y, normal.X);

        var v1N = Vector.Dot(normal, a.Velocity);
        var v1T = Vector.Dot(tangent, a.Velocity);

        var v2N = Vector.Dot(normal, b.Velocity);
        var v2T = Vector.Dot(tangent, b.Velocity);

        var mass1 = a.Config.Mass;
        var mass2 = b.Config.Mass;
        var totalMass = mass1 + mass2;

        // Conservation of momentum
        var v1NNew = (v1N * (mass1 - mass2) + 2 * mass2 * v2N) / totalMass;
        var v2NNew = (v2N * (mass2 - mass1) + 2 * mass1 * v1N) / totalMass;

        var v2NNewVect = normal * v2NNew;
        var v1NNewVect = normal * v1NNew;

        var v1TNewVect = tangent * v1T;
        var v2TNewVect = tangent * v2T;

        var v1New = (v1NNewVect + v1TNewVect) * b.Config.Restitution;
        var v2New = (v2NNewVect + v2TNewVect) * a.Config.Restitution;

        a.Velocity = v1New;
        b.Velocity = v2New;

        // Position correction
        var sumRadius = a.Config.Radius + b.Config.Radius;
        var massRatio1 = a.Config.Radius / sumRadius;
        var massRatio2 = b.Config.Radius / sumRadius;
        var delta = 0.5f * (collision.Penetration - sumRadius);

        var p1 = a.Position - normal * (massRatio2 * delta);
        var p2 = b.Position + normal * (massRatio1 * delta);

        a.Position = p1;
        b.Position = p2;
    }

    private static void ResolveUsingImpulse(CollisionResult collision, Particle a, Particle b)
    {
        var relativeVelocity = a.Velocity - b.Velocity;
        var velocityAlongNormal = Vector.Dot(relativeVelocity, collision.Normal);

        if (velocityAlongNormal > 0) return;

        var reducedMass = 1 / (a.Config.InverseMass + b.Config.InverseMass);
        var e = Math.Min(a.Config.Restitution, b.Config.Restitution);
        var j = -(1 + e) * velocityAlongNormal * reducedMass;

        var impulseVector = collision.Normal * j;

        if (!a.Config.IsStatic)
            a.Velocity += impulseVector * a.Config.InverseMass;
        if (!b.Config.IsStatic)
            b.Velocity -= impulseVector * b.Config.InverseMass;
    }
}