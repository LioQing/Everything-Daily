using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public string SaveFolder { get; private set; } = ApplicationData.Current.LocalFolder.Path.ToString();

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize((App.Current as App).RecordManager, options);

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
            savePicker.SuggestedFileName = "save";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, jsonString);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == FileUpdateStatus.Complete)
                {
                    // OutputTextBlock.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    // OutputTextBlock.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".json");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string jsonString = await FileIO.ReadTextAsync(file);
                (App.Current as App).RecordManager.Load(jsonString);
            }
            else
            {
                // this.textBlock.Text = "Operation cancelled.";
            }
        }
    }
}
