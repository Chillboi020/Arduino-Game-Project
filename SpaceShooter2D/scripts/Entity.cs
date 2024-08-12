using Godot;

namespace SpaceShooter2D.scripts;

public partial class Entity : CharacterBody2D
{
	protected bool PlayAnimation { get; set; }
	protected float AnimationLength { get; set; }
}
