extends Node2D

var motL = 0.5
var motR = 0.5
var speed = 10
var rotation_speed = 3

func _ready():
	set_physics_process(true)

func _physics_process(delta):
	var direction = Vector2(0, -1).rotated(rotation)
	var rotation_diff = motR - motL
	rotation += rotation_diff * rotation_speed * delta
	position += direction * speed * (motL + motR) / 2 * delta

func set_motor_left(value):
	motL = clamp(value, 0.0, 1.0)

func set_motor_right(value):
	motR = clamp(value, 0.0, 1.0)
