using Godot;
using System;

/**
 * @class Play
 * @brief Main gameplay scene controller.
 * 
 * This class handles transitions from gameplay to the pause menu.
 */
public partial class Play : Node2D
{
    /**
	 * @brief Opens the pause menu.
	 * 
	 * Switches the current scene to the pause menu scene.
	 */
    private void pauza()
    {
        GetTree().ChangeSceneToFile("res://pauza.tscn");
    }
}