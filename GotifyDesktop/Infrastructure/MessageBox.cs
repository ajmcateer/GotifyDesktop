using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GotifyDesktop.Infrastructure
{
    public static class Dialog
    {
        public static async Task ShowMessageAsync(ButtonEnum buttonEnum, string title, string message, Icon icon)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = buttonEnum,
                ContentTitle = title,
                ContentMessage = message,
                Icon = icon
            });
            await msBoxStandardWindow.Show();
        }

        public static async Task<ButtonResult> ShowDialogAsync(ButtonEnum buttonEnum, string title, string message, Icon icon)
        {
            var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = buttonEnum,
                ContentTitle = title,
                ContentMessage = message,
                Icon = icon
            });
            return await msBoxStandardWindow.Show();
        }
    }
}
