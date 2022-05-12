extends Control

var scene_path_to_load
var score
onready var topScore = false
var firstSwap = true

func _ready():
	$Menu/CenterRow/Buttons/StartGameButton.grab_focus()
	for button in $Menu/CenterRow/Buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed", [button.scene_to_load])
	var f = File.new()
	f.open("Data/Score.txt", File.READ)
	score = f.get_line()
	f.close()
	$Menu/Score/ScoreVar.text = score
	updateMilestone()
	
	checkHighScore()
	if(topScore == true):
		$HighScorePopup.popup_exclusive = true
		$HighScorePopup.popup_centered_ratio(.50)
	
	displayHighScores()

func displayHighScores():
	var f = File.new()
	f.open("Data/HighScore.txt", File.READ)
	#check if score achieved is greater than one of the scoreboard scores
	var nameText = ""
	var scoreText = ""
	var number = 1
	while not f.eof_reached():
		var lineTwo = f.get_line()
		var split = lineTwo.rsplit(",")
		if(lineTwo != "\n" and lineTwo != ""):
			nameText += (str(number) + ". " + split[0] + "\n\n")
			scoreText += (split[1] + "\n\n")
			number += 1
	$Menu/HighScoreContainer/NameContainer/Var.text = nameText
	$Menu/HighScoreContainer/ScoreContainer/Var.text = scoreText

func checkHighScore():
	var f = File.new()
	f.open("Data/HighScore.txt", File.READ)
	#check if score achieved is greater than one of the scoreboard scores
	while not f.eof_reached():
		var lineTwo = f.get_line()
		var split = lineTwo.rsplit(",")
		if(lineTwo != "\n" and lineTwo != ""):
			if(int(score) > int(split[1])):
				topScore = true
	f.close()

func updateHighScore(scoreAch, name):
	var f = File.new()
	var curScore = scoreAch
	var curName = name
	f.open("Data/HighScore.txt", File.READ)
	
	#check if score achieved is greater than one of the scoreboard scores
	var updatedText = ""
	while not f.eof_reached():
		var lineTwo = f.get_line()
		var split = lineTwo.rsplit(",")
		if(lineTwo != "\n" and lineTwo != ""):
			if(int(curScore) > int(split[1])):
				updatedText += (curName +  "," + str(curScore) + "\n")
				curScore = split[1]
				curName = split[0]
			else:
				updatedText += (split[0] + "," + split[1] + "\n")
	f.close()
	
	print("Updated High Scores:\n" + updatedText)
	
	f.open("Data/HighScore.txt", File.WRITE)
	f.store_line(updatedText)
	f.close()
	
func updateMilestone():
	var updatedText = "";
	var f = File.new()
	f.open("res://Data/milestones.txt", File.READ)
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
	f.open("res://Data/milestones.txt", File.WRITE)
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

func _on_ConfirmButton_pressed():
	updateHighScore(score, $HighScorePopup/VBoxContainer/Name.text)
	$HighScorePopup.hide()
	displayHighScores()
