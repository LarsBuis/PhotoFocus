using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace PhotoFocus
{
    public partial class MainPage : Microsoft.Maui.Controls.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Force the tabs to show at the bottom on Android
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
