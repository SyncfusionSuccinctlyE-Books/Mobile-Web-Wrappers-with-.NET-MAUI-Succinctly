using Microsoft.Extensions.DependencyInjection;

namespace MobileWrapper
{
    public partial class App : Application
    {

        /// <summary>
        /// Holds the list of dependency-injected services
        /// </summary>
        public static IServiceProvider Services { get; set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            VersionTracking.Track();
            Services = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}