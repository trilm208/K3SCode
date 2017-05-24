using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

using Xamarin.Forms.Platform.Android;
using K3S.Droid.Renderers;
using Android.Text;
using K3S;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(HtmlFormattedLabel), typeof(XWellcareHtmlLabelRenderer))]
namespace K3S.Droid.Renderers
{
    public class XWellcareHtmlLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (HtmlFormattedLabel)Element;
            if (string.IsNullOrEmpty(view?.Text)) return;
         
            Control?.SetText(BuildHtmlString(view.Text), TextView.BufferType.Spannable);
//            Control.TextFormatted = Html.FromHtml(@"<HTML><HEAD></HEAD>
//<BODY contentEditable=true scroll=auto>
//<P class=cs95E872D0><SPAN class=csCF6BBF71>Q1. Cửa hàng được ghé thăm thuộc thành phố nào?</SPAN></P>
//<P class=cs95E872D0><SPAN class=csCF6BBF71><STRONG><FONT color=blue>INT.: HÃY CHỌN ĐÚNG THÀNH PHỐ / CHỈ ĐƯỢC CHỌN MỘT CÂU TRẢ LỜI</FONT></STRONG></SPAN></P></BODY></HTML>");
        }

        private static ISpanned BuildHtmlString(string text)
        {
            if ((int)Build.VERSION.SdkInt >= 24)
            {
                return Html.FromHtml(text, FromHtmlOptions.ModeLegacy); // SDK >= Android 6
            }
#pragma warning disable 618
            return Html.FromHtml(text);
#pragma warning restore 618
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals(Label.TextProperty.PropertyName))
            {
                var view = (HtmlFormattedLabel)Element;
                if (string.IsNullOrEmpty(view?.Text)) return;

                Control?.SetText(BuildHtmlString(view.Text), TextView.BufferType.Spannable);
            }
        }
    }
}