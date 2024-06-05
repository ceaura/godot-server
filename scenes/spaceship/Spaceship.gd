extends Area2D

var motL = 0.5
var motR = 0.5
var speed = 500
var rotation_speed = 1

func _ready():
	set_physics_process(true)

func _physics_process(delta):
	var mapped_motL = map_motor_value(motL)
	var mapped_motR = map_motor_value(motR)

	if mapped_motL == 0 and mapped_motR == 0:
		return

	# Calculate movement
	var rotation_diff = mapped_motR - mapped_motL
	rotation += rotation_diff * rotation_speed * delta

	var thrust = (mapped_motL + mapped_motR) / 2.0
	var direction = Vector2(0, -1).rotated(rotation)

	global_position += direction * thrust * speed * delta

func set_motor_left(value):
	motL = clamp(value, 0.0, 1.0)

func set_motor_right(value):
	motR = clamp(value, 0.0, 1.0)

func map_motor_value(input_value):
	return 2 * (input_value - 0.5)
