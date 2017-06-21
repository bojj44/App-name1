namespace UI.Templates
{
    using System.Threading.Tasks;
    using Zebble;

    public class Blank : Page
    {
        public ScrollView BodyScroller = new ScrollView().Height(100.Percent()).Padding(20);
        public Stack Body;

        public override async Task OnInitializing()
        {
            this.Height(Device.Screen.Height);

            await base.OnInitializing();

            await Add(BodyScroller);
            await BodyScroller.Add(Body = new Stack());
        }

        public string Title { get { return string.Empty; } set { } }
    }
}