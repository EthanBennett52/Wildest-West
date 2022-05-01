extends HBoxContainer

# Called when the node enters the scene tree for the first time.

func _on_Player_changeHealth(current, maxHealth):
	$TextureProgress.value = current
	$Counter/Label.text = str(current) + "/" + str(maxHealth)
	$TextureProgress.max_value = maxHealth

func _ready():
	$Counter/Label.text = "100/100"

func _on_Player_changeMaxHealth(current, maxHealth):
	$TextureProgress.value = current
	$TextureProgress.max_value = maxHealth
