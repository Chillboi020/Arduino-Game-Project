using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Godot;
using SpaceShooter2D.scripts.Tools;
using FileAccess = Godot.FileAccess;

namespace SpaceShooter2D.scripts;

public partial class Game : Node2D
{
	[Export] private PackedScene[] EnemyScenes { get; set; }
	[Export] private float PlayerPointsNeededForBonusHp { get; set; } = 4000.0f;

	private Node2D _laserContainer;
	private Node2D _enemyContainer;

	private Timer _enemySpawnTimer;
	private Hud _hud;
	private GameOverScreen _gos;
	private ParallaxBackground _pb;

	private AudioStreamPlayer _laserSound;
	private AudioStreamPlayer _explosionSound;
	private AudioStreamPlayer _hitSound;

	private SerialCommunicator _controller;
	private Player _player;
	private Vector2 _screenSize;
	private int _highScore;
	private float _scrollSpeed = 100.0f;
	private bool _isPlayerDead;
	private float _playerPoints;

	private int _score;

	private int Score
	{
		get => _score;
		set
		{
			_score = value;
			_hud.Score.Text = _score.ToString(CultureInfo.InvariantCulture);
		}
	}

	private float _health;

	private float Health
	{
		get => _health;
		set
		{
			_health = value;
			_hud.Health.Text = _health.ToString(CultureInfo.InvariantCulture);
		}
	}

	private float _bonusProgressBarValue;

	private float BonusProgressBarValue
	{
		get => _bonusProgressBarValue;
		set
		{
			_bonusProgressBarValue = value;
			_hud.BonusProgressBar.Value = _bonusProgressBarValue;
		}
	}

	public override void _Ready()
	{
		// Load the Highscore
		// Structure: 0
		var saveFile = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Read);
		if (saveFile != null)
		{
			_highScore = int.Parse(saveFile.GetLine().Trim());
			saveFile.Close();
		}
		else
		{
			_highScore = 0;
			SaveGame();
		}

		// Get the Screen Size
		_screenSize = GetViewportRect().Size;

		// Create the Player from a NodeGroup
		_player = (Player)GetTree().GetFirstNodeInGroup("player");

		// Make his spawn position in the bottom Center of the screen
		_player.GlobalPosition = new Vector2(_screenSize.X / 2, _screenSize.Y - 50);

		// Connect the Signals
		_player.LaserShot += OnLaserShot;
		_player.Killed += OnPlayerKilled;
		_player.Hit += OnPlayerHit;

		_enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");
		_gos = GetNode<GameOverScreen>("UiLayer/GameOverScreen");
		_enemyContainer = GetNode<Node2D>("EnemyContainer");
		_laserContainer = GetNode<Node2D>("LaserContainer");
		_hitSound = GetNode<AudioStreamPlayer>("SFX/HitSound");
		_laserSound = GetNode<AudioStreamPlayer>("SFX/LaserSound");
		_explosionSound = GetNode<AudioStreamPlayer>("SFX/ExplodeSound");

		// Set the ParallaxBackground to match the Screen Size
		_pb = GetNode<ParallaxBackground>("ParallaxBackground");
		var parallaxSprite = (Sprite2D)_pb.GetChild(0).GetChild(0);
		parallaxSprite.RegionRect = new Rect2(0, 0, _screenSize.X, _screenSize.Y);

		// Score and Health get displayed
		_hud = GetNode<Hud>("UiLayer/HUD");
		Score = 0;
		Health = _player.Hp.Health;
		BonusProgressBarValue = 0.0f;

