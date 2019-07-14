using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Jupiter.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        public IList SecurityList { get; }
        public string Endpoint { get; }
        public int SelectedIndex { get; set; }

        public LoadingViewModel(IList securityList, string endpoint)
        {
            SecurityList = securityList;
            Endpoint = endpoint;
            SelectedIndex = 0;
        }
    }
}
