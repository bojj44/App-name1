namespace UI.Templates
{
    using System.Linq;
    using System.Threading.Tasks;
    using Zebble;

    [PopupPage]
    public class Modal : Page
    {
        public Stack Body;

        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await Add(Body = new Stack());
        }

        public override async Task OnPreRender()
        {
            await base.OnPreRender();

            this.Margin(top: (Device.Screen.Height - Body.ActualHeight) / 2);
        }

        public string Title { get { return string.Empty; } set { } }
    }
}