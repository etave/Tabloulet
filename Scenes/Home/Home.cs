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
        private List<Godot.Button> buttons = [];

        public override void _Ready()
        {
            _database = GetNode<Database>("/root/Database");
            addScenarioButtons();

            // Add the event to buttons
            Godot.Button cancelButton = GetNode<Godot.Button>(
                "MarginPopUp/PanelPopUp/MarginInsidePopUp/VBoxPopUp/MarginButtonPopUp/HBoxPopUp/CancelButton"
            );
            cancelButton.Pressed += closePopUp;

            Godot.Button createButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/CreateButton");
            createButton.Pressed += displayPopUpCreateScenario;

            Godot.Button cancelCreation = GetNode<Godot.Button>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/HBoxCreate/CancelCreationButton"
            );
            cancelCreation.Pressed += closePopUpCreateScenario;

            Godot.Button validateCreationButton = GetNode<Godot.Button>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/HBoxCreate/ValidateCreationButton"
            );
            validateCreationButton.Pressed += validateCreation;

            Godot.Button cancelDelete = GetNode<Godot.Button>(
                "MarginDelete/PanelDelete/MarginInsideDelete/VBoxDelete/HBoxDelete/CancelDeleteButton"
            );
            cancelDelete.Pressed += closePopUpDelete;
        }

        // Add the scenario buttons to the list
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
                scenarioButton.Name = "ScenarioButton" + scenario.Id;
                scenarioButton.CustomMinimumSize = new Vector2(0, 90);
                scenarioButton.Theme = theme;
                scenarioButton.ClipText = true;
                scenarioButton.Pressed += () => displayPopUpScenario(scenario.Id.ToString());

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
                //deleteButton.Visible = false;
                Texture2D deleteButtonIcon = (Texture2D)
                    ResourceLoader.Load("res://Assets/Home/DeleteLogo.png");
                deleteButton.Icon = deleteButtonIcon;
                deleteButton.AddThemeConstantOverride("icon_max_width", 60);
                deleteButton.AddThemeStyleboxOverride("normal", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("hover", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("pressed", normalStyleBox);
                deleteButton.Pressed += () => displayPopUpDelete(scenario.Id);
                buttons.Add(deleteButton);

                // Add the children to the parent nodes
                hBoxContainer.AddChild(editButton);
                hBoxContainer.AddChild(deleteButton);
                marginContainer.AddChild(hBoxContainer);
                scenarioButton.AddChild(marginContainer);
                listScenarioNode.AddChild(scenarioButton);
            }
        }

        // Change the page into the admin page
        public void changeToAdmin()
        {
            foreach (Godot.Button button in buttons)
            {
                button.Visible = true;
            }
            Godot.Button createButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/CreateButton");
            createButton.Visible = true;
        }

        // Display the pop up with the scenario description
        public void displayPopUpScenario(String idScenario)
        {
            Label title = GetNode<Label>("MarginPopUp/PanelPopUp/TitlePopUp");
            Label description = GetNode<Label>(
                "MarginPopUp/PanelPopUp/MarginInsidePopUp/VBoxPopUp/DescriptionPopUp"
            );
            Scenario scenario = (Scenario)_database.GetById<Scenario>(Guid.Parse(idScenario));
            title.Text = scenario.Name;
            description.Text = scenario.Description;

            MarginContainer marginPopUp = GetNode<MarginContainer>("MarginPopUp");
            marginPopUp.Visible = true;
        }

        // Close the pop up with the scenario description
        public void closePopUp()
        {
            MarginContainer marginPopUp = GetNode<MarginContainer>("MarginPopUp");
            marginPopUp.Visible = false;
        }

        // Display the pop up to create a scenario
        public void displayPopUpCreateScenario()
        {
            MarginContainer marginCreate = GetNode<MarginContainer>("MarginCreate");
            marginCreate.Visible = true;
        }

        // Close the pop up to create a scenario
        public void closePopUpCreateScenario()
        {
            MarginContainer marginCreate = GetNode<MarginContainer>("MarginCreate");
            marginCreate.Visible = false;
        }

        // Validate the creation of a scenario
        public void validateCreation()
        {
            TextEdit nameEdit = GetNode<TextEdit>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/TextEditName"
            );
            TextEdit descriptionEdit = GetNode<TextEdit>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/TextEditDescription"
            );

            Page page = new Page();
            page.Id = Guid.NewGuid();
            Scenario scenario = new Scenario();
            scenario.Id = Guid.NewGuid();
            scenario.Name = nameEdit.Text;
            scenario.Description = descriptionEdit.Text;
            scenario.PageId = page.Id;
            ScenarioPage scenarioPage = new ScenarioPage();
            scenarioPage.Id = Guid.NewGuid();
            scenarioPage.ScenarioId = scenario.Id;
            scenarioPage.PageId = page.Id;
            GD.Print(scenarioPage.ScenarioId);

            _database.Insert(page);
            _database.Insert(scenario);
            _database.Insert(scenarioPage);
            closePopUpCreateScenario();
        }

        // Display the pop up to delete a scenario
        public void displayPopUpDelete(Guid scenarioId)
        {
            MarginContainer marginDelete = GetNode<MarginContainer>("MarginDelete");
            marginDelete.Visible = true;
            Godot.Button validateDelete = GetNode<Godot.Button>(
                "MarginDelete/PanelDelete/MarginInsideDelete/VBoxDelete/HBoxDelete/ValidateDeleteButton"
            );
            validateDelete.Pressed += () => deleteScenario(scenarioId);
        }

        // Close the pop up to delete a scenario
        public void closePopUpDelete()
        {
            MarginContainer marginDelete = GetNode<MarginContainer>("MarginDelete");
            marginDelete.Visible = false;
        }

        // Delete a scenario
        public void deleteScenario(Guid scenarioId)
        {
            List<Page> pages = _database.GetPagesByScenario(scenarioId);
            List<IDatabaseModel> elements = [];
            foreach (Page page in pages)
            {
                elements.AddRange(_database.GetElementsByPage(page.Id));
            }
            foreach (IDatabaseModel element in elements)
            {
                _database.Delete(element);
            }
            Scenario scenario = (Scenario)_database.GetById<Scenario>(scenarioId);
            Page firstPage = _database.GetById<Page>(scenario.PageId);
            ScenarioPage firstScenarioPage = _database.GetScenarioPageByPage(firstPage.Id);
            foreach (Page page in pages)
            {
                if (page.Id == firstPage.Id)
                {
                    continue;
                }
                ScenarioPage scenarioPage = _database.GetScenarioPageByPage(page.Id);
                _database.Delete(scenarioPage);
                _database.Delete(page);
            }
            _database.Delete(firstScenarioPage);
            _database.Delete(scenario);
            _database.Delete(firstPage);
            Godot.Button button = GetNode<Godot.Button>(
                "MarginList/ScrollList/MarginScroll/ListScenario/ScenarioButton" + scenarioId
            );
            button.QueueFree();
            closePopUpDelete();
        }

        public override void _Process(double delta) { }
    }
}
