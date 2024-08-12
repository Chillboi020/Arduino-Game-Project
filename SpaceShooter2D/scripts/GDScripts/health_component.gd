extends Node2D
class_name HealthComponent

@export var MAX_HEALTH := 1.0
@onready var health : float

func _ready():
	health = MAX_HEALTH
	
func damage(amount : float):
	health -= amount
	
	if health <= 0:
		get_parent().die()

func get_health():
	return health

func add_health(amount : float):
	health += amount
