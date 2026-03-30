using Godot;
using System;
using System.Drawing;
using static Godot.TextServer;

/**
 * @class Enemy3
 * @brief Represents a ranged enemy that shoots projectiles at the player.
 * 
 * This class defines an enemy with very low health that attacks
 * the player from a distance using projectiles. It also handles
 * movement, respawning, and contact-based damage.
 */
public partial class Enemy3 : CharacterBody2D
{
    /**
	 * @brief Damage dealt to the player on contact.
	 */
    public int _damage = 5;

    /**
	 * @brief Maximum health of the enemy.
	 */
    public int max_health = 0;

    /**
	 * @brief Current health of the enemy.
	 */
    private int health;

    /**
	 * @brief Indicates whether the enemy is in contact with the player.
	 */
    public bool contact = false;

    /**
	 * @brief Time interval between damage ticks and shooting.
	 */
    public float damagetime = 2.0f;

    /**
	 * @brief Time before the enemy respawns.
	 */
    public float respawntime = 1f;

    /**
	 * @brief Marker used as the projectile spawn point.
	 */
    Marker2D point;

    /**
	 * @brief Packed scene for enemy projectiles.
	 */
    PackedScene projectileenemytscn = GD.Load<PackedScene>("res://projectileenemy.tscn");

    /**
	 * @brief Packed scene used to respawn this enemy.
	 */
    PackedScene enemy3scene;

    /**
	 * @brief Position where the enemy will respawn.
	 */
    Vector2 spawnposition;

    /**
	 * @brief Movement speed of the enemy (negative for retreating behavior).
	 */
    public const float Speed = -20.0f;

    /**
	 * @brief Reference to the player object.
	 */
    private CharacterBody2D player;

    /**
	 * @brief Initializes the enemy.
	 * 
	 * Sets health, loads scenes, assigns spawn position,
	 * retrieves player and marker references, and starts shooting.
	 * 
	 * @note Make sure required nodes exist:
	 * - "../player"
	 * - "point"
	 */
    public override void _Ready()
    {
        health = max_health;
        enemy3scene = GD.Load<PackedScene>("res://enemy3respawn.tscn");
        spawnposition = GetRandomPosition();
        player = GetNode<CharacterBody2D>("../player");
        point = GetNode<Marker2D>("point");

        CallDeferred(nameof(Shoot));
    }

    /**
	 * @brief Handles physics updates.
	 * 
	 * Moves the enemy relative to the player.
	 * Negative speed causes movement away from the player.
	 * 
	 * @param delta Time elapsed since last frame.
	 */
    public override void _PhysicsProcess(double delta)
    {
        if (player == null)
        {
            return;
        }

        Vector2 Direction = (player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = Direction * Speed;

        MoveAndSlide();
    }

    /**
	 * @brief Applies damage to the enemy.
	 * 
	 * Reduces health and triggers respawn when health reaches zero.
	 * The enemy is removed from the scene after spawning a new instance.
	 * 
	 * @param damage Amount of damage to apply.
	 */
    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            CallDeferred(nameof(SpawnEnemy3));
            QueueFree();
        }

        GD.Print(health);
    }

    /**
	 * @brief Handles collision with another body.
	 * 
	 * If the colliding object is a player, the enemy starts
	 * dealing damage over time until the contact ends.
	 * 
	 * @param body The colliding node.
	 */
    private async void BodyCollision(Node body)
    {
        if (body is Player player)
        {
            contact = true;

            while (contact)
            {
                await ToSignal(GetTree().CreateTimer(damagetime), "timeout");

                if (contact == true)
                    player.Damage(_damage);
            }
        }
    }

    /**
	 * @brief Handles when a body exits collision.
	 * 
	 * Stops dealing damage when the player is no longer in contact.
	 * 
	 * @param body The node leaving the collision.
	 */
    private void BodyCollisionOut(Node body)
    {
        if (body is Player player)
        {
            contact = false;
        }
    }

    /**
	 * @brief Spawns a new instance of Enemy3.
	 * 
	 * Instantiates a new enemy from the packed scene and places it
	 * at a predefined spawn position.
	 * 
	 * @note The scene "res://enemy3respawn.tscn" must exist.
	 */
    public void SpawnEnemy3()
    {
        var enemy3 = enemy3scene.Instantiate<CharacterBody2D>();
        enemy3.GlobalPosition = spawnposition;
        GetParent().AddChild(enemy3);

        GD.Print("Enemy3 respawned");
    }

    /**
	 * @brief Continuously shoots projectiles at the player.
	 * 
	 * Creates projectile instances at a fixed time interval
	 * and directs them toward the player's current position.
	 * 
	 * @note Requires a valid Marker2D ("point") as spawn origin.
	 */
    public async void Shoot()
    {
        while (true)
        {
            await ToSignal(GetTree().CreateTimer(damagetime), "timeout");

            if (player == null)
            {
                return;
            }

            Projectileenemy projectile = projectileenemytscn.Instantiate<Projectileenemy>();
            projectile.GlobalPosition = point.GlobalPosition;

            Vector2 direction = (player.GlobalPosition - point.GlobalPosition).Normalized();
            projectile.direction = direction;

            GetParent().AddChild(projectile);
        }
    }

    /**
	 * @brief Generates a random position within predefined bounds.
	 * 
	 * @return A Vector2 representing a random position.
	 */
    public Vector2 GetRandomPosition()
    {
        float x = GD.RandRange(175, 975);
        float y = GD.RandRange(190, 500);

        return new Vector2(x, y);
    }
}