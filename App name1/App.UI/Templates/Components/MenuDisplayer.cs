namespace UI.Templates
{
    using System.Threading.Tasks;
    using Modules;
    using Zebble;

    public class MenuDisplayer : MainMenuLauncher<MainMenu>
    {
        // Note: If you want a different menu displaying animation, override the following methods:

        //protected override Task SetInitialMenuPosition() => base.SetInitialMenuPosition();
        //protected override Task AnimateMenuIn() => base.AnimateMenuIn();
        //protected override Task AnimateMenuOut() => base.AnimateMenuOut();
    }
}