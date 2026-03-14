using Godot;
using System;

public partial class Pauza : Control
{
	
	private void menu(){
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
}
