class_name Game extends Node2D

#region Variables
@export var enemy_scenes: Array[PackedScene] = []
@export var player_points_needed_for_bonus_hp = 4000.0

@onready var laser_container = $LaserContainer
@onready var enemy_container = $EnemyContainer

@onready var timer = $EnemySpawnTimer
@onready var hud = $UiLayer/HUD
@onready var gos = $UiLayer/GameOverScreen
@onready var pb = $ParallaxBackground

@onready var laser_sound = $SFX/LaserSound
@onready var hit_sound = $SFX/HitSound
@onready var explode_sound = $SFX/ExplodeSound

var screen_size
var player : Player = null
var hi_score = 0
var scroll_speed = 100
var is_player_dead = false
var cur_player_score = 0.0

# with setter
var score := 0.0:
	set(value):
		score = value
		hud.score = score
var health := 0:
	set(value):
		health = value
		hud.health = health
var bonus_progress_value := 0.0:
	set(value):
		bonus_progress_value = value
		hud.bonus_progress_bar = value
#endregion

# Called when the node enters the scene tree for the first time.
func _ready():
	# load the high-score
	var save_file = FileAccess.open("user://Score.save", FileAccess.READ)
	if save_file != null:
		var json_string = save_file.get_line()
		var json = JSON.new()
		var _parsed_result = json.parse(json_string)
		var data_received = json.get_data()
		hi_score = data_received.hi_score
	else:
		hi_score = 0
		save_game()
	
	# get the games' screensize
	screen_size = get_viewport_rect().size
	
	# create the player
	player = get_tree().get_first_node_in_group("player")
	assert(player != null)
	# make his spawn position in the bottom center
	player.global_position = Vector2(screen_size.x / 2, screen_size.y - 50)
	# connect the various signals the player needs
	player.laser_shot.connect(_on_player_laser_shot)
	player.killed.connect(_on_player_killed)
	player.hit.connect(_on_player_hit)
	
	# set the ParallaxBackground to match with the screensize
	pb.get_child(0).get_child(0).region_rect = Rect2(0, 0, screen_size.x, screen_size.y)

	# score and health get displayed
	score = 0.0
	health = player.hp.get_health()
	bonus_progress_value = 0.0

# save the highscore when the game is over
func save_game():
	var save_file = FileAccess.open("user://Score.save", FileAccess.WRITE)
	var hi_score_for_json = { "hi_score" : hi_score}
	var hi_score_json = JSON.stringify(hi_score_for_json)
	save_file.store_line(hi_score_json)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	# quit
	if Input.is_action_just_pressed("main_escape"):
		get_tree().change_scene_to_file("res://scenes/menu/main_menu.tscn")
	
	# the enemy spawn timer, as for now they gradually 
	# spawn more frequently the more time passes
	# change it when level system is made
	if !is_player_dead:
		if timer.wait_time > 0.5:
			timer.wait_time -= delta * 0.005
		elif timer.wait_time < 0.5:
			timer.wait_time = 0.5
	else:
		if bonus_progress_value > 0:
			bonus_progress_value -= 0.5
	
	# The Background scrolling
	pb.scroll_offset.y += delta * scroll_speed
	# To enahnce performance we loop the background
	if pb.scroll_offset.y >= screen_size.y:
		pb.scroll_offset.y = 0

# when the player shoots
func _on_player_laser_shot(laser_scene, location):
	var laser = laser_scene.instantiate()
	laser.global_position = location
	laser_container.add_child(laser)
	laser_sound.play()

# when the timer is over, then we spawn a new enemy
func _on_enemy_spawn_timer_timeout():
	# picks a random enemy type from the container
	var e = enemy_scenes.pick_random().instantiate()
	
	# it will be spawned somewhere above the screen
	e.global_position = Vector2(randf_range(50, screen_size.x - 50), -50)
	# connect the signals for an enemy
	e.killed.connect(_on_enemy_killed)
	e.hit.connect(_on_enemy_hit)
	
	enemy_container.add_child(e)

# when an enemy gets hit
func _on_enemy_hit():
	hit_sound.play()

# when an enemy dies
func _on_enemy_killed(points):
	if score + points < 0:
		score = 0
	else:
		if !is_player_dead:
			if points > 0:
				hit_sound.play()
			score += points
			if cur_player_score + points < 0:
				cur_player_score = 0
			else:
				cur_player_score += points
			update_bonus()
			if cur_player_score >= player_points_needed_for_bonus_hp:
				bonus_health()
			if score > hi_score:
				hi_score = score

func _on_player_hit(amount):
	hit_sound.play()
	health -= amount

func _on_player_killed():
	is_player_dead = true
	health = 0
	explode_sound.play()
	game_over()

func game_over():
	gos.set_score(score)
	gos.set_high_score(hi_score)
	save_game()
	await get_tree().create_timer(1.5).timeout
	gos.visible = true
	timer.wait_time = 0.001

func update_bonus():
	bonus_progress_value = int((cur_player_score / player_points_needed_for_bonus_hp) * 100)

func bonus_health():
		player.hp.add_health(1)
		health = player.hp.get_health()
		cur_player_score -= player_points_needed_for_bonus_hp 
		player_points_needed_for_bonus_hp *= 2
		update_bonus()
