﻿using System;
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
        private Events.NowLoading model;

        public IList SecurityList {
            get
            {
                return model.SecurityList[SelectedItem];
            }
        }

        public IList Endpoints {
            get
            {
                return model.Endpoints;
            }
        }

        public string UserName
        {
            get
            {
                return model.UserName;
            }

            set
            {
                model.UserName = value;
            }
        }

        public string Password
        {
            get
            {
                return model.Password;
            }

            set
            {
                model.Password = value;
            }
        }

        public int SelectedIndex {
            get
            {
                return model.SelectedIndex;
            }

            set
            {
                model.SelectedIndex = value;
            }
        }

        public string SelectedItem { get; set; }

        public LoadingViewModel(Events.NowLoading model)
        {
            this.model = model;
            SelectedIndex = 0;
            SelectedItem = model.SecurityList.First().Key;
        }
    }
}
