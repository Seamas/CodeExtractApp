using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

namespace MAUILib.Platforms.Windows
{
    public class FolderPicker : IFolderPicker
    {
        public async Task<string> PickFolderAsync()
        {
            var folderPicker = new WindowsFolderPicker();
            var hwnd = WindowStateManager.Default.GetActiveWindow().GetWindowHandle();

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
            var result = await folderPicker.PickSingleFolderAsync();
            return result?.Path;
        }
    }
}
