using Godot;
using System;

public class Bandit : KinematicBody2D
{
    private int health = 100;

    public void takeDamage(int damage){
        health = health - damage;
    }

    private void onDeath(){
        QueueFree();
    }
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      if (health <= 0) {
          onDeath();
      }
  }
}
