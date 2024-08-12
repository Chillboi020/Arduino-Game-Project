extends Control

@onready var restart_button = $Panel/MarginContainer/VBoxContainer/HBoxContainer/Restart_Button as Button
@onready var menu_button = $Panel/MarginContainer/VBoxContainer/HBoxContainer/Menu_Button as Button
@onready var score_label = $Panel/MarginContainer/VBoxContainer/Score as Label
@onready var high_score_label = $Panel/MarginContainer/VBoxContainer/HighScore as Label

func _ready():
	restart_button.button_down.connect(on_restart_button_pressed)
	menu_button.button_down.connect(on_menu_button_pressed)
	
func on_restart_button_pressed() -> void:
	get_tree().reload_current_scene()
	
func on_menu_button_pressed() -> void:
	get_tree().change_scene_to_file("res://scenes/menu/main_menu.tscn")

func set_score(value):
	score_label.text = "SCORE:" + str(value)
	
func set_high_score(value):
	high_score_label.text = "HI-SCORE:" + str(value)
