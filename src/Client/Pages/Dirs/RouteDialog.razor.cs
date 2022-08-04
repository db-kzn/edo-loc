using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteDialog
    {
        [Inject] private IDirectoryManager DirManager { get; set; }

        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public AddEditRouteCommand _route { get; set; } = new();

        private int page = 1;

        public void CancelAsync() { }
        public void SaveAsync() { }

        public void StagesPageAsync()
        {
            page = 2;
        }
        public void CardPageAsync()
        {
            page = 1;
        }

        private class TabView
        {
            public String Name { get; set; }
            public String Content { get; set; }
            public Guid Id { get; set; }
            public String Style { get; set; }
        }

        private List<TabView> _tabs = new();
        private int _index = 0;
        private int? _nextIndex = null;

        private void RemoveTab(MudTabPanel tabPanel)
        {
            var tab = _tabs.FirstOrDefault(x => x.Id == (Guid)tabPanel.Tag);
            if (tab != null)
            {
                _tabs.Remove(tab);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //_tabs.Add(new TabView { Content = "First tab content", Name = "Card", Id = Guid.NewGuid() });
        }

        private void AddTabCallback(Boolean append)
        {
            var tabView = new TabView { Name = $"Stage {_tabs.Count + 1}", Content = "A new tab 1", Id = Guid.NewGuid() }; // , Style="color: red;"
            _tabs.Add(tabView);
            _nextIndex = _tabs.Count;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_nextIndex.HasValue == true)
            {
                _index = _nextIndex.Value;
                _nextIndex = null;
                StateHasChanged();
            }
        }
    }
}
