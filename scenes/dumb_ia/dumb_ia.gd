extends Node2D

@onready var spaceship = $spaceship
@onready var value_dumb_ia_interval = $value_dumb_ia_interval

# Called when the node enters the scene tree for the first time.
func _ready():
	spaceship.set_motor_left(0.5)
	spaceship.set_motor_right(0.5)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func dumb_ia_value():
	spaceship.set_motor_left(rand_float())
	spaceship.set_motor_right(rand_float())
	spaceship.setGunTrig(rand_float())


func rand_float():
	return randf_range(0.1,1.0)
	
func _on_value_dumb_ia_interval_timeout():
	dumb_ia_value()
		
