namespace UI
{
    using System;
    using System.Threading.Tasks;
    using Zebble;

    public partial class StartUp : Zebble.StartUp
    {
        public override async Task Run()
        {
            ApplicationName = "$SolutionName$";

            await InstallIfNeeded();

            RegisterDataProvider(typeof(AppData.AdoDotNetDataProviderFactory));

            CssStyles.LoadAll();

            Device.System.ReceivedMemoryWarning.Handle(() => Alert.Toast("There is a shortage of memory. The application may crash."));

            Services.PushNotificationListener.Setup();

            LoadFirstPage().RunInParallel();

            //  Zebble.Sanity.Agent.Register();
        }

        public static Task LoadFirstPage() => Nav.Go(new Pages.Page1());
    }
}