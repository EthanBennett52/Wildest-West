extends Area2D

onready var playerInRange = false
onready var chestUsed = false
onready var gold = preload("res://Scenes/GoldPickup.tscn")
onready var goldInst = gold.instance()


# Called when the node enters the scene tree for the first time.
func _ready():
	pass



func _process(delta):
	if (playerInRange and Input.is_action_just_pressed("pickup") and !chestUsed):
		add_child(goldInst)
		
		
	  
func removeChest():
	get_parent().remove_child(self)

func _on_Area2D_input_event(viewport, event, shape_idx):
	pass # Replace with function body.


func _on_Area2D_body_entered(body):
	#if(body is Player):
		playerInRange = true
		$ChestSprite/KeySprite.show()

func _on_Area2D_body_exited(body):
	playerInRange = false
	$ChestSprite/KeySprite.hide()
