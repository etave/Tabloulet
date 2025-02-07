using System;
using Godot;
using Tabloulet.Scenes.Components.BaseNS;

namespace Tabloulet.Scenes
{
    public interface IDisplay
    {
        Control GetDisplayRoot();
        void AddPage(Control page);
        void AddComponent(Guid idPage, Guid idComponent, Base baseComponent);
        void FreePage();
        void ChangePage(Guid idPage);
    }
}
