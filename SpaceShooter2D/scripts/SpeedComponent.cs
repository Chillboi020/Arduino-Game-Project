using Godot;

namespace SpaceShooter2D.scripts;

public partial class SpeedComponent : Node2D
{
	[Export] public float MaxSpeed { get; set; } = 1.0f;
	public float Speed;

	public override void _Ready()
	{
		Speed = MaxSpeed * 100;
	}
	
	public void AddSpeed(float amount)
	{
		Speed += (amount * 100);
	}
}
