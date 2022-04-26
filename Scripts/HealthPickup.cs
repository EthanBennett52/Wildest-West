using Godot;
using System;

public class HealthPickup : Area2D
{
    //How much health this pickup contains
    private int healAmount = 10;

    //Called when the player picks this up
    private void OnPickup(Player player){
        player.heal(healAmount);
        QueueFree();
    }

    //Called when a Node enters this area
    private void OnBodyEntered(Node area){
        if (area is Player) {
            OnPickup((Player)area);
        }
    }


    public void setHealAmount(int amount){
        healAmount = amount;
    }
    
    public override void _Ready()
    {
        Connect("body_entered", this, "OnBodyEntered");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
