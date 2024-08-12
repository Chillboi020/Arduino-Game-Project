using System;
using SpaceShooter2D.scripts.Tools;
using Godot;

namespace SpaceShooter2D.scripts
{
	public partial class Player : Entity
	{
		#region Signals

		[Signal]
		public delegate void HitEventHandler(float amount);

		[Signal]
		public delegate void KilledEventHandler();

		[Signal]
		public delegate void LaserShotEventHandler(PackedScene laserScene, Vector2 location);

		#endregion

		#region Exported Variables

		[Export] private float Acceleration { get; set; } = 5.0f;
		[Export] private float Friction { get; set; } = 2.0f;
		[Export] private float FireRate { get; set; } = 0.25f;
		[Export] private float InvulnerabilityDuration { get; set; } = 0.75f;

		#endregion

		#region Variables
		
		private Marker2D _muzzle;
		private AnimatedSprite2D _animation;
		public HealthComponent Hp;
		private SpeedComponent _speed;

		private Vector2 _direction = Vector2.Zero;
		private Variant _laserScene;
		private bool _shootCooldown;
		private bool _isHurt;

		#endregion

		#region Overridden Methods

		public override void _Ready()
		{
			_muzzle = GetNode<Marker2D>("Muzzle");
			_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			Hp = GetNode<HealthComponent>("HealthComponent");
			_speed = GetNode<SpeedComponent>("SpeedComponent");
			_laserScene = GD.Load<PackedScene>("res://scenes/laser_projectile.tscn");
		}

		public override void _PhysicsProcess(double delta)
		{
			PlayerMovement(delta);
			GlobalPosition = GlobalPosition.Clamp(new Vector2(0, 30), GetViewportRect().Size);

			if (_isHurt)
			{
				ProcessAnimation();
			}
		}

		public override void _Process(double delta)
		{
			if (!Input.IsActionPressed("player_shoot")) return;
			if (_shootCooldown) return;
			_shootCooldown = true;
			EmitSignal(nameof(LaserShot), _laserScene, _muzzle.GlobalPosition);
			ToSignal(GetTree().CreateTimer(FireRate), "timeout").OnCompleted(() => _shootCooldown = false);
		}

		#endregion

		#region Private Methods

		private void ProcessAnimation()
		{
			if (PlayAnimation)
			{
				_animation.Play("hurt");
				AnimationLength = InvulnerabilityDuration * 100;
				PlayAnimation = false;
			}

			if (AnimationLength > 0)
			{
				AnimationLength -= 1;
				_animation.SpeedScale += 0.05f;
			}
			else
			{
				_animation.Play("default");
				_animation.SpeedScale = 1;
				_isHurt = false;
			}
		}

		private void PlayerMovement(double delta)
		{
			var direction = GetDirection(Input.GetAxis("player_move_left", "player_move_right"), 
										 Input.GetAxis("player_move_up", "player_move_down"));

			if (direction == Vector2.Zero)
			{
				var decceleration = (Friction * 1000) * (float)delta;
				if (Velocity.Length() > decceleration) Velocity -= Velocity.Normalized() * decceleration;
				else Velocity = Vector2.Zero;
			}
			else
			{
				Velocity += (direction * (Acceleration * 1000)) * (float)delta;
				Velocity = Velocity.LimitLength(_speed.Speed);
			}
			MoveAndSlide();
		}

		private static Vector2 GetDirection(float x, float y)
		{
			return new Vector2(x, y).Normalized();
		}

		public void Die()
		{
			EmitSignal(nameof(Killed));
			QueueFree();
		}

		public void TakeDamage(float amount)
		{
			if (_isHurt) return;
			EmitSignal(nameof(Hit), amount);
			Hp.Damage(amount);
			_isHurt = true;
			PlayAnimation = true;
		}

		#endregion
	}
}
