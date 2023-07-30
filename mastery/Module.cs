using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace mastery
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        private Image imageFishing;
        private Image imageSkiff;
        private Image imageWaypoint;

        // from https://search.gw2dat.com/
        private const int FISHING_ASSET_ID = 2594729;
        private const int SKIFF_ASSET_ID = 2593817;
        private const int WAYPOINT_ASSET_ID = 2595066;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {

        }

        protected override void Initialize()
        {

        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            DrawUi();

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private void DrawUi()
        {
            imageSkiff = new Image(AsyncTexture2D.FromAssetId(SKIFF_ASSET_ID)) { Location = new Point(0, 0), Parent = GameService.Graphics.SpriteScreen };
            imageWaypoint = new Image(AsyncTexture2D.FromAssetId(WAYPOINT_ASSET_ID)) { Location = new Point(64, 0), Parent = GameService.Graphics.SpriteScreen };
            imageFishing = new Image(AsyncTexture2D.FromAssetId(FISHING_ASSET_ID)) { Location = new Point(128, 0), Parent = GameService.Graphics.SpriteScreen };
        }

        protected override void Update(GameTime gameTime)
        {

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            imageSkiff?.Dispose();
            imageWaypoint?.Dispose();
            imageFishing?.Dispose();
            // Unload here

            // All static members must be manually unset
        }

    }

}
