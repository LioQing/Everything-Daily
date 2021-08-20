﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowseRecordTypesPage : Page
    {
        public bool AscSort { get; set; } = true;
        private string SelectedTypeId { get; set; } = null;

        public BrowseRecordTypesPage()
        {
            this.InitializeComponent();

            UpdateList();
        }

        private void UpdateList()
        {
            ListView.Items.Clear();

            var TypeList = (App.Current as App).RecordManager.RecordTypes.ToList();

            TypeList.Sort((x, y) => AscSort ? x.Value.Name.CompareTo(y.Value.Name) : -x.Value.Name.CompareTo(y.Value.Name));

            foreach (var (id, record) in TypeList)
            {
                var border = new Border() { BorderThickness = new Thickness(2) };

                var grid = new Grid() { ColumnSpacing = 10 };
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                border.Child = grid;

                // color
                var canvasBorder = new Border() { BorderThickness = new Thickness(2) };
                grid.Children.Add(canvasBorder);

                var canvas = new Canvas();
                canvas.Width = 64;
                canvas.Height = 64;
                canvasBorder.Child = canvas;

                var path = new Path() { Fill = new SolidColorBrush(record.Color) };
                canvas.Children.Add(path);

                var ellipse = new EllipseGeometry();
                path.Data = ellipse;
                ellipse.Center = new Point(32f, 32f);
                ellipse.RadiusX = 24;
                ellipse.RadiusY = 24;

                // name and id
                var subGrid = new Grid();
                Grid.SetColumn(subGrid, 1);
                subGrid.VerticalAlignment = VerticalAlignment.Center;
                subGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                subGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                grid.Children.Add(subGrid);

                var nameText = new TextBlock() { Text = record.Name };
                nameText.Style = this.Resources["TitleTextBlockStyle"] as Style;
                subGrid.Children.Add(nameText);

                var idBorder = new Border() { BorderThickness = new Thickness(0, 0, 0, 5) };
                subGrid.Children.Add(idBorder);
                Grid.SetRow(idBorder, 1);

                var idText = new TextBlock() { Text = id };
                idText.Style = this.Resources["CaptionTextBlockStyle"] as Style;
                idBorder.Child = idText;

                // count
                var recordList = (App.Current as App).RecordManager.Records;

                var countText = new TextBlock() { Text = recordList.Where(x => x.Id == id).Select(x => x.Id).Count().ToString() };
                countText.HorizontalAlignment = HorizontalAlignment.Left;
                countText.VerticalAlignment = VerticalAlignment.Center;
                countText.Style = this.Resources["SubtitleTextBlockStyle"] as Style;
                Grid.SetColumn(countText, 2);
                grid.Children.Add(countText);

                // duration
                var durationText = new TextBlock() { Text = (new TimeSpan(recordList.Where(x => x.Id == id).Sum(x => TimeSpan.Parse(x.Duration).Ticks)).TotalMinutes / 60f).ToString() + " hr" };
                durationText.HorizontalAlignment = HorizontalAlignment.Left;
                durationText.VerticalAlignment = VerticalAlignment.Center;
                durationText.Style = this.Resources["SubtitleTextBlockStyle"] as Style;
                Grid.SetColumn(durationText, 3);
                grid.Children.Add(durationText);

                ListView.Items.Add(border);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SelectedTypeId = 
                    (((((e.AddedItems[0] as Border)
                    .Child as Grid)
                    .Children
                    .Cast<FrameworkElement>()
                    .First(x => Grid.GetColumn(x) == 1) as Grid)
                    .Children
                    .Cast<FrameworkElement>()
                    .First(x => Grid.GetRow(x) == 1) as Border)
                    .Child as TextBlock)
                    .Text;

                DeleteButton.IsEnabled = true;
            }
            else
            {
                SelectedTypeId = null;
                DeleteButton.IsEnabled = false;
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var recordManager = (App.Current as App).RecordManager;

            ContentDialog dialog = new ContentDialog();
            dialog.Title = $"Delete {recordManager.RecordTypes[SelectedTypeId].Name}({SelectedTypeId})?";
            dialog.PrimaryButtonText = "Delete";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Close;
            dialog.Content = "The selected record type will be permanently deleted. The record type can be recreated with the same record type ID.";

            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.None)
                return;

            recordManager.RecordTypes.Remove(SelectedTypeId);
            recordManager.Save();

            SelectedTypeId = null;
            DeleteButton.IsEnabled = false;

            UpdateList();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            AscSort = !AscSort;
            UpdateList();
        }
    }
}
