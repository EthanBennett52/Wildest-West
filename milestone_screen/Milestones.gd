extends Control

onready var path
onready var file = "res://milestone_screen/milestones.txt"
func _ready():
	var f = File.new()
	f.open(file, File.READ)
	var index = 1
	while not f.eof_reached(): # iterate through all lines until the end of file is reached
		var line = f.get_line()
		var split = line.rsplit(",")
		if(split[2] >= split[3]):
			unlock(split[0], split[5])
		updateText(split[0], split[2], split[3])
		
	f.close()

func updateText(name, current, maximum):
	if(current >= maximum):
		pass
	
	if(name == "Kill"):
		$Panel/VBoxContainer/KillMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Enemies Killed"
	elif(name == "Score"):
		$Panel/VBoxContainer/ScoreMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Score Achieved"
	elif(name == "Revolver"):
		$Panel/VBoxContainer/RevolverOnlyMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Games Beaten With Only The Revolver"
	elif(name == "Damage"):
		$Panel/VBoxContainer/NoHitMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Games Beaten With Out Taking Damage"
	elif(name == "Ammo"):
		$Panel/VBoxContainer/NoAmmoPickupMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Games Beaten With Out Picking Up Ammo"
	elif(name == "Deaths"):
		$Panel/VBoxContainer/DeathMilestone/VBoxContainer/MilestoneProgress.text = str(current) + "/" + str(maximum) + " Deaths"

func unlock(name, enabled):
	if(name == "Kill" and enabled == "disabled"):
		$Panel/VBoxContainer/KillMilestone/KillPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	elif(name == "Score" and enabled == "disabled"):
		$Panel/VBoxContainer/ScoreMilestone/ScorePictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	elif(name == "Revolver" and enabled == "disabled"):
		$Panel/VBoxContainer/RevolverOnlyMilestone/RevolverPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	elif(name == "Damage" and enabled == "disabled"):
		$Panel/VBoxContainer/NoHitMilestone/NoHitPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	elif(name == "Ammo" and enabled == "disabled"):
		$Panel/VBoxContainer/NoAmmoPickupMilestone/AmmoPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	elif(name == "Deaths" and enabled == "disabled"):
		$Panel/VBoxContainer/DeathMilestone/DeathPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)
	
	if(name == "Kill" and enabled == "enabled"):
		$Panel/VBoxContainer/KillMilestone/KillPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(name == "Score" and enabled == "enabled"):
		$Panel/VBoxContainer/ScoreMilestone/ScorePictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(name == "Revolver" and enabled == "enabled"):
		$Panel/VBoxContainer/RevolverOnlyMilestone/RevolverPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(name == "Damage" and enabled == "enabled"):
		$Panel/VBoxContainer/NoHitMilestone/NoHitPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(name == "Ammo" and enabled == "enabled"):
		$Panel/VBoxContainer/NoAmmoPickupMilestone/AmmoPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(name == "Deaths" and enabled == "enabled"):
		$Panel/VBoxContainer/DeathMilestone/DeathPictureButton.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	
func changePic():
	if(path.texture_normal == $Panel/VBoxContainer/HBoxContainer/NotAccomplishedPic.texture):
		pass
	elif(path.texture_normal == $Panel/VBoxContainer/HBoxContainer/NotActivePic.texture):
		path.set_normal_texture($Panel/VBoxContainer/HBoxContainer/ActivePic.texture)
	elif(path.texture_normal == $Panel/VBoxContainer/HBoxContainer/ActivePic.texture):
		path.set_normal_texture($Panel/VBoxContainer/HBoxContainer/NotActivePic.texture)

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
