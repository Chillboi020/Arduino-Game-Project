using Godot;

namespace SpaceShooter2D.scripts;

public partial class Hud : Control
{
	private Label _score;

	public Label Score
	{
		get => _score;
		private set => _score.Text = "SCORE: " + value;
	}

	private Label _health;
	public Label Health
	{
		get => _health;
		private set => _health.Text = value.ToString();
	}

	private TextureProgressBar _bonusProgressBar;
	public TextureProgressBar BonusProgressBar
	{
		get => _bonusProgressBar;
		private set => _bonusProgressBar.Value = value.Value;
	}

	public override void _Ready()
	{
		_score = GetNode<Label>("Score");
		_health = GetNode<Label>("Health");
		_bonusProgressBar = GetNode<TextureProgressBar>("BonusProgressBar");
	}
}
