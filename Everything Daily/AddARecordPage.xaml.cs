using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddARecordPage : Page
    {
        private IDictionary<DispatcherTimer, InfoBar> InfoBarTimers { get; set; } = new Dictionary<DispatcherTimer, InfoBar>();
        private IList<string> RecordTypeIdList { get; set; } = new List<string>();

        private Thickness TypeComboBoxThickness { get; set; }
        private Brush TypeComboBoxBrush { get; set; }

        public AddARecordPage()
        {
            this.InitializeComponent();

            TypeComboBoxThickness = RecordTypeComboBox.BorderThickness;
            TypeComboBoxBrush = RecordTypeComboBox.BorderBrush;

            foreach (var (id, record) in (App.Current as App).RecordManager.RecordTypes)
            {
                RecordTypeComboBox.Items.Add(record.Name);
                RecordTypeIdList.Add(id);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ResetInfoBorder();

            if (RecordTypeComboBox.SelectedIndex < 0)
            {
                RecordTypeComboBox.BorderThickness = new Thickness(1);
                RecordTypeComboBox.BorderBrush = new SolidColorBrush(Colors.Red);

                AddInfoBar(InfoBarSeverity.Error, "Please select a record type.");

                return;
            }
            else if (RecordDatePicker.SelectedDate is null)
            {
                AddInfoBar(InfoBarSeverity.Error, "Please select a date.");

                return;
            }
            else if (RecordTimePicker.SelectedTime is null)
            {
                AddInfoBar(InfoBarSeverity.Error, "Please select a time.");

                return;
            }
            else if (RecordDurationPicker.SelectedTime is null && DurationCheckBox.IsChecked == true)
            {
                AddInfoBar(InfoBarSeverity.Error, "Please select a duration or uncheck duration check box.");

                return;
            }

            var id = RecordTypeIdList[RecordTypeComboBox.SelectedIndex];
            var time = RecordDatePicker.Date.DateTime.Date + RecordTimePicker.Time;
            var duration = (DurationCheckBox.IsChecked == true ? RecordDurationPicker.SelectedTime : new TimeSpan(0)) ?? new TimeSpan(0);

            (App.Current as App).RecordManager.Records.Add(new Record(id, time, duration.ToString("hh\\:mm"), ""));
            (App.Current as App).RecordManager.Save();

            AddInfoBar(InfoBarSeverity.Success, "Successfully added record.");
        }

        private void ResetInfoBorder()
        {
            RecordTypeComboBox.BorderThickness = TypeComboBoxThickness;
            RecordTypeComboBox.BorderBrush = TypeComboBoxBrush;
        }

        private void DurationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RecordDurationPicker.IsEnabled = true;
        }

        private void DurationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RecordDurationPicker.IsEnabled = false;
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
