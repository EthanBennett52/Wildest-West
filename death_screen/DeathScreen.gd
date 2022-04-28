extends Control

var scene_path_to_load

func _ready():
	$Menu/CenterRow/Buttons/StartGameButton.grab_focus()
	for button in $Menu/CenterRow/Buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed", [button.scene_to_load])


func _on_StartGameButton_pressed(scene_to_load):
	print("button pressed")
	$FadeIn.show()
	$FadeIn.fade_in()
	scene_path_to_load = scene_to_load

func _on_FadeIn_fade_finished():
	$FadeIn.hide()
	get_tree().change_scene(scene_path_to_load)

func _on_ExitGameButton_pressed():
	get_tree().quit()



func _on_TitleScreenButton_pressed():
	get_tree().change_scene("res://title_screen/TitleScreen.tscn")
