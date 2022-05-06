extends Area2D

var rng = RandomNumberGenerator.new()

onready var playerInRange = false
onready var chestUsed = false
onready var gold = preload("res://Scenes/GoldPickup.tscn")
onready var ammo = preload("res://Scenes/HealthPickup.tscn")
onready var health = preload("res://Scenes/AmmoPickup.tscn")


onready var loot = [gold.instance(), ammo.instance(), health.instance()]

# Called when the node enters the scene tree for the first time.
func _ready():
	pass



func _process(delta):
	if (playerInRange and Input.is_action_just_pressed("pickup") and !chestUsed):
		rng.randomize()
		var randNum = rng.randi_range(0, (loot.size() - 1))
		loot[randNum].set_position(self.position)
		get_parent().add_child(loot[randNum])
		queue_free()
		
	  
func removeChest():
	queue_free()

func _on_Area2D_input_event(viewport, event, shape_idx):
	pass # Replace with function body.


func _on_Area2D_body_entered(body):
	#if(body is Player):
		playerInRange = true
		$ChestSprite/KeySprite.show()

func _on_Area2D_body_exited(body):
	playerInRange = false
	$ChestSprite/KeySprite.hide()
