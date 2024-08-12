using Godot;

namespace SpaceShooter2D.scripts;

public partial class GameOverScreen : Control
{
	private Button _restartButton;
	private Button _menuButton;
	private Label _scoreLabel;
	private Label _highScoreLabel;
	
	public override void _Ready()
	{
		_restartButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/HBoxContainer/Restart_Button");
		_menuButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/HBoxContainer/Menu_Button");
		_scoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/Score");
		_highScoreLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HighScore");
		// Connect the button signals
		_restartButton.ButtonDown += OnRestartButtonPressed;
		_menuButton.ButtonDown += OnMenuButtonPressed;
	}
	
	private void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}
	
	private void OnMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu/main_menu.tscn");
	}
	
	public void SetScore(int score)
	{
		_scoreLabel.Text = "SCORE: " + score;
	}
	
	public void SetHighScore(int highScore)
	{
		_highScoreLabel.Text = "HI-SCORE: " + highScore;
	}
}
