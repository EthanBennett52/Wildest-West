extends Control

var scene_path_to_load

func _ready():
	$Menu/CenterRow/Buttons/StartGameButton.grab_focus()
	for button in $Menu/CenterRow/Buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed", [button.scene_to_load])
	var f = File.new()
	f.open("interface/Score.txt", File.READ)
	$Score/ScoreVar.text = f.get_line()
	f.close()


func _on_StartGameButton_pressed():
	print("button pressed")
	$FadeIn.show()
	$FadeIn.fade_in()
	get_tree().change_scene("res://Scenes/Main.tscn")

func _on_FadeIn_fade_finished():
	$FadeIn.hide()
	get_tree().change_scene(scene_path_to_load)

func _on_ExitGameButton_pressed():
	get_tree().quit()



func _on_TitleScreenButton_pressed():
	get_tree().change_scene("res://title_screen/TitleScreen.tscn")
