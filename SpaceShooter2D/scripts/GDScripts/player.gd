extends Entity
class_name Player

#region Variables
signal hit(amount : float)
signal killed
signal laser_shot(laser_scene, location)

@export var acceleration = 5.0
@export var friction = 2.0
@export var fire_rate := 0.25
@export var invulnerability_duration = 0.75

@onready var muzzle = $Muzzle
@onready var animation = $AnimatedSprite2D
@onready var hp = $HealthComponent
@onready var speed = $SpeedComponent

var direction = Vector2.ZERO
var laser_scene = preload("res://scenes/laser_projectile.tscn")
var shoot_cd := false

var is_hurt = false
#endregion

#region Player Movement Methods
func _physics_process(delta):
	player_movement(delta)
	global_position = global_position.clamp(Vector2(0,30), 
	get_viewport_rect().size)
	
	if is_hurt:
		process_animation()

func process_animation():
	if play_animation:
		play_animation = false
		animation_length = invulnerability_duration * 100
		animation.play("hurt")
	
	if animation_length > 0:
		animation_length -= 1
		animation.speed_scale += 0.05
	elif animation_length == 0:
		animation.play("default")
		animation.speed_scale = 1
		is_hurt = false

func player_movement(delta):
	direction = get_direction()
	
	if direction == Vector2.ZERO:
		var decceleration = (friction * 1000 ) * delta
		if velocity.length() > decceleration:
			velocity -= velocity.normalized() * decceleration
		else:
			velocity = Vector2.ZERO
	else:
		velocity += (direction * (acceleration * 1000 ) * delta)
		velocity = velocity.limit_length(speed.get_speed())
	move_and_slide()

func get_direction():
	direction = Vector2(Input.get_axis("player_move_left", "player_move_right"), 
	Input.get_axis("player_move_up", "player_move_down"))
	return direction.normalized()
#endregion

#region Player Shoot Methods
func _process(delta):
	if Input.is_action_pressed("player_shoot"):
		if !shoot_cd:
			shoot_cd = true
			shoot()
			await get_tree().create_timer(fire_rate).timeout
			shoot_cd = false

func shoot():
	laser_shot.emit(laser_scene, muzzle.global_position)
#endregion

#region Player Damage Methods
func die():
	killed.emit()
	queue_free()
	
func take_damage(amount):
	if !is_hurt:
		hit.emit(amount)
		hp.damage(amount)
		is_hurt = true
		play_animation = true
#endregion
