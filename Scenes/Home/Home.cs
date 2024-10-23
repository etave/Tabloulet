using System;
using System.Collections.Generic;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;

namespace Tabloulet.Scenes.HomeNS
{
    public partial class Home : Control
    {
        private Database _database;

        private List<Godot.Button> buttons = new List<Godot.Button>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Hello, World!");
            _database = GetNode<Database>("/root/Database");
            addScenarioButtons();
        }

        public void addScenarioButtons()
        {
            VBoxContainer listScenarioNode = GetNode<VBoxContainer>(
                "MarginList/ScrollList/MarginScroll/ListScenario"
            );
            SQLite.TableQuery<Scenario> scenarios = _database.GetAllByType<Scenario>();

            // Create a theme for the buttons
            Theme theme = new() { DefaultFontSize = 35 };

            //Create a styleBox for the buttons
            StyleBoxFlat styleBox = new StyleBoxFlat();
            styleBox.CornerRadiusTopLeft = 12;
            styleBox.CornerRadiusTopRight = 12;
            styleBox.CornerRadiusBottomLeft = 12;
            styleBox.CornerRadiusBottomRight = 12;
            styleBox.BorderWidthTop = 0;
            styleBox.BorderWidthBottom = 0;
            styleBox.BorderWidthLeft = 0;
            styleBox.BorderWidthRight = 0;
            styleBox.BgColor = new Color(0.126f, 0.286f, 0.412f);

            // Create a styleBox for the buttons
            StyleBoxFlat normalStyleBox = new StyleBoxFlat();
            normalStyleBox.BgColor = new Color(1, 1, 1, 0);

            foreach (Scenario scenario in scenarios)
            {
                // Create a button to display the scenario
                Godot.Button scenarioButton = new Godot.Button();
                scenarioButton.Text = scenario.Name;
                scenarioButton.CustomMinimumSize = new Vector2(0, 90);
                scenarioButton.Theme = theme;
                scenarioButton.ClipText = true;

                // Add StyleBox to the button
                scenarioButton.AddThemeStyleboxOverride("normal", styleBox);
                scenarioButton.AddThemeStyleboxOverride("hover", styleBox);
                scenarioButton.AddThemeStyleboxOverride("pressed", styleBox);

                // Create a margin container
                MarginContainer marginContainer = new MarginContainer();
                marginContainer.AddThemeConstantOverride("margin_right", 30);
                marginContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);

                // Create a HBoxContainer to align the buttons
                HBoxContainer hBoxContainer = new HBoxContainer();
                hBoxContainer.Alignment = BoxContainer.AlignmentMode.End;
                hBoxContainer.AddThemeConstantOverride("separation", 10);

                // Create the edit button
                Godot.Button editButton = new Godot.Button();
                editButton.CustomMinimumSize = new Vector2(60, 60);
                editButton.Name = "EditButton" + scenario.Id;
                editButton.Visible = false;
                Texture2D EditButtonIcon = (Texture2D)
                    ResourceLoader.Load("res://Assets/Home/EditLogo.png");
                editButton.Icon = EditButtonIcon;
                editButton.AddThemeConstantOverride("icon_max_width", 60);
                editButton.AddThemeStyleboxOverride("normal", normalStyleBox);
                editButton.AddThemeStyleboxOverride("hover", normalStyleBox);
                editButton.AddThemeStyleboxOverride("pressed", normalStyleBox);
                buttons.Add(editButton);

                // Create the delete button
                Godot.Button deleteButton = new Godot.Button();
                deleteButton.CustomMinimumSize = new Vector2(60, 60);
                deleteButton.Name = "DeleteButton" + scenario.Id;
                deleteButton.Visible = false;
                Texture2D deleteButtonIcon = (Texture2D)
                    ResourceLoader.Load("res://Assets/Home/DeleteLogo.png");
                deleteButton.Icon = deleteButtonIcon;
                deleteButton.AddThemeConstantOverride("icon_max_width", 60);
                deleteButton.AddThemeStyleboxOverride("normal", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("hover", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("pressed", normalStyleBox);
                buttons.Add(deleteButton);

                // Add the children to the parent nodes
                hBoxContainer.AddChild(editButton);
                hBoxContainer.AddChild(deleteButton);
                marginContainer.AddChild(hBoxContainer);
                scenarioButton.AddChild(marginContainer);
                listScenarioNode.AddChild(scenarioButton);
            }
        }

        public void changeToAdmin()
        {
            foreach (Godot.Button button in buttons)
            {
                button.Visible = true;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta) { }
    }
}
