namespace Loupedeck.ColorBridgePlugin
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;

    /// <summary>
    /// ColorBridge — Turn your MX Creative Console into a Color Studio.
    /// Universal plugin for color management across all applications.
    /// </summary>
    public class ColorBridgePlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        public override void Load()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);

            PresetStorage.Init(this.GetPluginDataDirectory());

            var engine = ColorEngine.Instance;
            PluginLog.Info($"ColorBridge loaded — Hue: {engine.Hue}°, Scheme: {engine.CurrentScheme}");
        }

        public override void Unload()
        {
            PluginLog.Info("ColorBridge unloaded");
        }
    }
}
