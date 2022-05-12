extends Control

onready var killMilestonePath = $Panel/VBoxContainer/KillMilestone/KillPictureButton
onready var ScoreMilestonePath = $Panel/VBoxContainer/ScoreMilestone/ScorePictureButton
onready var RevolverMilestonePath = $Panel/VBoxContainer/RevolverOnlyMilestone/RevolverPictureButton
onready var DamageMilestonePath = $Panel/VBoxContainer/NoHitMilestone/NoHitPictureButton
onready var AmmoMilestonePath = $Panel/VBoxContainer/NoAmmoPickupMilestone/AmmoPictureButton
onready var DeathsMilestonePath = $Panel/VBoxContainer/DeathMilestone/DeathPictureButton

onready var activePicTecture = $Panel/VBoxContainer/HBoxContainer/ActivePic.texture
onready var notActivePicTexture = $Panel/VBoxContainer/HBoxContainer/NotActivePic.texture
onready var notAccompPicTexture =  $Panel/VBoxContainer/HBoxContainer/NotAccomplishedPic.texture

onready var path
onready var file = "res://Data/milestones.txt"

func _ready():
	var updatedText = ""
	var f = File.new()
	f.open(file, File.READ)
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(line != "\n" and line != ""):
			if(int(split[2]) >= int(split[3])):
				unlock(split[0], split[5])
				split[4] = "true";
				updatedText += (split[0] + "," + split[1] + "," + split[2] + "," + split[3] + "," + split[4] + "," + split[5] + "\n")
			else:
				updatedText += (line + "\n")
			updateText(split[0], split[2], split[3])
	f.close()
	
	f.open(file, File.WRITE)
	f.store_line(updatedText)
	f.close()
	print(updatedText)

func updateText(name, current, maximum):
	if(name == "Kill"):
		$Panel/VBoxContainer/KillMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Enemies Killed"
	elif(name == "Score"):
		$Panel/VBoxContainer/ScoreMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Score Achieved"
	elif(name == "Revolver"):
		$Panel/VBoxContainer/RevolverOnlyMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Kills With The Revolver"
	elif(name == "Damage"):
		$Panel/VBoxContainer/NoHitMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Kills With Out Taking Damage"
	elif(name == "Ammo"):
		$Panel/VBoxContainer/NoAmmoPickupMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Kills Without Picking Up Ammo Packs"
	elif(name == "Deaths"):
		$Panel/VBoxContainer/DeathMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Deaths"

func unlock(name, enabled):
	if(name == "Kill" and enabled == "disabled"):
		killMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Kill" and enabled == "enabled"):
		killMilestonePath.set_normal_texture(activePicTecture)
	
	if(name == "Score" and enabled == "disabled"):
		ScoreMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Score" and enabled == "enabled"):
		ScoreMilestonePath.set_normal_texture(activePicTecture)
	
	if(name == "Revolver" and enabled == "disabled"):
		RevolverMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Revolver" and enabled == "enabled"):
		RevolverMilestonePath.set_normal_texture(activePicTecture)
	
	if(name == "Damage" and enabled == "disabled"):
		DamageMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Damage" and enabled == "enabled"):
		DamageMilestonePath.set_normal_texture(activePicTecture)
	
	if(name == "Ammo" and enabled == "disabled"):
		AmmoMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Ammo" and enabled == "enabled"):
		AmmoMilestonePath.set_normal_texture(activePicTecture)
	
	if(name == "Deaths" and enabled == "disabled"):
		DeathsMilestonePath.set_normal_texture(notActivePicTexture)
	elif(name == "Deaths" and enabled == "enabled"):
		DeathsMilestonePath.set_normal_texture(activePicTecture)

func changePic():
	if(path.texture_normal == notAccompPicTexture):
		pass
	elif(path.texture_normal == notActivePicTexture):
		path.set_normal_texture(activePicTecture)
	elif(path.texture_normal == activePicTecture):
		path.set_normal_texture(notActivePicTexture)
		
func saveChange():
	var updatedText = "";
	var f = File.new()
	f.open(file, File.READ)
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(line != "\n" and line != ""):
			var enableOrDisable
			if(split[0] == "Kill" and killMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Kill" and killMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"
				
			if(split[0] == "Score" and ScoreMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Score" and ScoreMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"
			
			if(split[0] == "Revolver" and RevolverMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Revolver" and RevolverMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"
				
			if(split[0] == "Damage" and DamageMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Damage" and DamageMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"	
				
			if(split[0] == "Ammo" and AmmoMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Ammo" and AmmoMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"
			
			if(split[0] == "Deaths" and DeathsMilestonePath.texture_normal == activePicTecture):
				split[5] = "enabled"
			elif(split[0] == "Deaths" and DeathsMilestonePath.texture_normal == notActivePicTexture):
				split[5] = "disabled"
			
			if(split[4] == "true"):
				updatedText += (split[0] + "," + split[1] + "," + split[2] + "," + split[3] + "," + split[4] + "," + split[5] + "\n")
			else:
				updatedText += (line + "\n")
	f.close()
	
	f.open(file, File.WRITE)
	f.store_line(updatedText)
	f.close()

func _on_KillPictureButton_pressed():
	path = $Panel/VBoxContainer/KillMilestone/KillPictureButton
	changePic()

func _on_ScorePictureButton_pressed():
	path = $Panel/VBoxContainer/ScoreMilestone/ScorePictureButton
	changePic()

func _on_RevolverPictureButton_pressed():
	path = $Panel/VBoxContainer/RevolverOnlyMilestone/RevolverPictureButton
	changePic()

func _on_NoHitPictureButton_pressed():
	path = $Panel/VBoxContainer/NoHitMilestone/NoHitPictureButton
	changePic()

func _on_AmmoPictureButton_pressed():
	path = $Panel/VBoxContainer/NoAmmoPickupMilestone/AmmoPictureButton
	changePic()

func _on_DeathPictureButton_pressed():
	path = $Panel/VBoxContainer/DeathMilestone/DeathPictureButton
	changePic()

func _on_ReturnButton_pressed():
	saveChange()
	get_tree().change_scene("res://title_screen/TitleScreen.tscn")
