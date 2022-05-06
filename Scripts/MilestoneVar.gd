extends Node

onready var file = "res://milestone_screen/milestones.txt"
onready var KillCanAchieve
onready var KillAmount
onready var KillMax
onready var KillAchieved
onready var KillEnabled

onready var ScoreCanAchieve
onready var ScoreAmount
onready var ScoreMax
onready var ScoreAchieved
onready var ScoreEnabled

onready var RevolverCanAchieve
onready var RevolverAmount
onready var RevolverMax
onready var RevolverAchieved
onready var RevolverEnabled

onready var DamageCanAchieve
onready var DamageAmount
onready var DamageMax
onready var DamageAchieved
onready var DamageEnabled

onready var AmmoCanAchieve
onready var AmmoAmount
onready var AmmoMax
onready var AmmoAchieved
onready var AmmoEnabled

onready var DeathsCanAchieve
onready var DeathsAmount
onready var DeathsMax
onready var DeathsAchieved
onready var DeathsEnabled

# Called when the node enters the scene tree for the first time.
func _ready():
	var f = File.new()
	f.open(file, File.READ)
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(line != "\n" and line != ""):
			if(split[0] == "Kill"):
				KillCanAchieve = split[1]
				KillAmount = split[2]
				KillMax = split[3]
				KillAchieved = split[4]
				KillEnabled = split[5]
			elif(split[0] == "Score"):
				ScoreCanAchieve = split[1]
				ScoreAmount = split[2]
				ScoreMax = split[3]
				ScoreAchieved = split[4]
				ScoreEnabled = split[5]
			elif(split[0] == "Revolver"):
				RevolverCanAchieve = split[1]
				RevolverAmount = split[2]
				RevolverMax = split[3]
				RevolverAchieved = split[4]
				RevolverEnabled = split[5]
			elif(split[0] == "Damage"):
				DamageCanAchieve = split[1]
				DamageAmount = split[2]
				DamageMax = split[3]
				DamageAchieved = split[4]
				DamageEnabled = split[5]
			elif(split[0] == "Ammo"):
				AmmoCanAchieve = split[1]
				AmmoAmount = split[2]
				AmmoMax = split[3]
				AmmoAchieved = split[4]
				AmmoEnabled = split[5]
			elif(split[0] == "Deaths"):
				AmmoCanAchieve = split[1]
				AmmoAmount = split[2]
				AmmoMax = split[3]
				AmmoAchieved = split[4]
				AmmoEnabled = split[5]
	f.close()

func save():
	var updatedText = ""
	var f = File.new()
	f.open(file, File.READ)
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(line != "\n" and line != ""):
			if(split[0] == "Kill"):
				updatedText += (split[0] + "," + KillCanAchieve + "," + KillAmount + "," + KillMax + "," + KillAchieved + "," + KillMax + "\n")
			elif(split[0] == "Score"):
				updatedText += (split[0] + "," + ScoreCanAchieve + "," + ScoreAmount + "," + ScoreMax + "," + ScoreAchieved + "," + ScoreMax + "\n")
			elif(split[0] == "Revolver"):
				updatedText += (split[0] + "," + RevolverCanAchieve + "," + RevolverAmount + "," + RevolverMax + "," + RevolverAchieved + "," + RevolverMax + "\n")
			elif(split[0] == "Damage"):
				updatedText += (split[0] + "," + DamageCanAchieve + "," + DamageAmount + "," + DamageMax + "," + DamageAchieved + "," + DamageMax + "\n")
			elif(split[0] == "Ammo"):
				updatedText += (split[0] + "," + AmmoCanAchieve + "," + AmmoAmount + "," + AmmoMax + "," + AmmoAchieved + "," + AmmoMax + "\n")
			elif(split[0] == "Deaths"):
				updatedText += (split[0] + "," + DeathsCanAchieve + "," + DeathsAmount + "," + DeathsMax + "," + DeathsAchieved + "," + DeathsMax + "\n")
		else:
			updatedText += (line + "\n")
			
	f.close()
	
	f.open(file, File.WRITE)
	f.store_line(updatedText)
	f.close()
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
