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
        Ball ball = (Ball)a;
        Peg peg = (Peg)b;

        var dx = ball.Position.X - peg.Position.X;
        var dy = ball.Position.Y - peg.Position.Y;
        var impact_angle = Math.Atan2(dy, dx);

        var vxa = ball.Velocity.X;
        var vya = ball.Velocity.Y;
        var aplha_0 = peg.Config.Restitution;

        var impact_angle_cos = Math.Cos(impact_angle);
        var impact_angle_sin = Math.Sin(impact_angle);

        var v_tangen = -vxa * impact_angle_sin + vya * impact_angle_cos;
        var v_radial = -aplha_0 * (vxa * impact_angle_cos + vya * impact_angle_sin);

        var vx_new = v_radial * impact_angle_cos - v_tangen * impact_angle_sin;
        var vy_new = v_radial * impact_angle_sin + v_tangen * impact_angle_cos;

        var sum_radius = ball.Config.Radius + peg.Config.Radius;
        var new_x = (sum_radius) * impact_angle_cos + peg.Position.X;
        var new_y = (sum_radius) * impact_angle_sin + peg.Position.Y;

        ball.Position = new Vector(new_x, new_y);
        ball.Velocity = new Vector(vx_new, vy_new);
    }

    private static void ResolveBallBall(CollisionResult collision, Particle a, Particle b)
    {
        var normal = collision.Normal;
        var tangent = new Vector(-normal.Y, normal.X);

        var v1n = Vector.Dot(normal, a.Velocity);
        var v1t = Vector.Dot(tangent, a.Velocity);

        var v2n = Vector.Dot(normal, b.Velocity);
        var v2t = Vector.Dot(tangent, b.Velocity);

        var mass_1 = a.Config.Mass;
        var mass_2 = b.Config.Mass;
        var total_mass = mass_1 + mass_2;

        // Conservation of momentum
        var v1n_new = (v1n * (mass_1 - mass_2) + 2 * mass_2 * v2n) / total_mass;
        var v2n_new = (v2n * (mass_2 - mass_1) + 2 * mass_1 * v1n) / total_mass;

        var v2n_new_vect = normal * v2n_new;
        var v1n_new_vect = normal * v1n_new;

        var v1t_new_vect = tangent * v1t;
        var v2t_new_vect = tangent * v2t;

        var v1_new = (v1n_new_vect + v1t_new_vect) * b.Config.Restitution;
        var v2_new = (v2n_new_vect + v2t_new_vect) * a.Config.Restitution;

        a.Velocity = v1_new;
        b.Velocity = v2_new;

        // Position correction
        var sum_radius = a.Config.Radius + b.Config.Radius;
        var mass_ratio_1 = a.Config.Radius / sum_radius;
        var mass_ratio_2 = b.Config.Radius / sum_radius;
        var delta = 0.5f * (collision.Penetration - sum_radius);

        var p1 = a.Position - normal * (mass_ratio_2 * delta);
        var p2 = b.Position + normal * (mass_ratio_1 * delta);

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