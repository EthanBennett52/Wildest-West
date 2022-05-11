extends Node


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	checkFile("Data/Score.txt")
	checkFile("Data/milestones.txt")

func checkFile(filePath):
	var f = File.new()
	if(f.open(filePath, File.READ) != OK):
		f.open(filePath, File.WRITE)
		if(filePath == "Data/Score.txt"):
			f.store_line("0")
		if(filePath == "Data/milestones.txt"):
			f.store_line("Kill,true,0,100,false,disabled")
			f.store_line("Score,true,0,1000,false,disabled")
			f.store_line("Revolver,true,0,1,false,disabled")
			f.store_line("Damage,true,0,1,false,disabled")
			f.store_line("Ammo,true,0,1,false,disabled")
			f.store_line("Deaths,true,0,50,false,disabled")
	f.close()

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
