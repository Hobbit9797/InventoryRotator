using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Storm.ExternalEvent;
using Storm.StardewValley;
using Storm.StardewValley.Event;
using Storm.StardewValley.Wrapper;
using Microsoft.Xna.Framework.Input;

namespace InventoryRotator
{
    [Mod]
    public class InventoryRotator : DiskResource
    {
        public Config ModConfig { get; private set; }
        public KeyboardState lastState;
        public GamePadState lastGPState;
        public Keys keyboardKey;

        [Subscribe]
        public void InitializeCallback(InitializeEvent @event)
        {
            Console.WriteLine("Initializie InventoryRotator");
            var configLocation = Path.Combine(PathOnDisk, "Config.json");
            if (!File.Exists(configLocation))
            {
                Console.WriteLine("Create config file for InventoryRotator...");
                ModConfig = new Config();
                ModConfig.KeyboardKey = "Tab";
                File.WriteAllBytes(configLocation, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ModConfig)));
                Console.WriteLine("Config file for InventoryRotator has been loaded.",
                ModConfig.KeyboardKey, ModConfig.GamepadButton);
            }
            else
            {
                ModConfig = JsonConvert.DeserializeObject<Config>(Encoding.UTF8.GetString(File.ReadAllBytes(configLocation)));
                Console.WriteLine("Config file for InventoryRotator has been loaded.",
                    ModConfig.KeyboardKey);
            }

            keyboardKey = (Keys)Enum.Parse(typeof(Keys), ModConfig.KeyboardKey);
            Console.WriteLine("Initializatiohn for InventoryRotator complete");
        }


        [Subscribe]
        public void UpdateCallback(PreUpdateEvent @event)
        {
            var player = @event.LocalPlayer;
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            Keys[] pressedKeys = keyboard.GetPressedKeys();
            if (!player.Name.Equals(""))
            {
                if ((keyboard.IsKeyDown(keyboardKey) && lastState.IsKeyUp(keyboardKey))||
                    (gamepad.Buttons.LeftShoulder == ButtonState.Pressed && lastGPState.Buttons.LeftShoulder == ButtonState.Released)||
                    (gamepad.Buttons.RightShoulder == ButtonState.Pressed && lastGPState.Buttons.RightShoulder == ButtonState.Released))
                {   
                    Console.WriteLine(ModConfig.KeyboardKey+" pressed");
                    int invSize = player.Items.Count;
                    Console.WriteLine("invSize:" + invSize);
                    List<Item> newInventory = new List<Item>();
                    for (int i = 12; i < invSize; i++)
                    {
                        if (player.Items[i] != null)
                        {
                            newInventory.Add(player.Items[i]);
                        }
                        else
                        {
                            newInventory.Add(null);
                        }
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        if (player.Items[i] != null)
                        {
                            newInventory.Add(player.Items[i]);
                        }
                        else
                        {
                            newInventory.Add(null);
                        }
                    }
                    for (int i = 0; i < invSize; i++)
                    {
                        player.SetItem(i, null);
                        if (newInventory[i] != null)
                        {
                            player.SetItem(i, newInventory[i]);
                        }
                    }
                }
            }

            lastState = keyboard;
            lastGPState = gamepad;
        }
    }

    public class Config
    {
        public String KeyboardKey { get; set; }
        public String GamepadButton { get; set; }
    }
}
