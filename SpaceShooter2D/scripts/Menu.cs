using Godot;

namespace SpaceShooter2D.scripts;

public partial class Menu : Control
{
	private Button _playButton;
	private Button _quitButton;
	private MarginContainer _marginContainer;
	private ParallaxBackground _pb;
	private PackedScene _startGameScene;
	
	[Export] private int ScrollSpeed { get; set; } = 100;

	public override void _Ready()
	{
		_playButton = GetNode<Button>("MarginContainer/HBoxContainer/VBoxContainer/Play_Button");
		_quitButton = GetNode<Button>("MarginContainer/HBoxContainer/VBoxContainer/Quit_Button");
		_marginContainer = GetNode<MarginContainer>("MarginContainer");
		_pb = GetNode<ParallaxBackground>("ParallaxBackground");
		_startGameScene = GD.Load<PackedScene>("res://scenes/game/game.tscn");
		HandleConnectingSignals();
		var parallaxSprite = (Sprite2D)_pb.GetChild(0).GetChild(0);
		parallaxSprite.RegionRect = new Rect2(0, 0, GetViewportRect().Size.X, GetViewportRect().Size.Y);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("main_escape"))
		{
			OnQuitPressed();
		}
		
		_pb.ScrollOffset += new Vector2(ScrollSpeed * (float)delta, _pb.ScrollOffset.Y);
		if(_pb.ScrollOffset.X >= GetViewportRect().Size.X)
		{
			_pb.ScrollOffset = new Vector2(0, _pb.ScrollOffset.Y);
		}
	}

	private void HandleConnectingSignals()
	{
		_playButton.ButtonDown += OnPlayPressed;
		_quitButton.ButtonDown += OnQuitPressed;
	}
	
	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToPacked(_startGameScene);
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
