extends Entity
class_name Enemy

signal hit(amount : float)
signal killed

@export var POINTS := 1.0
@export var PENALTY := -10.0
@export var DAMAGE := 1.0

@onready var hp = $HealthComponent
@onready var animation = $AnimatedSprite2D
@onready var speed = 0.0

var is_hurt = false
var points = 0.0
var penalty = 0.0
var damage = 0.0

func _ready():
	points = POINTS
	penalty = PENALTY
	damage = DAMAGE
	speed = $SpeedComponent.get_speed()

func process_animation():
	if play_animation:
		play_animation = false
		animation_length = 7.5
		animation.play("hurt")
	
	if animation_length > 0:
		animation_length -= 1
	elif animation_length <= 0:
		is_hurt = false
		animation.play("default")

func die():
	queue_free()
	
func take_damage(amount):
	hp.damage(amount)
	if hp.get_health() <= 0:
		killed.emit(points)
	else:
		hit.emit()
		is_hurt = true
		play_animation = true

func _on_visible_on_screen_notifier_2d_screen_exited():
	killed.emit(penalty)
	die()

func _on_hitbox_area_entered(area):
	if area is Hitbox:
		area.get_parent().take_damage(damage)
		die()
