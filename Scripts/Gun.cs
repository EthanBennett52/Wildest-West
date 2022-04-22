using Godot;
using System;

public class Gun : Node2D
{
    PackedScene BULLET = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
    Sprite sprite;
    //for adjusting the bullet spawn location
    double rotationOffset = -.2;
    //How often the gun shoots
    protected double fireRate = .25;
    //How long it takes to reload (in seconds).
    protected double reloadTime = 2;
    //Cooldown timer between shots
    private double shotTimer = 0;
    //False if weapon is cooling down between shots
    private Boolean canShoot = true;
    //amount of ammo available
    public int ammo = 80;
    //number of bullets that can be loaded
    public int maxLoadedCapacity = 8;
    //Ammo loaded in the gun
    public int loaded;
    //Damage per bullet
    public int damage = 25;
    //where the gun is pointing at
    public Vector2 target;

    protected Node2D parent;
    protected Node2D player;

    //Fires the gun
    public void fire(){
        if (canShoot && loaded > 0){
            Bullet bullet = (Bullet)BULLET.Instance(); 
            bullet.Position = new Vector2((float) (GlobalPosition.x + 50 * Math.Cos(Rotation + rotationOffset)), (float) (GlobalPosition.y + 50 * Math.Sin(Rotation + rotationOffset))); 
            bullet.RotationDegrees = RotationDegrees;
            bullet.creator = GetPath();
            GetTree().GetRoot().AddChild(bullet);
            canShoot = false;
            shotTimer = fireRate;
            loaded--;
            
        } /*else if (loaded <= 0) {
            reload();
        }*/
        
    }

    //Reloads the gun
    public void reload(){
        shotTimer = reloadTime;
        canShoot = false;
        int diff = maxLoadedCapacity - loaded;
        if (ammo >= diff){
            loaded += diff;
            ammo -= diff;
        } else {
            loaded += ammo;
            ammo = 0;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        loaded = maxLoadedCapacity;
        sprite = (Sprite)GetChild(0);
        parent = GetParent<Node2D>();

        //Sets the target. There might be a better way to this without the redundant code in _Process.
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
        //The cooldown timer
        if (shotTimer <= 0) {
            canShoot = true;
        } else {
            shotTimer -= delta;
        }

        //Points the gun at the target
        if (parent is Player) {
            target = GetGlobalMousePosition();
            LookAt(target);
            
        } else {
            target = player.GlobalPosition;
            LookAt(target);
        }
        
        //Keeps the weapon sprite in the correct orientation
        if (270 > Math.Abs(RotationDegrees % 360) && 90 < Math.Abs(RotationDegrees % 360)) {
            sprite.FlipV = true;
            rotationOffset = .2;
        } else {
            sprite.FlipV = false;
            rotationOffset = -.2;
        }

        
        
    }
}
