namespace Tabloulet.Scenes.Components
{
    internal interface IComponent
    {
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        float SizeX { get; set; }
        float SizeY { get; set; }
        float PositionX { get; set; }
        float PositionY { get; set; }
        float RotationDeg { get; set; }
        void UpdateSizePositionRotationParameters(
            float scaleX,
            float scaleY,
            float sizeX,
            float sizeY,
            float positionX,
            float positionY,
            float rotationDeg
        );
    }
}
