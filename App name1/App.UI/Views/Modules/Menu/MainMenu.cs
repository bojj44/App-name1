namespace UI.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Zebble;
    using Zebble.Framework;

    partial class MainMenu
    {
        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await InitializeComponents();
        }

        public async Task Page1Tapped() => await Nav.Go<Pages.Page1>(PageTransition.Fade);

        public async Task Page2Tapped() => await Nav.Go<Pages.Page2>(PageTransition.Fade);

        public async Task Page3Tapped() => await Nav.Go<Pages.Page3>(PageTransition.Fade);

        public async Task ContactTapped() => await Nav.Go<Pages.Page4>(PageTransition.Fade);
    }
}