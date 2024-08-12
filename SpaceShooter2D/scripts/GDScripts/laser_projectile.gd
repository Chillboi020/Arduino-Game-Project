extends Area2D

@export var damage = 1
@onready var speed = $SpeedComponent

# Called when the node enters the scene tree for the first time.
func _physics_process(delta):
	global_position.y += -speed.get_speed() * delta

func _on_visible_on_screen_notifier_2d_screen_exited():
	queue_free()

func _on_area_entered(area):
	if area is Hitbox:
		area.get_parent().take_damage(damage)
		queue_free()
