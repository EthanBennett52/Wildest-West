extends Control

var scene_path_to_load

func _ready():
	OS.window_fullscreen = true
	
	$Menu/CenterRow/Buttons/StartGameButton.grab_focus()
	for button in $Menu/CenterRow/Buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed", [button.scene_to_load])
		
func _on_Button_pressed(scene_to_load):
	$FadeIn.show()
	$FadeIn.fade_in()
	scene_path_to_load = scene_to_load

func _on_FadeIn_fade_finished():
	$FadeIn.hide()
	get_tree().change_scene(scene_path_to_load)

func _on_ExitButton_pressed():
	get_tree().quit()

func _on_MilestonesButton_pressed():
	get_tree().change_scene("res://milestone_screen/Milestones.tscn")

func _on_OptionsButton_pressed():
	get_tree().change_scene("res://options_screen/Options.tscn")

func _on_CreditsButton_pressed():
	get_tree().change_scene("res://title_screen/Credits.tscn")

func _on_TextureButton_pressed():
	$"/root/Music".set_stream_paused(true)

func _on_ScoreButton_pressed():
	get_tree().change_scene("res://title_screen/HighScore.tscn")
