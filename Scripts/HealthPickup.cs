using Godot;
using System;

public class HealthPickup : Node2D
{
    //How much health this pickup contains
    private int healAmount = 10;

    private bool playerInRange = false;

    private Node2D player;

    private Area2D pickupArea;
    private Area2D attractArea;

    //Called when the player picks this up
    private void OnPickup(Player player){
        player.heal(healAmount);
        QueueFree();
    }

    //Called when a Node enters this area
    private void OnBodyEnteredPickup(Node2D area){
        if (area is Player) {
            OnPickup((Player)area);
        }
    }

    private void OnBodyEnteredAttract(Node2D area){
        if (area is Player){
            player = area;
            SetProcess(true);
            playerInRange = true;
            
        }
    }

    private void OnBodyExitedAttract(Node2D area){
        if (area is Player){
            SetProcess(false);
            playerInRange = false;
            
        }
    }

    public void setHealAmount(int amount){
        healAmount = amount;
    }
    
    public override void _Ready()
    {
        SetProcess(false);
        pickupArea = (Area2D)FindNode("PickupArea");
        attractArea = (Area2D)FindNode("AttractArea");
        pickupArea.Connect("body_entered", this, "OnBodyEnteredPickup");
        attractArea.Connect("body_entered", this, "OnBodyEnteredAttract");
        attractArea.Connect("body_exited", this, "OnBodyExitedAttract");
    }

// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (playerInRange){
            GlobalPosition = GlobalPosition.MoveToward(player.GlobalPosition, delta * 250);
        }
      
    }
}
