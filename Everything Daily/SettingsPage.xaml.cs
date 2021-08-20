using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        private IDictionary<DispatcherTimer, InfoBar> InfoBarTimers { get; set; } = new Dictionary<DispatcherTimer, InfoBar>();
        public string SaveFolder { get; private set; } = ApplicationData.Current.LocalFolder.Path.ToString();

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string jsonString = JsonConvert.SerializeObject((App.Current as App).RecordManager, Formatting.Indented);

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
                    AddInfoBar(InfoBarSeverity.Success, "Save successfully exported.");
                }
                else
                {
                    AddInfoBar(InfoBarSeverity.Error, "Save failed to be exported.");
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
                try
                {
                    var task = Task.Run(async () => await FileIO.ReadTextAsync(file));
                    var jsonString = task.Result;
                    (App.Current as App).RecordManager.Load(jsonString);

                    AddInfoBar(InfoBarSeverity.Success, "Save successfully imported.");
                }
                catch(Exception)
                {
                    AddInfoBar(InfoBarSeverity.Error, "Save failed to be imported.");
                }
            }
        }

        private void AddInfoBar(InfoBarSeverity severity, string msg)
        {
            var infoBar = new InfoBar();
            infoBar.IsOpen = true;
            infoBar.Severity = severity;
            infoBar.Message = msg;

            var timer = new DispatcherTimer();
            timer.Tick += OnTimedEvent;
            timer.Interval = new TimeSpan(0, 0, 3);
            InfoBarTimers.Add(timer, infoBar);
            timer.Start();

            NotificationPanel.Children.Add(infoBar);
        }

        private void OnTimedEvent(object sender, object args)
        {
            var timer = sender as DispatcherTimer;

            timer.Stop();
            InfoBarTimers[timer].IsOpen = false;
            NotificationPanel.Children.Remove(InfoBarTimers[timer]);
            InfoBarTimers.Remove(timer);
        }
    }
}
