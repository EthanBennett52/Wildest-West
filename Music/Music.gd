extends AudioStreamPlayer

var rng = RandomNumberGenerator.new()
# Declare member variables here. Examples:
# var a = 2
# var b = "text"
onready var songs = ["res://Music/HoliznaCC0 - A Yankees Southern Blues.mp3", "res://Music/Mr Smith - Americana.mp3","res://Music/Mr Smith - The Get Away.mp3"]
onready var lastPlayed

# Called when the node enters the scene tree for the first time.
func _ready():
	#playRandomSong()
	pass

func playRandomSong():
	rng.randomize()
	var randNum = rng.randi_range(0, (songs.size() - 1))
	if(lastPlayed != songs[randNum]):
		self.stream = load(songs[randNum])
		lastPlayed = songs[randNum]
		self.playing = true
	else:
		playRandomSong()

func _on_Music_finished():
	playRandomSong()
