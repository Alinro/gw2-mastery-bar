using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace mastery
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        // controls
        private StandardWindow windowContainer;
        private Image imageFishing;
        private Image imageSkiff;
        private Image imageWaypoint;

        // asset ids from https://search.gw2dat.com/
        private const int FISHING_ASSET_ID = 2594729;
        private const int SKIFF_ASSET_ID = 2593817;
        private const int WAYPOINT_ASSET_ID = 2595066;

        // settings
        private SettingEntry<KeyBinding> settingShowWindowContainerKeybind;
        private SettingEntry<KeyBinding> settingFishingKeybind;
        private SettingEntry<KeyBinding> settingSkiffKeybind;
        private SettingEntry<KeyBinding> settingWaypointKeybind;

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
            settingShowWindowContainerKeybind = settings.DefineSetting("Mastery buttons keybind", new KeyBinding(Keys.None), () => "Mastery buttons keybind", () => "When the keybind is pressed, the mastery buttons will be shown. If this is unset, the buttons will be always shown");
            settingShowWindowContainerKeybind.Value.Enabled = true;
            settingShowWindowContainerKeybind.Value.Activated += ShowWindowContainerKeybindOnActivated;

            settingFishingKeybind = settings.DefineSetting("Fishing keybind", new KeyBinding(Keys.None), () => "Fishing keybind", () => "When the keybind is pressed, fishing will be toggled.");
            settingFishingKeybind.Value.Enabled = true;

            settingSkiffKeybind = settings.DefineSetting("Skiff keybind", new KeyBinding(Keys.None), () => "Skiff keybind", () => "When the keybind is pressed, the skiff will be activated.");
            settingSkiffKeybind.Value.Enabled = true;

            settingWaypointKeybind = settings.DefineSetting("Waypoint keybind", new KeyBinding(Keys.None), () => "Waypoint keybind", () => "When the keybind is pressed, the waypoint will be activated.");
            settingWaypointKeybind.Value.Enabled = true;
        }

        private void ShowWindowContainerKeybindOnActivated(object sender, EventArgs e)
        {
            // show window container when keybind is triggered
            windowContainer.Show();
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
            windowContainer = new StandardWindow(
                AsyncTexture2D.FromAssetId(43309).Texture,
                new Rectangle(0, 0, 64 * 3, 64),
                new Rectangle(0, 0, 64 * 3, 64)
            )
            { Title = "", CanResize = false, CanClose = false, Parent = GameService.Graphics.SpriteScreen, SavesPosition = true };

            imageSkiff = new Image(AsyncTexture2D.FromAssetId(SKIFF_ASSET_ID)) { Location = new Point(0, 0), Parent = windowContainer };
            imageSkiff.Click += ImageSkiffOnClick;

            imageWaypoint = new Image(AsyncTexture2D.FromAssetId(WAYPOINT_ASSET_ID)) { Location = new Point(64, 0), Parent = windowContainer };
            imageWaypoint.Click += ImageWaypointOnClick;

            imageFishing = new Image(AsyncTexture2D.FromAssetId(FISHING_ASSET_ID)) { Location = new Point(128, 0), Parent = windowContainer };
            imageFishing.Click += ImageFishingOnClick;
        }

        private void ImageSkiffOnClick(object sender, MouseEventArgs e)
        {
            PressKey(settingSkiffKeybind);
        }

        private void ImageWaypointOnClick(object sender, MouseEventArgs e)
        {
            PressKey(settingWaypointKeybind);
        }

        private void ImageFishingOnClick(object sender, MouseEventArgs e)
        {
            PressKey(settingFishingKeybind);
        }

        // press the settings keys, wait, release the settings keys
        private async void PressKey(SettingEntry<KeyBinding> settingKeybind)
        {
            Tuple<ModifierKeys, VirtualKeyShort>[] modifierKeys = {
                new Tuple<ModifierKeys, VirtualKeyShort>(ModifierKeys.Alt, VirtualKeyShort.MENU), 
                new Tuple<ModifierKeys, VirtualKeyShort>(ModifierKeys.Ctrl, VirtualKeyShort.CONTROL), 
                new Tuple<ModifierKeys, VirtualKeyShort>(ModifierKeys.Shift, VirtualKeyShort.SHIFT)
            };

            foreach(Tuple<ModifierKeys, VirtualKeyShort> modifierKey in modifierKeys)
            {
                if (settingKeybind.Value.ModifierKeys.HasFlag(modifierKey.Item1))
                {
                    Blish_HUD.Controls.Intern.Keyboard.Press(modifierKey.Item2, false);
                }
            }

            Blish_HUD.Controls.Intern.Keyboard.Press((VirtualKeyShort)settingKeybind.Value.PrimaryKey, false);

            await Task.Delay(50);

            foreach (Tuple<ModifierKeys, VirtualKeyShort> modifierKey in modifierKeys)
            {
                if (settingKeybind.Value.ModifierKeys.HasFlag(modifierKey.Item1))
                {
                    Blish_HUD.Controls.Intern.Keyboard.Release(modifierKey.Item2, false);
                }
            }

            Blish_HUD.Controls.Intern.Keyboard.Release((VirtualKeyShort)settingKeybind.Value.PrimaryKey, false);
        }

        protected override void Update(GameTime gameTime)
        {
            // hide the window container if the keybind is not being pressed and if there is a keybind set for it
            if (windowContainer.Visible && !settingShowWindowContainerKeybind.Value.IsTriggering && settingShowWindowContainerKeybind.Value.PrimaryKey != Keys.None)
            {
                windowContainer.Hide();
            }
            // show the window container if there is no keybind set for it
            else if (!windowContainer.Visible && settingShowWindowContainerKeybind.Value.PrimaryKey == Keys.None)
            {
                windowContainer.Show();
            }
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            imageSkiff?.Dispose();
            imageWaypoint?.Dispose();
            imageFishing?.Dispose();
            windowContainer?.Dispose();
            // Unload here

            // All static members must be manually unset
        }

    }

}
