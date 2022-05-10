extends HBoxContainer

func _on_Player_updateHotbarGun(name, number):
	print(name)
	print(number)	
	var path
	
	if(number == 1):
		path = $WeaponOneIcon/WeaponPic
	elif(number == 2):
		path = $WeaponTwoIcon/WeaponPic
	
	if(name == "Revolver"):
		path.texture = load("res://Art/Gun.png")
	elif(name == "MachineGun"):
		path.texture = load("res://Art/MachineGun.png")
	elif(name == "Knife"):
		path.texture = load("res://Art/Knife.png")
	elif(name == "Shotgun"):
		path.texture = load("res://Art/Shotgun.png")
	elif(name == "Rifle"):
		path.texture = load("res://Art/RifleIcon.png")

