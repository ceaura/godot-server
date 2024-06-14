extends RigidBody2D
class_name Laser

const IMPULSE_STRENGTH = 20
var angle : float
@onready var audio_laser = $laser_sfx

# Called when the node enters the scene tree for the first time.
func _ready():
	audio_laser.play()
	angle = get_parent().rotation
	
func _physics_process(delta):
	apply_central_impulse(Vector2(cos(angle), sin(angle)) * IMPULSE_STRENGTH)


func _on_lifespan_timeout():
	queue_free()
