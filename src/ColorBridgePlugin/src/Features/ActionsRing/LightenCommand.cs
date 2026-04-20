namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class LightenCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public LightenCommand()
            : base(displayName: "Lighten", description: "Increase lightness by 10%", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.AdjustLightness(10);
            PluginLog.Info($"Lightened to {this._engine.Lightness}%");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
