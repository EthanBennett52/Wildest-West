extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	_on_World_startLoading()

func _on_Player_death():
	get_tree().change_scene("res://death_screen/DeathScreen.tscn")

func _on_World_startLoading():
	$Player/InterfaceLayer/Interface.hide()
	$ColorRect.show()

func _on_World_endLoading():
	$Player/InterfaceLayer/Interface.show()
	$ColorRect.hide()

func _on_World_endInitialLoading():
	OS.delay_msec(1000)
	
