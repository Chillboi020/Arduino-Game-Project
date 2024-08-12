using Godot;

namespace SpaceShooter2D.scripts;

public partial class Stormer : Enemy
{
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += new Vector2(0, Speed * (float)delta);

		if (IsHurt)
		{
			ProcessAnimation();
		}
	}
}
