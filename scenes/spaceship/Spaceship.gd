extends Area2D

const MIN_GUNTRIG_SHOOT_AVAILABLE = 0.5
var motL = 0.5
var motR = 0.5
var gunTrig = 0
var speed = 200
var rotation_speed = 1

var message_timer: Timer

#### ON READY ####
@onready var player_name = $Name
@onready var message_label = $Msg
@onready var marker = $laser

#### EXPORT ####
@export var laser_scene : PackedScene


func _ready():
	set_physics_process(true)
	message_timer = Timer.new()
	message_timer.set_wait_time(6)  # seconds
	message_timer.connect("timeout", _on_message_timer_timeout)
	add_child(message_timer)

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
	
	if gunTrig > MIN_GUNTRIG_SHOOT_AVAILABLE:
		print(gunTrig)
		shoot()

func set_motor_left(value):
	motL = clamp(value, 0.0, 1.0)

func set_motor_right(value):
	motR = clamp(value, 0.0, 1.0)

func map_motor_value(input_value):
	return 2 * (input_value - 0.5)
	
func set_player_name(name):
	player_name.text = name

func set_message(msg):
	message_label.text = msg.substr(0, 30)  # Truncate if longer than 30 characters
	message_label.show()
	message_timer.start()

func _on_message_timer_timeout():
	message_label.hide()
	
func shoot():
	var laser = laser_scene.instantiate()
	laser.transform = marker.transform
	add_child(laser)

func setGunTrig(value):
	gunTrig = value
	shoot()
	print("Guntrig = " , str(gunTrig))
