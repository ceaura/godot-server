extends "res://addons/gut/test.gd"

func test_spaceship_initialization():
	var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
	var spaceship = spaceship_scene.instantiate()
	assert_not_null(spaceship, "Spaceship should be initialized correctly")
	assert_eq(spaceship.speed, 200, "Speed should be set to default value")
	assert_eq(spaceship.rotation_speed, 1, "Rotation speed should be set to default value")
	assert_true(spaceship.can_shoot, "Spaceship should be able to shoot initially")

func test_set_motor_left():
	var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
	var spaceship = spaceship_scene.instantiate()
	spaceship.set_motor_left(0.75)
	assert_eq(spaceship.motL, 0.75, "Left motor value should be set correctly")

func test_set_motor_right():
	var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
	var spaceship = spaceship_scene.instantiate()
	spaceship.set_motor_right(0.25)
	assert_eq(spaceship.motR, 0.25, "Right motor value should be set correctly")

