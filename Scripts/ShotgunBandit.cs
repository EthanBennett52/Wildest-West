using Godot;
using System;

public class ShotgunBandit : Bandit
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        health = 150;
        speed = 135;
        ((Shotgun)weapon).spreadAngle = Math.PI/6;
        weapon.damage = 10;
        weapon.soundEffectVolume(-17);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
