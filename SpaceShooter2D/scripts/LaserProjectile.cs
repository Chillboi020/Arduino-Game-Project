using Godot;

namespace SpaceShooter2D.scripts;

public partial class LaserProjectile : Area2D
{
	[Export] public float Damage { get; set; } = 1.0f;

	private SpeedComponent _speed;

	public override void _Ready()
	{
		_speed = GetNode<SpeedComponent>("SpeedComponent");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += new Vector2(0, -(_speed.Speed * (float)delta));
	}

	public void Die()
	{
		QueueFree();
	}

	private void OnVisibleOnScreenNotifier2dScreenExited()
	{
		Die();
	}
}
