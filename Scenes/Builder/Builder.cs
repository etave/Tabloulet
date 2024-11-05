using System;
using System.Collections.Generic;
using Godot;
using SQLite;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;
using Tabloulet.Scenes.BuilderNS.ComponentsPanelNS;
using Base = Tabloulet.Scenes.Components.BaseNS.Base;
using Button = Godot.Button;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using TextModel = Tabloulet.DatabaseNS.Models.Text;

namespace Tabloulet.Scenes.BuilderNS
{
    public partial class Builder : Control
    {
        private Database _database;

        private Guid _idScenario;
        private Guid _currentPage;

        private Control _blueprint;
        public ComponentsPanel componentsPanel;
        private Button _exitButton;

        private Button _addTextButton;
        private Button _addImageButton;

        private Dictionary<Guid, Dictionary<Guid, Control>> _pages;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            _database = GetNode<Database>("/root/Database");

            _blueprint = GetNode<Control>("Blueprint");

            componentsPanel = GetNode<ComponentsPanel>("ComponentsPanel");
            _exitButton = GetNode<Button>("ExitPanel/MarginContainer/Button");

            _exitButton.Pressed += ExitButtonPressed;

            _addTextButton = componentsPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/TextMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );
            _addImageButton = componentsPanel.GetNode<Button>(
                "OpenPanel/VBoxContainer/ImageMarginContainer/PanelContainer/GridContainer/MarginContainer/Button"
            );

            _addTextButton.Pressed += AddTextButtonPressed;
            _addImageButton.Pressed += AddImageButtonPressed;

            _pages = [];
        }

        public void Init(Guid idScenario)
        {
            this._idScenario = idScenario;
            LoadScenario();
        }

        private void LoadScenario()
        {
            Scenario scenario = _database.GetById<Scenario>(_idScenario);
            Page firstPage = _database.GetById<Page>(scenario.PageId);
            _currentPage = firstPage.Id;

            LoadPage(firstPage);
        }

        private void LoadPage(Page page)
        {
            Control pageControl =
                new() { Name = page.Id.ToString(), Size = _blueprint.GetRect().Size };
            ColorRect background =
                new()
                {
                    Name = "Background",
                    Color = new Color("#FFFFFF"),
                    AnchorRight = 1,
                    AnchorBottom = 1,
                };
            if (page.BackgroundColor != null)
            {
                background.Color = new Color(page.BackgroundColor);
            }
            pageControl.AddChild(background);
            _blueprint.AddChild(pageControl);

            LoadComponents<TextModel>(page.Id);
            LoadComponents<ImageModel>(page.Id);
        }

        private void LoadComponents<T>(Guid pageId)
            where T : IDatabaseModelComponent, new()
        {
            TableQuery<T> query = _database.GetTableComponentsByPageId<T>(pageId);

            foreach (T component in query)
            {
                switch (component)
                {
                    case TextModel text:
                        CreateTextComponent(text);
                        break;
                    case ImageModel image:
                        CreateImageComponent(image);
                        break;
                }
            }
        }

        private void AddTextButtonPressed()
        {
            TextModel text =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = _currentPage,
                    Content = $"[color=#000000]{LoremNET.Lorem.Words(30, true)}[/color]",
                    Font = null,
                    FontSize = 20,
                    ScaleX = 1,
                    ScaleY = 1,
                    SizeX = 200,
                    SizeY = 200,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    IsMovable = true,
                };
            CreateTextComponent(text);
            _database.Insert(text);
        }

        private void AddImageButtonPressed()
        {
            ImageModel image =
                new()
                {
                    Id = Guid.NewGuid(),
                    PageId = _currentPage,
                    Path = "",
                    ScaleX = 1,
                    ScaleY = 1,
                    SizeX = 200,
                    SizeY = 200,
                    PositionX = GetRect().Size.X / 2,
                    PositionY = GetRect().Size.Y / 2,
                    Rotation = 0,
                    IsMovable = true,
                };
            CreateImageComponent(image);
            _database.Insert(image);
        }

        private static Base CreateBase(
            Control node,
            bool isMovable,
            bool inBuilderMode,
            Builder builder
        )
        {
            return new Base(node, isMovable, inBuilderMode, builder);
        }

        private void CreateTextComponent(TextModel text)
        {
            TextComponent textComponent =
                new(
                    text.Content,
                    text.Font,
                    text.FontSize,
                    text.ScaleX,
                    text.ScaleY,
                    text.SizeX,
                    text.SizeY,
                    text.PositionX,
                    text.PositionY,
                    text.Rotation,
                    text.IsMovable
                );
            Base textBase = CreateBase(textComponent, text.IsMovable, true, this);
            AddComponent(text.Id, textBase, textComponent);
        }

        private void CreateImageComponent(ImageModel image)
        {
            ImageComponent imageComponent =
                new(
                    image.Path,
                    image.ScaleX,
                    image.ScaleY,
                    image.SizeX,
                    image.SizeY,
                    image.PositionX,
                    image.PositionY,
                    image.Rotation,
                    image.IsMovable
                );
            Base imageBase = CreateBase(imageComponent, image.IsMovable, true, this);
            AddComponent(image.Id, imageBase, imageComponent);
        }

        private void AddComponent(Guid id, Base baseComponent, Control component)
        {
            _blueprint.AddChild(baseComponent);
            if (!_pages.ContainsKey(_currentPage))
            {
                _pages[_currentPage] = [];
            }
            _pages[_currentPage][id] = component;
        }

        private void ExitButtonPressed()
        {
            // TODO: Save and change scene before queue free
            QueueFree();
        }
    }
}
