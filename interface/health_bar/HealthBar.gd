extends HBoxContainer

signal health_change(change)

onready var value = -50;
# Called when the node enters the scene tree for the first time.

func _on_Player_changeHealth(change):
	$TextureProgress.value = change


func _on_Player_changeMaxHealth(change):
	$TextureProgress.value = change
	$TextureProgress.max_value = change
