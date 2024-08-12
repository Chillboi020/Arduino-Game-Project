extends Control
class_name MainMenu

@onready var play_button = $MarginContainer/HBoxContainer/VBoxContainer/Play_Button as Button
@onready var quit_button = $MarginContainer/HBoxContainer/VBoxContainer/Quit_Button as Button
@onready var margin_container = $MarginContainer as MarginContainer
@onready var pb = $ParallaxBackground

@onready var start_game = preload("res://scenes/game/game.tscn") as PackedScene
@export var pb_speed = 100

func _ready():
	handle_connecting_signals()
	pb.get_child(0).get_child(0).region_rect = Rect2(0, 0, 
	get_viewport_rect().size.x, get_viewport_rect().size.y)

func _process(delta):
	if Input.is_action_just_pressed("main_escape"):
		on_quit_pressed()
			
	# The Background scrolling
	pb.scroll_offset.x += delta * pb_speed
	# To enahnce performance we loop the background
	if pb.scroll_offset.x >= get_viewport_rect().size.x:
		pb.scroll_offset.x = 0

func on_play_pressed() -> void: 
	get_tree().change_scene_to_packed(start_game)

func on_quit_pressed() -> void:
	get_tree().quit()
	
func handle_connecting_signals() -> void:
	play_button.button_down.connect(on_play_pressed)
	quit_button.button_down.connect(on_quit_pressed)
