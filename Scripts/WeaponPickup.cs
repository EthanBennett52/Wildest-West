using Godot;
using System;

public class WeaponPickup : Area2D
{
    public Gun weapon;
    private Sprite weaponSprite;
    private Sprite keySprite;
    private Player player;
    private bool playerInRange = false;

    public void setWeapon(Gun weapon) {
        AddChild(weapon);
        this.weapon = weapon;
        weaponSprite.Texture = weapon.sprite.Texture;
    }

    private void OnBodyEntered(Node area){
        if (area is Player){
            player = (Player)area;
            playerInRange = true;
            keySprite.Show();
        }
    }

    private void OnBodyExited(Node area){
        if (area is Player){
            playerInRange = false;
            keySprite.Hide();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        weaponSprite = (Sprite)FindNode("WeaponSprite");
        keySprite = (Sprite)FindNode("KeySprite");
        //keySprite.Hide();
        Connect("body_entered", this, "OnBodyEntered");
        Connect("body_exited", this, "OnBodyExited");


        //test
        PackedScene machineGun = GD.Load<PackedScene>("res://Scenes/MachineGun.tscn");
        Gun testGun = machineGun.Instance<Gun>();
		setWeapon(testGun);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      if (playerInRange && Input.IsActionJustPressed("pickup")){
          RemoveChild(weapon);
          player.pickupWeapon(weapon);
          QueueFree();
      }

  }
}
