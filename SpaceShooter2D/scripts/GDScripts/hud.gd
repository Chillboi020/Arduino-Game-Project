extends Control

@onready var score = $Score:
	set(value):
		score.text = "SCORE:" + str(value)

@onready var health = $Health:
	set(value):
		health.text = str(value)

@onready var bonus_progress_bar = $BonusProgressBar as TextureProgressBar:
	set(value):
		bonus_progress_bar.value = value
