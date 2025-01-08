using System;
using DotNetEnv;
using Godot;
using SQLite;
using Tabloulet.DatabaseNS;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Scenes.BuilderNS;
using BaseComponent = Tabloulet.Scenes.Components.BaseNS.Base;
using ImageComponent = Tabloulet.Scenes.Components.ImageNS.Image;
using ImageModel = Tabloulet.DatabaseNS.Models.Image;
using TextComponent = Tabloulet.Scenes.Components.TextNS.Text;
using TextModel = Tabloulet.DatabaseNS.Models.Text;
using VideoComponent = Tabloulet.Scenes.Components.VideoNS.Video;
using VideoModel = Tabloulet.DatabaseNS.Models.Video;

namespace Tabloulet.Scenes
{
    public partial class ScenarioLoader(Database database, IDisplay display)
    {
        private Guid _currentPage;

        public Guid LoadScenario(Guid idScenario)
        {
            Scenario scenario = database.GetById<Scenario>(idScenario);
            Page firstPage = database.GetById<Page>(scenario.PageId);

            LoadPage(firstPage);

            return firstPage.Id;
        }

        public void LoadPage(Page page)
        {
            _currentPage = page.Id;

            Control pageControl =
                new() { Name = page.Id.ToString(), Size = display.GetDisplayRoot().GetRect().Size };

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
            display.AddPage(pageControl);

            LoadComponents<TextModel>(page.Id);
            LoadComponents<ImageModel>(page.Id);
            LoadComponents<VideoModel>(page.Id);
        }

        private void LoadComponents<T>(Guid pageId)
            where T : IDatabaseModelComponent, new()
        {
            TableQuery<T> query = database.GetTableComponentsByPageId<T>(pageId);

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
                    case VideoModel video:
                        CreateVideoComponent(video);
                        break;
                }
            }
        }

        private static BaseComponent CreateBase(Control node, bool isMovable, IDisplay display)
        {
            if (display is Builder)
            {
                return new BaseComponent(node, isMovable, true, display as Builder);
            }

            return new BaseComponent(node, isMovable, false, null);
        }

        public void CreateTextComponent(TextModel text)
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
                    text.ZIndex,
                    text.IsMovable
                );
            BaseComponent textBase = CreateBase(textComponent, text.IsMovable, display);
            display.AddComponent(_currentPage, text.Id, textBase);
        }

        public void CreateImageComponent(ImageModel image)
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
                    image.ZIndex,
                    image.IsMovable
                );
            BaseComponent imageBase = CreateBase(imageComponent, image.IsMovable, display);
            display.AddComponent(_currentPage, image.Id, imageBase);
        }

        public void CreateVideoComponent(VideoModel video)
        {
            PackedScene videoPacked = GD.Load<PackedScene>(
                "res://Scenes/Components/Video/Video.tscn"
            );
            VideoComponent videoComponent = (VideoComponent)videoPacked.Instantiate();
            BaseComponent videoBase = CreateBase(videoComponent, video.IsMovable, display);
            videoComponent.ScaleX = video.ScaleX;
            videoComponent.ScaleY = video.ScaleY;
            videoComponent.SizeX = video.SizeX;
            videoComponent.SizeY = video.SizeY;
            videoComponent.PositionX = video.PositionX;
            videoComponent.PositionY = video.PositionY;
            videoComponent.RotationDeg = video.Rotation;
            videoComponent.Index = video.ZIndex;
            videoComponent.AutoplayVideo = video.Autoplay;
            videoComponent.LoopVideo = video.Loop;
            display.AddComponent(_currentPage, video.Id, videoBase);
            videoComponent.Path = video.Path;
            videoComponent.IsMovable = video.IsMovable;
        }
    }
}
