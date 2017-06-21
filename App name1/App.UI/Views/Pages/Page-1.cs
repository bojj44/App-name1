namespace UI.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Zebble;
    using Zebble.Framework;
    using Domain;

    partial class Page1
    {
        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await InitializeComponents();
        }
    }
}