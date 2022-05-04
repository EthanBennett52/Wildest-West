extends Control

var is_paused = false setget set_is_paused

func _unhandled_input(event):
	if(event.is_action_pressed("pause")):
		self.is_paused = !is_paused
		$CenterContainer/VBoxContainer/ContinueButton.grab_focus()

func set_is_paused(value):
	
	is_paused = value
	get_tree().paused = is_paused
	visible = is_paused


func _on_ContinueButton_pressed():
	self.is_paused = false


func _on_QuitButton_pressed():
	self.is_paused = false
	get_tree().change_scene("res://title_screen/TitleScreen.tscn")


func _on_ExitButton_pressed():
	get_tree().quit()
	

func _ready():
	$CenterContainer/VBoxContainer/ContinueButton.grab_focus()
