using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.BuilderNS;
using Tabloulet.Scenes.ViewerNS;

namespace Tabloulet.Scenes.HomeNS
{
    public partial class Home : Control
    {
        private Database _database;
        private List<Godot.Button> _buttons = [];
        private Action deleteAction;
        public Label labelRFID;

        public override void _Ready()
        {
            _database = GetNode<Database>("/root/Database");
            AddScenarioButtons();

            // Add the event to buttons
            Godot.Button cancelButton = GetNode<Godot.Button>(
                "MarginPopUp/PanelPopUp/MarginInsidePopUp/VBoxPopUp/MarginButtonPopUp/HBoxPopUp/CancelButton"
            );
            cancelButton.Pressed += ClosePopUp;

            Godot.Button createButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/CreateButton");
            createButton.Pressed += DisplayPopUpCreateScenario;

            Godot.Button cancelCreation = GetNode<Godot.Button>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/HBoxCreate/CancelCreationButton"
            );
            cancelCreation.Pressed += ClosePopUpCreateScenario;

            Godot.Button validateCreationButton = GetNode<Godot.Button>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/HBoxCreate/ValidateCreationButton"
            );
            validateCreationButton.Pressed += ValidateCreation;

            Godot.Button cancelDelete = GetNode<Godot.Button>(
                "MarginDelete/PanelDelete/MarginInsideDelete/VBoxDelete/HBoxDelete/CancelDeleteButton"
            );
            cancelDelete.Pressed += ClosePopUpDelete;

            Godot.Button adminButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/AdminButton");
            adminButton.Pressed += DisplayAdmin;

