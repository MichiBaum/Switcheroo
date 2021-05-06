// Copyright by Switcheroo

using Switcheroo.Core;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace Switcheroo {
    public class HotkeyViewModel {
        
        public Key KeyCode { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Ctrl { get; set; }
        public bool Windows { get; set; }

        public override string ToString() {
            StringBuilder shortcutText = new();

            if (Ctrl) shortcutText.Append("Ctrl + ");

            if (Shift) shortcutText.Append("Shift + ");

            if (Alt) shortcutText.Append("Alt + ");

            if (Windows) shortcutText.Append("Win + ");

            string keyString =
                KeyboardHelper.CodeToString((uint)KeyInterop.VirtualKeyFromKey(KeyCode)).ToUpper().Trim();
            if (keyString.Length == 0) keyString = new KeysConverter().ConvertToString(KeyCode);

            // If the user presses "Escape" then show "Escape" :)
            if (keyString == "\u001B") keyString = "Escape";

            shortcutText.Append(keyString);
            return shortcutText.ToString();
        }
        
    }
}