using Godot;
using System;

/**
 * @class Pauza
 * @brief Controls the pause menu scene.
 * 
 * This class handles navigation from the pause menu back to the main menu.
 */
public partial class Pauza : Control
{
    /**
	 * @brief Returns to the main menu.
	 * 
	 * Switches the current scene to the main menu scene.
	 */
    private void menu()
    {
        GetTree().ChangeSceneToFile("res://menu.tscn");
    }
}