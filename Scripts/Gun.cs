using Godot;
using System;

public class Gun : Node2D
{
    PackedScene BULLET = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
    Sprite sprite;
    double rotationOffset = -.2;

    private double fireRate = .25;

    private double shotTimer = 0;

    private Boolean canShoot = true;

    public Vector2 target;

    private Node2D parent;
    private Node2D player;

    private void fire(){
        Bullet bullet = (Bullet)BULLET.Instance(); 
            bullet.Position = new Vector2((float) (GlobalPosition.x + 50 * Math.Cos(Rotation + rotationOffset)), (float) (GlobalPosition.y + 50 * Math.Sin(Rotation + rotationOffset))); 
            bullet.RotationDegrees = RotationDegrees;
            bullet.creator = GetPath();
            GetTree().GetRoot().AddChild(bullet);
            canShoot = false;
            shotTimer = fireRate;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = (Sprite)GetChild(0);
        parent = GetParent<Node2D>();
        if (parent is Player){
            target = GetGlobalMousePosition();
        } else {
            player = (Node2D)GetNode("../../Player");
            target = player.GlobalPosition;
        }
        
    }

// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (shotTimer <= 0) {
            canShoot = true;
        } else {
            shotTimer -= delta;
        }
        if (parent is Player) {
            target = GetGlobalMousePosition();
            LookAt(target);
            if (Input.IsActionJustPressed("shoot") && canShoot) {
                fire();
            }
            
        } else {
            target = player.GlobalPosition;
            LookAt(target);
            //temp
            if (canShoot) {
                fire();
            }
        }
        

        if (270 > Math.Abs(RotationDegrees % 360) && 90 < Math.Abs(RotationDegrees % 360)) {
            sprite.FlipV = true;
            rotationOffset = .2;
        } else {
            sprite.FlipV = false;
            rotationOffset = -.2;
        }

        
        
    }
}
