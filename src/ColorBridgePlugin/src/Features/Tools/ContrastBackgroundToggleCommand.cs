namespace Loupedeck.ColorBridgePlugin.Features.Tools
{
    using System;
    using Loupedeck.ColorBridgePlugin.Engine;
    using Loupedeck.ColorBridgePlugin.Rendering;

    public class ContrastBackgroundToggleCommand : PluginDynamicCommand
    {
        private readonly ColorEngine _engine = ColorEngine.Instance;

        public ContrastBackgroundToggleCommand()
            : base(displayName: "Contrast Background", description: "Toggle light/dark background", groupName: "2. Tools")
        {
            this._engine.ContrastBackgroundChanged += () => this.ActionImageChanged("");
        }

        protected override void RunCommand(String actionParameter)
        {
            this._engine.ToggleContrastBackground();
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var isWhite = this._engine.CurrentContrastBackground == ColorEngine.ContrastBackground.White;
            var fileName = isWhite ? "ContrastLight.png" : "ContrastDark.png";
            return IconRenderer.RenderImageOnly(fileName);
        }
    }
}
