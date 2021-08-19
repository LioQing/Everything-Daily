using Microsoft.UI;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        static public string IndicatorString { get; set; } = "<Viewbox xmlns=\'http://schemas.microsoft.com/winfx/2006/xaml/presentation\' HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\"><Path Data=\"F1 M 113.769,0.314 L 107.466,5.134 C 107.220,5.322 106.919,5.424 106.610,5.424 L 1.370,5.424 C 0.613,5.424 0.000,6.038 0.000,6.794 L 0.000,6.794 C 0.000,7.550 0.613,8.164 1.370,8.164 L 106.610,8.164 C 106.919,8.164 107.220,8.265 107.466,8.454 L 113.769,13.274 C 114.035,13.478 114.360,13.588 114.695,13.588 L 122.597,13.588 C 126.350,13.588 129.391,10.546 129.391,6.794 L 129.391,6.794 L 129.391,6.794 C 129.391,3.042 126.350,0.000 122.597,0.000 L 114.695,0.000 C 114.360,-0.000 114.035,0.110 113.769,0.314 Z\"/></Viewbox>";
        public Viewbox IndicatorTemplateBox { get; set; }
        private IDictionary<UIElement, Record> TimeDict { get; set; } = new Dictionary<UIElement, Record>();
        private Record SelectedRecord { get; set; }

        public HomePage()
        {
            this.InitializeComponent();

            Calendar.SelectedDates.Add(DateTime.Today);

            DayOverviewBackground.SizeChanged += OnDayBackgroundSizeChanged;

            IndicatorTemplateBox = GenIndicatorBox();
            DayInfoGrid.Children.Add(IndicatorTemplateBox);
            Grid.SetRow(IndicatorTemplateBox, 1);
        }

        private void OnTimePressed(object sender, PointerRoutedEventArgs args)
        {
            var timeElement = sender as UIElement;
            var record = TimeDict[timeElement];

            SelectedRecord = record;
            RecordRemarks.IsEnabled = true;
            RecordRemarks.Text = record.Remarks;
            SetRecordInfoDisplay(record);
        }

        private void SetRecordInfoDisplay()
        {
            RecordText.Text = "Select a Record";
            RecordDateText.Text = "Date: ";
            RecordStartTimeText.Text = "Start Time: ";
            RecordEndTimeText.Text = "End Time: ";
            RecordDurationText.Text = "Duration: ";

            RecordRemarks.Text = "";
            RecordRemarks.IsEnabled = false;

            DeleteButton.IsEnabled = false;
        }

        private void SetRecordInfoDisplay(Record record)
        {
            var recordManager = (App.Current as App).RecordManager;

            RecordText.Text = recordManager.RecordTypes[record.Id].Name;
            RecordDateText.Text = "Date: " + record.Time.Date.ToLongDateString();
            RecordStartTimeText.Text = "Start Time: " + record.Time.ToString("HH\\:mm");
            RecordEndTimeText.Text = "End Time: " + (record.Time + TimeSpan.Parse(record.Duration)).ToString("HH\\:mm");
            RecordDurationText.Text = "Duration: " + record.Duration;
            RecordRemarks.Text = record.Remarks;

            DeleteButton.IsEnabled = true;
        }

        private void OnDayBackgroundSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateTimeIndicatorAndRect();
        }

        private void UpdateTimeIndicatorAndRect()
        {
            if (Window.Current is null)
                return;

            // clear indicators and rects
            var recordManager = (App.Current as App).RecordManager;

            for (int i = DayInfoGrid.Children.Count; i >= 0; --i)
            {
                var child = DayInfoGrid.Children[i];

                if (child == IndicatorTemplateBox || 
                    child == DayOverviewBackground ||
                    child is StackPanel)
                    continue;

                DayInfoGrid.Children.Remove(child);
            }

            TimeDict.Clear();

            IList<Tuple<Record, RecordType>> toBeAddIndicator = new List<Tuple<Record, RecordType>>();

            foreach (var record in recordManager.Records)
            {
                if (!recordManager.RecordTypes.ContainsKey(record.Id))
                    continue;

                var type = recordManager.RecordTypes[record.Id];

                if (record.Time.Date != Calendar.SelectedDates[0].Date)
                    continue;

                var duration = TimeSpan.Parse(record.Duration);
                if (duration.TotalSeconds == 0)
                    toBeAddIndicator.Add(new Tuple<Record, RecordType>(record, type));
                else
                    TimeDict.Add(AddTimeRect(record.Time, duration, type.Color), record);
            }

            foreach (var (record, type) in toBeAddIndicator)
            {
                TimeDict.Add((AddTimeIndicator(record.Time, type.Color) as Viewbox).Child, record);
            }
        }

        private UIElement AddTimeRect(DateTime time, TimeSpan duration, Color color)
        {
            var hour = time.Hour + time.Minute / 60.0;
            var durationHour = duration.Hours + duration.Minutes / 60.0;

            if (hour + durationHour > 24.0)
                durationHour = 24.0 - hour;

            var rect = new Rectangle();
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.Fill = new SolidColorBrush(color);
            rect.PointerPressed += OnTimePressed;
            rect.Margin = new Thickness(
                DayOverviewBackground.Margin.Left, 
                hour / 24.0 * DayOverviewBackground.ActualHeight + DayOverviewBackground.Margin.Top, 
                DayOverviewBackground.Margin.Right, 
                DayOverviewBackground.Margin.Bottom);
            rect.RadiusX = DayOverviewBackground.RadiusX;
            rect.RadiusY = DayOverviewBackground.RadiusY;
            rect.Width = DayOverviewBackground.Width;
            rect.Height = durationHour / 24.0 * DayOverviewBackground.ActualHeight;

            Grid.SetRow(rect, 1);
            DayInfoGrid.Children.Add(rect);

            return rect;
        }

        private UIElement AddTimeIndicator(DateTime time, Color color)
        {
            var hour = time.Hour + time.Minute / 60.0;
            var indicator = GenIndicatorBox();

            var path = indicator.Child as Path;
            path.Fill = new SolidColorBrush(color);
            path.PointerPressed += OnTimePressed;
            indicator.Margin = new Thickness(0, hour / 24.0 * DayOverviewBackground.ActualHeight, 0, 0);

            Grid.SetRow(indicator, 1);
            DayInfoGrid.Children.Add(indicator);

            return indicator;
        }

        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count == 0)
                return;

            UpdateTimeIndicatorAndRect();

            if (args.AddedDates.Count > 0)
                DateText.Text = args.AddedDates[0].Date.ToLongDateString();
        }

        private Viewbox GenIndicatorBox()
        {
            var ret = (Viewbox)XamlReader.Load(IndicatorString);
            ret.Width = DayOverviewBackground.ActualWidth * 1.3;

            return ret;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var recordManager = (App.Current as App).RecordManager;

            ContentDialog dialog = new ContentDialog();
            dialog.Title = $"Delete {recordManager.RecordTypes[SelectedRecord.Id].Name}?";
            dialog.Content = "The selected record will be permanently deleted.";
            dialog.PrimaryButtonText = "Delete";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Close;

            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.None)
                return;

            recordManager.Records.Remove(SelectedRecord);
            recordManager.Save();

            SelectedRecord = null;

            RecordRemarks.Text = "";
            RecordRemarks.IsEnabled = false;

            DeleteButton.IsEnabled = false;

            UpdateTimeIndicatorAndRect();
            SetRecordInfoDisplay();
        }

        private void RecordRemarks_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedRecord is null)
                return;

            var remarks = sender as TextBox;

            SelectedRecord.Remarks = remarks.Text;
            (App.Current as App).RecordManager.Save();
        }
    }
}
