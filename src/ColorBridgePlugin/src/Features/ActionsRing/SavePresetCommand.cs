namespace Loupedeck.ColorBridgePlugin.Features.ActionsRing
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    public class SavePresetCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public SavePresetCommand()
            : base(displayName: "Save Preset", description: "Save current color and scheme as a preset", groupName: "1. Actions Ring")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            PresetStorage.Save(
                this._engine.Hue,
                this._engine.Saturation,
                this._engine.Lightness,
                this._engine.CurrentScheme);
            PluginLog.Info("Saved preset from Actions Ring");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => null;
    }
}
