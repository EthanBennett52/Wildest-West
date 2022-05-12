extends Control

# Called when the node enters the scene tree for the first time.
func _ready():
	displayHighScores()

func displayHighScores():
	var f = File.new()
	f.open("Data/HighScore.txt", File.READ)
	var scorePos = 1
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
	$Panel/VBoxContainer/HighScoreContainer/NameContainer/Var.text = nameText
	$Panel/VBoxContainer/HighScoreContainer/ScoreContainer/Var.text = scoreText

func _on_ReturnButton_pressed():
	get_tree().change_scene("res://title_screen/TitleScreen.tscn")