            labelRFID = GetNode<Label>("HBoxContainerRFID/LabelRFID");
        }

        // Add the scenario buttons to the list
        public void AddScenarioButtons()
        {
            VBoxContainer listScenarioNode = GetNode<VBoxContainer>(
                "MarginList/ScrollList/MarginScroll/ListScenario"
            );
            SQLite.TableQuery<Scenario> scenarios = _database.GetAllByType<Scenario>();

            // Create a theme for the buttons
            Theme theme = new() { DefaultFontSize = 35 };

            //Create a styleBox for the buttons
            StyleBoxFlat styleBox =
                new()
                {
                    CornerRadiusTopLeft = 12,
                    CornerRadiusTopRight = 12,
                    CornerRadiusBottomLeft = 12,
                    CornerRadiusBottomRight = 12,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BgColor = new Color(0.126f, 0.286f, 0.412f),
                };

            // Create a styleBox for the buttons
            StyleBoxFlat normalStyleBox = new() { BgColor = new Color(1, 1, 1, 0) };

            foreach (Scenario scenario in scenarios)
            {
                // Create a button to display the scenario
                Godot.Button scenarioButton =
                    new()
                    {
                        Text = scenario.Name,
                        Name = "ScenarioButton" + scenario.Id,
                        CustomMinimumSize = new Vector2(0, 90),
                        Theme = theme,
                        ClipText = true,
                    };
                scenarioButton.Pressed += () => DisplayPopUpScenario(scenario.Id.ToString());

                // Add StyleBox to the button
                scenarioButton.AddThemeStyleboxOverride("normal", styleBox);
                scenarioButton.AddThemeStyleboxOverride("hover", styleBox);
                scenarioButton.AddThemeStyleboxOverride("pressed", styleBox);

                // Create a margin container
                MarginContainer marginContainer = new() { Name = "MarginContainer" };
                marginContainer.AddThemeConstantOverride("margin_right", 30);
                marginContainer.SetAnchorsPreset(Control.LayoutPreset.FullRect);

                // Create a HBoxContainer to align the buttons
                HBoxContainer hBoxContainer =
                    new() { Alignment = BoxContainer.AlignmentMode.End, Name = "HBoxContainer" };
                hBoxContainer.AddThemeConstantOverride("separation", 10);

                // Create the edit button
                Godot.Button editButton =
                    new()
                    {
                        CustomMinimumSize = new Vector2(60, 60),
                        Name = "EditButton" + scenario.Id,
                        Visible = false,
                    };
                Texture2D EditButtonIcon = (Texture2D)
                    ResourceLoader.Load("res://Assets/Home/EditLogo.png");
                editButton.Icon = EditButtonIcon;
                editButton.AddThemeConstantOverride("icon_max_width", 60);
                editButton.AddThemeStyleboxOverride("normal", normalStyleBox);
                editButton.AddThemeStyleboxOverride("hover", normalStyleBox);
                editButton.AddThemeStyleboxOverride("pressed", normalStyleBox);
                editButton.Pressed += () => SwitchToBuilder(scenario.Id);
                editButton.FocusMode = Control.FocusModeEnum.None;
                _buttons.Add(editButton);

                // Create the delete button
                Godot.Button deleteButton =
                    new()
                    {
                        CustomMinimumSize = new Vector2(60, 60),
                        Name = "DeleteButton" + scenario.Id,
                        Visible = false,
                    };
                //deleteButton.Visible = false;
                Texture2D deleteButtonIcon = (Texture2D)
                    ResourceLoader.Load("res://Assets/Home/DeleteLogo.png");
                deleteButton.Icon = deleteButtonIcon;
                deleteButton.AddThemeConstantOverride("icon_max_width", 60);
                deleteButton.AddThemeStyleboxOverride("normal", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("hover", normalStyleBox);
                deleteButton.AddThemeStyleboxOverride("pressed", normalStyleBox);
                deleteButton.Pressed += () => DisplayPopUpDelete(scenario.Id);
                deleteButton.FocusMode = Control.FocusModeEnum.None;
                _buttons.Add(deleteButton);

                // Add the children to the parent nodes
                hBoxContainer.AddChild(editButton);
                hBoxContainer.AddChild(deleteButton);
                marginContainer.AddChild(hBoxContainer);
                scenarioButton.AddChild(marginContainer);
                listScenarioNode.AddChild(scenarioButton);
            }
        }

        // Change the page into the admin page
        public void ChangeToAdmin()
        {
            foreach (Godot.Button button in _buttons)
            {
                button.Visible = true;
            }
            Godot.Button createButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/CreateButton");
            createButton.Visible = true;

            Godot.Button adminButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/AdminButton");
            Texture2D adminButtonIcon = (Texture2D)
                ResourceLoader.Load("res://Assets/Home/AdminLogoutLogo.png");
            adminButton.Icon = adminButtonIcon;
            adminButton.Pressed -= DisplayAdmin;
            adminButton.Pressed += ChangeToUser;
        }

        // Change the page into the user page
        public void ChangeToUser()
        {
            foreach (Godot.Button button in _buttons)
            {
                button.Visible = false;
            }
            Godot.Button createButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/CreateButton");
            createButton.Visible = false;

            Godot.Button adminButton = GetNode<Godot.Button>("MarginAdmin/HBoxAdmin/AdminButton");
            Texture2D adminButtonIcon = (Texture2D)
                ResourceLoader.Load("res://Assets/Home/AdminLogo.png");
            adminButton.Icon = adminButtonIcon;
            adminButton.Pressed -= ChangeToUser;
            adminButton.Pressed += DisplayAdmin;
            CloseAdmin();
        }

        // Display the pop up with the scenario description
        public void DisplayPopUpScenario(String idScenario)
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

            Godot.Button loadButton = GetNode<Godot.Button>(
                "MarginPopUp/PanelPopUp/MarginInsidePopUp/VBoxPopUp/MarginButtonPopUp/HBoxPopUp/LoadButton"
            );
            loadButton.Pressed += () => SwitchToViewer(Guid.Parse(idScenario));
        }

        // Close the pop up with the scenario description
        public void ClosePopUp()
        {
            MarginContainer marginPopUp = GetNode<MarginContainer>("MarginPopUp");
            marginPopUp.Visible = false;
        }

        // Display the pop up to create a scenario
        public void DisplayPopUpCreateScenario()
        {
            MarginContainer marginCreate = GetNode<MarginContainer>("MarginCreate");
            marginCreate.Visible = true;
        }

        // Close the pop up to create a scenario
        public void ClosePopUpCreateScenario()
        {
            Label ErrorCreate = GetNode<Label>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/ErrorCreate"
            );
            ErrorCreate.Visible = false;
            MarginContainer marginCreate = GetNode<MarginContainer>("MarginCreate");
            marginCreate.Visible = false;
        }

        // Validate the creation of a scenario
        public void ValidateCreation()
        {
            TextEdit nameEdit = GetNode<TextEdit>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/TextEditName"
            );
            TextEdit descriptionEdit = GetNode<TextEdit>(
                "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/TextEditDescription"
            );

            if (nameEdit.Text == "" || descriptionEdit.Text == "")
            {
                Label ErrorCreate = GetNode<Label>(
                    "MarginCreate/PanelCreate/MarginInsideCreate/VBoxCreate/ErrorCreate"
                );
                ErrorCreate.Visible = true;
            }
            else
            {
                Page page = new() { Id = Guid.NewGuid(), Name = "Accueil" };
                Scenario scenario =
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = nameEdit.Text,
                        Description = descriptionEdit.Text,
                        PageId = page.Id,
                    };
                ScenarioPage scenarioPage =
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ScenarioId = scenario.Id,
                        PageId = page.Id,
                    };

                _database.Insert(page);
                _database.Insert(scenario);
                _database.Insert(scenarioPage);

                string directoryPath = Path.Combine(Constants.AppPath, scenario.Id.ToString());
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                SwitchToBuilder(scenario.Id);
            }
        }

        private void SwitchToBuilder(Guid id)
        {
            PackedScene builderScene = GD.Load<PackedScene>("res://Scenes/Builder/Builder.tscn");
            Builder builder = (Builder)builderScene.Instantiate();
            GetTree().Root.AddChild(builder);
            builder.Init(id);
            QueueFree();
        }

        private void SwitchToViewer(Guid id)
        {
            PackedScene viewerScene = GD.Load<PackedScene>("res://Scenes/Viewer/Viewer.tscn");
            Viewer viewer = (Viewer)viewerScene.Instantiate();
            GetTree().Root.AddChild(viewer);
            viewer.Init(id);
            QueueFree();
        }

        // Display the pop up to delete a scenario
        public void DisplayPopUpDelete(Guid scenarioId)
        {
            MarginContainer marginDelete = GetNode<MarginContainer>("MarginDelete");
            marginDelete.Visible = true;
            Godot.Button validateDelete = GetNode<Godot.Button>(
                "MarginDelete/PanelDelete/MarginInsideDelete/VBoxDelete/HBoxDelete/ValidateDeleteButton"
            );
            if (this.deleteAction != null)
            {
                validateDelete.Pressed -= this.deleteAction;
            }
            Action deleteAction = () => DeleteScenario(scenarioId);
            this.deleteAction = deleteAction;
            validateDelete.Pressed += deleteAction;
        }

        // Close the pop up to delete a scenario
        public void ClosePopUpDelete()
        {
            MarginContainer marginDelete = GetNode<MarginContainer>("MarginDelete");
            marginDelete.Visible = false;
        }

        // Delete a scenario
        public void DeleteScenario(Guid scenarioId)
        {
            List<Page> pages = _database.GetPagesByScenario(scenarioId);
            List<IDatabaseModel> elements = [];
            foreach (Page page in pages)
            {
                elements.AddRange(_database.GetElementsByPage(page.Id));
            }
            // Delete all the elements of the scenario
            foreach (IDatabaseModel element in elements)
            {
                _database.Delete(element);
            }
            // Delete the pages of the scenario, but not the first one
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
            // Delete the first page and the scenario
            _database.Delete(firstScenarioPage);
            _database.Delete(scenario);
            _database.Delete(firstPage);
            // Delete the buttons
            Godot.Button button = GetNodeOrNull<Godot.Button>(
                "MarginList/ScrollList/MarginScroll/ListScenario/ScenarioButton" + scenarioId
            );
            Godot.Button editButton = button.GetNode<Godot.Button>(
                "MarginContainer/HBoxContainer/EditButton" + scenarioId
            );
            Godot.Button deleteButton = button.GetNode<Godot.Button>(
                "MarginContainer/HBoxContainer/DeleteButton" + scenarioId
            );
            _buttons.Remove(editButton);
            _buttons.Remove(deleteButton);
            button.QueueFree();
            ClosePopUpDelete();
        }

        // Display the admin login panel
        public void DisplayAdmin()
        {
            Control loginPanel = GetNode<Control>("LoginPanel");
            loginPanel.Visible = true;
        }

        // Close the admin login panel
        public void CloseAdmin()
        {
            Control loginPanel = GetNode<Control>("LoginPanel");
            loginPanel.Visible = false;
        }

        public override void _Process(double delta) { }
    }
}
