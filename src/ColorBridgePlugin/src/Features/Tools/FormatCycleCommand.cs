namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Platform;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class FormatCycleCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public FormatCycleCommand()
            : base(displayName: "Format Convert", description: "Press to cycle output format (HEX, RGB, HSL, CMYK)", groupName: "2. Tools")
        {
            this._engine.FormatChanged += () => this.ActionImageChanged("");
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.CycleFormat();
            PluginLog.Info($"Format changed to {this._engine.ActiveFormat}");
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var iconName = this._engine.ActiveFormat switch
            {
                ColorEngine.ColorFormat.HEX => "Format HEX.png",
                ColorEngine.ColorFormat.RGB => "Format RGB.png",
                ColorEngine.ColorFormat.HSL => "Format HSL.png",
                ColorEngine.ColorFormat.CMYK => "Format CMYK.png",
                _ => "Format HEX.png"
            };

            return IconRenderer.RenderImageOnly(iconName);
        }
    }
}
