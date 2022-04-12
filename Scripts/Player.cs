using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export] public int speed = 300;

    private int maxHealth = 100;
    protected int health;
    ProgressBar healthBar;

    public Vector2 velocity = new Vector2();

    public void takeDamage(int damage){
        health = health - damage;
        healthBar.Value = health;
    }

    public void GetInput() {
        //LookAt(GetGlobalMousePosition());
        velocity = new Vector2();

        if (Input.IsActionPressed("right")){
            velocity.x += 1;
        }

        if (Input.IsActionPressed("left")) {
            velocity.x -= 1;
        }

        if (Input.IsActionPressed("up")){
            velocity.y -= 1;
        }

        if (Input.IsActionPressed("down")){
            velocity.y += 1;
        }
        
        velocity = velocity.Normalized() * speed;

    }
    
    public override void _Ready()
    {
        health = maxHealth;
        healthBar = GetNode<ProgressBar>("HealthBar");
        healthBar.Value = maxHealth;
        //Temp
        healthBar.Hide();
    }

    public override void _Process(float delta)
    {
        GetInput();
        velocity = MoveAndSlide(velocity);
    }
}
