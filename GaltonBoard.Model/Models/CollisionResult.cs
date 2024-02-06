namespace GaltonBoard.Model.Models;

public class CollisionResult(bool isColliding, Vector normal, float penetration)
{
    public bool IsColliding { get; set; } = isColliding;
    public Vector Normal { get; set; } = normal;
    public float Penetration { get; set; } = penetration;
}