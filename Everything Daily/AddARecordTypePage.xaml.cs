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
using Windows.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddARecordTypePage : Page
    {
        private IDictionary<DispatcherTimer, InfoBar> InfoBarTimers { get; set; } = new Dictionary<DispatcherTimer, InfoBar>();

        private Thickness IdTextBoxThickness { get; set; }
        private Brush IdTextBoxBrush { get; set; }
        private Thickness NameTextBoxThickness { get; set; }
        private Brush NameTextBoxBrush { get; set; }

        public AddARecordTypePage()
        {
            this.InitializeComponent();

            IdTextBoxThickness = RecordTypeIdTextBox.BorderThickness;
            IdTextBoxBrush = RecordTypeIdTextBox.BorderBrush;
            NameTextBoxThickness = RecordTypeNameTextBox.BorderThickness;
            NameTextBoxBrush = RecordTypeNameTextBox.BorderBrush;
        }

        private void GenIdCheckTextBox_Checked(object sender, RoutedEventArgs e)
        {
            RecordTypeIdTextBox.IsEnabled = false;
        }

        private void GenIdCheckTextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RecordTypeIdTextBox.IsEnabled = true;
        }

        private void RecordTypeNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!RecordTypeIdTextBox.IsEnabled)
                RecordTypeIdTextBox.Text = ((TextBox)sender).Text.Replace(' ', '_');
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ResetInfoBorder();
            var recordTypeList = (App.Current as App).RecordManager.RecordTypes;

            if (RecordTypeIdTextBox.Text.Length == 0 || RecordTypeNameTextBox.Text.Length == 0)
            {
                RecordTypeIdTextBox.BorderThickness = new Thickness(1.0);
                RecordTypeIdTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                RecordTypeNameTextBox.BorderThickness = new Thickness(1.0);
                RecordTypeNameTextBox.BorderBrush = new SolidColorBrush(Colors.Red);

                AddInfoBar(InfoBarSeverity.Error, "Record Type ID and Name must contain at least 1 character.");

                return;
            }
            else if (recordTypeList.ContainsKey(RecordTypeIdTextBox.Text))
            {
                RecordTypeIdTextBox.BorderThickness = new Thickness(1.0);
                RecordTypeIdTextBox.BorderBrush = new SolidColorBrush(Colors.Red);

                AddInfoBar(InfoBarSeverity.Error, "Record Type ID already exists.");

                return;
            }
            else if (RecordTypeIdTextBox.Text.Contains(' '))
            {
                RecordTypeIdTextBox.BorderThickness = new Thickness(1.0);
                RecordTypeIdTextBox.BorderBrush = new SolidColorBrush(Colors.Red);

                AddInfoBar(InfoBarSeverity.Error, "Record Type ID should not contain space.");

                return;
            }

            // successful add
            var record = new RecordType(RecordTypeNameTextBox.Text, ColorPicker.Color);
            recordTypeList.Add(RecordTypeIdTextBox.Text, record);
            (App.Current as App).RecordManager.Save();

            RecordTypeIdTextBox.Text = "";
            RecordTypeNameTextBox.Text = "";

            AddInfoBar(InfoBarSeverity.Success, "Successfully added record type.");
        }

        private void ResetInfoBorder()
        {
            RecordTypeIdTextBox.BorderThickness = IdTextBoxThickness;
            RecordTypeIdTextBox.BorderBrush = IdTextBoxBrush;
            RecordTypeNameTextBox.BorderThickness = NameTextBoxThickness;
            RecordTypeNameTextBox.BorderBrush = NameTextBoxBrush;
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
