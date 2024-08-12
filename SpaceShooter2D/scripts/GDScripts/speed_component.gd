extends Node2D
class_name SpeedComponent

@export var MAX_SPEED := 1.0
@onready var speed : float

func _ready():
	speed = MAX_SPEED * 100
	
func get_speed():
	return speed
	
func set_speed(amount : float):
	speed = amount

func add_speed(amount : float):
	speed += (amount * 100)
