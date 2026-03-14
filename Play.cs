using Godot;
using System;

public partial class Play : Node2D
{
	
	private void pauza(){
		GetTree().ChangeSceneToFile("res://pauza.tscn");
	}
}