		// Connect the SerialCommunicator
		_controller = GetNode<SerialCommunicator>("Controller");
		if (_controller == null) return;
		_controller.DataReceived += OnDataReceived;
		ConnectArduino();
	}

	public override void _Process(double delta)
	{
		if (!_isPlayerDead)
		{
			switch (_enemySpawnTimer.WaitTime)
			{
				case > 0.5:
					_enemySpawnTimer.WaitTime -= delta * 0.005f;
					break;
				case < 0.5:
					_enemySpawnTimer.WaitTime = 0.5f;
					break;
			}
		}
		else
		{
			if (BonusProgressBarValue > 0)
			{
				BonusProgressBarValue -= 0.05f;
			}
		}

		// The Background scrolling
		_pb.ScrollOffset += new Vector2(_pb.ScrollOffset.X, _scrollSpeed * (float)delta);
		// To enhance performance, we reset the offset to 0 if it's out of bounds
		if (_pb.ScrollOffset.Y >= _screenSize.Y)
		{
			_pb.ScrollOffset = new Vector2(_pb.ScrollOffset.X, 0);
		}
	}

	private void ConnectArduino()
	{
		if (!_controller.IsConnected)
		{
			GD.Print("Arduino controller not connected.");
		}
		else
		{
			GD.Print("Arduino controller connected.");
		}
	}

	private void SaveGame()
	{
		var saveFile = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Write);
		saveFile.StoreLine(_highScore.ToString(CultureInfo.InvariantCulture));
		saveFile.Close();
	}

	private void OnLaserShot(PackedScene laserscene, Vector2 location)
	{
		var laser = laserscene.Instantiate<LaserProjectile>();
		laser.GlobalPosition = location;
		_laserContainer.AddChild(laser);
		_laserSound.Play();
	}

	private void OnEnemySpawnTimerTimeout()
	{
		// When the Timer times out, we spawn a random enemy
		var e = EnemyScenes[GD.RandRange(0, EnemyScenes.Length - 1)].Instantiate<Enemy>();

		e.GlobalPosition = new Vector2((float)GD.RandRange(50, _screenSize.X - 50), -50);
		e.Killed += OnEnemyKilled;
		e.Hit += OnEnemyHit;
		_enemyContainer.AddChild(e);
	}

	private void OnEnemyHit()
	{
		_hitSound.Play();
	}

	private void OnEnemyKilled(int points)
	{
		if (Score + points < 0)
		{
			Score = 0;
		}
		else
		{
			if (_isPlayerDead) return;
			if (points > 0)
			{
				_hitSound.Play();
			}

			Score += points;
			if (_playerPoints + points < 0)
			{
				_playerPoints = 0;
			}
			else
			{
				_playerPoints += points;
			}

			UpdateBonus();
			if (Score > _highScore)
			{
				_highScore = Score;
			}
		}
	}

	private void OnPlayerHit(float amount)
	{
		_hitSound.Play();
		Health -= amount;
	}

	private void OnPlayerKilled()
	{
		_isPlayerDead = true;
		Health = 0;
		_explosionSound.Play();
		GameOver();
	}

	private async void GameOver()
	{
		_gos.SetScore(Score);
		_gos.SetHighScore(_highScore);
		SaveGame();
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		_gos.Visible = true;
		_enemySpawnTimer.WaitTime = 0.05f;
	}

	private void UpdateBonus()
	{
		if (_playerPoints >= PlayerPointsNeededForBonusHp)
		{
			_player.Hp.AddHealth(1);
			Health = _player.Hp.Health;
			_playerPoints -= PlayerPointsNeededForBonusHp;
			PlayerPointsNeededForBonusHp *= 2;
		}

		BonusProgressBarValue = (_playerPoints / PlayerPointsNeededForBonusHp) * 100;
	}

	private void OnDataReceived(string data)
	{
		GD.Print("Received data: " + data);
		var parts = data.Split(':');
		if (parts.Length != 3) return;
		_ = int.TryParse(parts[0], out var x);
		_ = int.TryParse(parts[1], out var y);
		_ = int.TryParse(parts[2], out var button);

		SimulateInput(x, y, button);
	}

	private static void SimulateInput(int x, int y, int b)
	{
		// Handle x-axis input
		if (x == 1)
		{
			Input.ActionPress("player_move_right");
		}
		else if (x == -1)
		{
			Input.ActionPress("player_move_left");
		}
		else
		{
			Input.ActionRelease("player_move_right");
			Input.ActionRelease("player_move_left");
		}

		// Handle y-axis input
		if (y == 1)
		{
			Input.ActionPress("player_move_up");
		}
		else if (y == -1)
		{
			Input.ActionPress("player_move_down");
		}
		else
		{
			Input.ActionRelease("player_move_up");
			Input.ActionRelease("player_move_down");
		}

		// Handle button press
		if (b == 1)
		{
			Input.ActionPress("player_shoot");
		}
		else
		{
			Input.ActionRelease("player_shoot");
		}
	}
}
