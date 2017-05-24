using System;
using System.Collections.Generic;
using Extensions;
using Xamarin.Forms;

namespace K3S.Controls
{
    public partial class NewSurveyView : ContentView
    {
        void btnClick_Clicked(object sender, System.EventArgs e)
        {
            UI.ShowSuccess("Create succcesed new survey");
        }

        public NewSurveyView()
        {
            InitializeComponent();
        }
    }
}
