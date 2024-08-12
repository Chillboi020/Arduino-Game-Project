using System;
using Godot;

namespace SpaceShooter2D.scripts;

public partial class HealthComponent : Node2D
{
	[Export] private float MaxHealth { get; set; } = 1.0f;
	public float Health;

	public override void _Ready()
	{
		Health = MaxHealth;
	}

	public void Damage(float amount)
	{
		Health -= amount;

		if (Health <= 0)
		{
			if (GetParent() is Player playerC)
			{
				playerC.Die();
			}
			else if (GetParent() is Enemy enemyC)
			{
				enemyC.Die();
			}
		}
	}

	public void AddHealth(float amount)
	{
		Health += amount;
	}
}
