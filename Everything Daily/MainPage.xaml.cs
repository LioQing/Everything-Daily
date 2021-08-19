using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Everything_Daily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

            ContentFrame.NavigateToType(typeof(HomePage), null, new FrameNavigationOptions());
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();
            navOptions.IsNavigationStackEnabled = false;

            if (args.InvokedItemContainer == AddARecordButton)
            {
                ContentFrame.NavigateToType(typeof(AddARecordPage), null, navOptions);
                NavView.IsBackEnabled = true;
            }
            else if (args.InvokedItemContainer == AddARecordTypeButton)
            {
                ContentFrame.NavigateToType(typeof(AddARecordTypePage), null, navOptions);
                NavView.IsBackEnabled = true;
            }
            else if (args.InvokedItemContainer == BrowseRecordTypesButton)
            {
                ContentFrame.NavigateToType(typeof(BrowseRecordTypesPage), null, navOptions);
                NavView.IsBackEnabled = true;
            }
            else if (args.IsSettingsInvoked)
            {
                ContentFrame.NavigateToType(typeof(SettingsPage), null, navOptions);
                NavView.IsBackEnabled = true;
            }
            else
            {
                ContentFrame.NavigateToType(typeof(HomePage), null, navOptions);
                NavView.IsBackEnabled = false;
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.IsNavigationStackEnabled = false;

            navOptions.TransitionInfoOverride = new DrillInNavigationTransitionInfo();

            ContentFrame.NavigateToType(typeof(HomePage), null, navOptions);

            sender.SelectedItem = null;
            NavView.IsBackEnabled = false;
        }
    }
}
