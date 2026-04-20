namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class InvertColorCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public InvertColorCommand()
            : base(displayName: "Invert Color", description: "Flip to the complementary opposite color", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            var newHue = (this._engine.Hue + 180) % 360;
            this._engine.SetColor(newHue, this._engine.Saturation, this._engine.Lightness);
            PluginLog.Info($"Inverted color to hue {newHue}");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
