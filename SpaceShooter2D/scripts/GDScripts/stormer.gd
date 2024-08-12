extends Enemy
class_name Stormer

func _physics_process(delta):
	global_position.y += speed * delta
	
	if is_hurt:
		process_animation()
