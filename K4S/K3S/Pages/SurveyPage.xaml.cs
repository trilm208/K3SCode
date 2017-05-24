using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DataAccess;
namespace K3S
{
    public partial class SurveyPage : ContentPage
    {
        ClientServices Services;
        public SurveyPage(ClientServices services)
        {
            InitializeComponent();
            this.Services = services;
        }

    }
}
