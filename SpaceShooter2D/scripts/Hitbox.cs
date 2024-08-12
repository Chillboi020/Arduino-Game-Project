using Godot;

namespace SpaceShooter2D.scripts;

public partial class Hitbox : Area2D
{
	[Export] private HealthComponent Hp { get; set; }

	private void Damage(float amount)
	{
		Hp?.Damage(amount);
	}
}
