extends HBoxContainer


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var score = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	$ScoreVar.text = str(score)



func _process(delta):
	var f = File.new()
	f.open("interface/Score.txt", File.READ)
	$ScoreVar.text = f.get_line()
	f.close()
