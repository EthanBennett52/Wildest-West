extends Control

var scene_path_to_load
var score

func _ready():
	$Menu/CenterRow/Buttons/StartGameButton.grab_focus()
	for button in $Menu/CenterRow/Buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed", [button.scene_to_load])
	var f = File.new()
	f.open("interface/Score.txt", File.READ)
	score = f.get_line()
	f.close()
	$Menu/Score/ScoreVar.text = score
	updateMilestone()

func updateMilestone():
	var updatedText = "";
	var f = File.new()
	f.open("res://milestone_screen/milestones.txt", File.READ)
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(split[0] == "Score" and (int(score) > int(split[2]))):
			updatedText += (split[0] + "," + split[1] + "," + score + "," + split[3] + "," + split[4] + "," + split[5] + "\n")
		elif(split[0] == "Deaths"):
			var deaths = (int(split[2]) + 1)
			print(deaths)
			updatedText += (split[0] + "," + split[1] + "," + str(deaths) + "," + split[3] + "," + split[4] + "," + split[5] + "\n")
		else:
			updatedText += (line + "\n")
	
	f.close()
	f.open("res://milestone_screen/milestones.txt", File.WRITE)
	f.store_line(updatedText)
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
