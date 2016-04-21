using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Marduk.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<TestItem> testItems;

        public MainPage()
        {
            this.InitializeComponent();

            testItems = new List<TestItem>()
            {
                new TestItem() {Name="0 - Adventure Time 1", ColorBrush = new SolidColorBrush(Colors.Aquamarine), Image = "ms-appx:///Assets/Adventure-Time-Wallpaper-Backgrounds-A8Z.jpg"},
                new TestItem() {Name="1 - Adventure Time 2", ColorBrush = new SolidColorBrush(Colors.Coral), Image = "ms-appx:///Assets/adventure_time_1920x1080_47767.jpg"},
                new TestItem() {Name="2 - Kumamon 1", ColorBrush = new SolidColorBrush(Colors.Gold), Image = "ms-appx:///Assets/c-1600x1200px.jpg"},
                new TestItem() {Name="3 - We Bare Bears 1", ColorBrush = new SolidColorBrush(Colors.Indigo), Image = "ms-appx:///Assets/cn_cee_we_bare_bears__cn3__wallpaper_03_1600x900.jpg"},
                new TestItem() {Name="4 - We Bare Bears 2", ColorBrush = new SolidColorBrush(Colors.MediumSeaGreen), Image = "ms-appx:///Assets/hjyKMii94AsGAcrVTY0lUo3GCV9.jpg"},
                new TestItem() {Name="5 - Kumamon 2", ColorBrush = new SolidColorBrush(Colors.SteelBlue), Image = "ms-appx:///Assets/honda-monkey-Kumamon-bike-picture-1000x502.jpg"},
                new TestItem() {Name="6 - We Bare Bears 3", ColorBrush = new SolidColorBrush(Colors.DarkGreen), Image = "ms-appx:///Assets/maxresdefault.jpg"},
                new TestItem() {Name="7 - Adventure Time 3", ColorBrush = new SolidColorBrush(Colors.IndianRed), Image = "ms-appx:///Assets/MNXxLiq.jpg"},
                new TestItem() {Name="8 - Kumamon 3", ColorBrush = new SolidColorBrush(Colors.Firebrick), Image = "ms-appx:///Assets/wallpaper1208-1-3.jpg"},
                new TestItem() {Name="9 - We Bare Bears 3", ColorBrush = new SolidColorBrush(Colors.Lime), Image = "ms-appx:///Assets/we-bare-bears.jpg"},
                new TestItem() {Name="10 - Adventure Time 4", ColorBrush = new SolidColorBrush(Colors.Moccasin), Image = "ms-appx:///Assets/s9HNQHG.jpg"},
                new TestItem() {Name="11 - Gravity Falls 1", ColorBrush = new SolidColorBrush(Colors.Lime), Image = "ms-appx:///Assets/Gravity-Falls.png"},
                new TestItem() {Name="12 - Gravity Falls 2", ColorBrush = new SolidColorBrush(Colors.Moccasin), Image = "ms-appx:///Assets/S2e15_playing_interdimensional_chess.png"},
            };

            EasyCarousel.ItemsSource = testItems;

            var directions = new List<Controls.EasyCarousel.CarouselShiftingDirection>()
            {
                Controls.EasyCarousel.CarouselShiftingDirection.Forward,
                Controls.EasyCarousel.CarouselShiftingDirection.Backward
            };

            comboBox.ItemsSource = directions;
            comboBox.SelectedIndex = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            EasyCarousel.MoveBackward();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            EasyCarousel.MoveForward();
        }

        private async void nc_ItemTapped(object sender, FrameworkElement e)
        {
            await new MessageDialog((e?.DataContext as TestItem)?.Name).ShowAsync();
        }
    }

    public class TestItem
    {
        public string Name { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
        public string Image { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}