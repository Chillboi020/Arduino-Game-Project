using Godot;

namespace SpaceShooter2D.scripts;

public partial class Enemy : Entity
{
	#region Signals

	[Signal]
	public delegate void HitEventHandler();

	[Signal]
	public delegate void KilledEventHandler(int points);

	#endregion

	#region Export Variables

	[Export] private float Points { get; set; } = 1.0f;
	[Export] private float Penalty { get; set; } = -10.0f;
	[Export] private float Damage { get; set; } = 1.0f;

	#endregion

	#region Variables

	private HealthComponent _hp;
	private AnimatedSprite2D _animation;
	private SpeedComponent _speedComponent;

	protected bool IsHurt;
	protected float Speed;

	#endregion

	#region Override Methods

	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_hp = GetNode<HealthComponent>("HealthComponent");
		_speedComponent = GetNode<SpeedComponent>("SpeedComponent");
		Speed = _speedComponent.Speed;
	}

	#endregion

	protected void ProcessAnimation()
	{
		if (PlayAnimation)
		{
			_animation.Play("hurt");
			AnimationLength = 7.5f;
			PlayAnimation = false;
		}

		if (AnimationLength > 0)
		{
			AnimationLength -= 1;
		}
		else
		{
			_animation.Play("default");
			IsHurt = false;
		}
	}

	public void Die()
	{
		QueueFree();
	}

	public void TakeDamage(float amount)
	{
		_hp.Damage(amount);
		if (_hp.Health <= 0)
		{
			EmitSignal(nameof(Killed), Points);
		}
		else
		{
			EmitSignal(nameof(Hit));
			IsHurt = true;
			PlayAnimation = true;
		}
	}

	private void OnVisibleOnScreenNotifier2dScreenExited()
	{
		EmitSignal(nameof(Killed), Penalty);
		Die();
	}

	private void OnHitboxAreaEntered(Node area)
	{
		if (area.GetParent() is Player)
		{
			var playerC = (Player)area.GetParent();
			playerC.TakeDamage(Damage);
			Die();
		} else if (area is LaserProjectile projectile)
		{
			projectile.Die();
			TakeDamage(projectile.Damage);
		}
	}
}
