using System;
using Xamarin.Forms;

namespace TilesApp
{
    public class HybridWebView : View
    {
        Action<string> actionS;
        Action actionV;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterActionS(Action<string> callback)
        {
            actionS = callback;
        }

        public void RegisterActionV(Action callback)
        {
            actionV = callback;
        }

        public void Cleanup()
        {
            actionS = null;
            actionV = null;
        }

        public void InvokeAction(string data)
        {
            if (actionS == null || data == null)
            {
                return;
            }
            actionS.Invoke(data);
        }

        public void InvokeVoidAction()
        {
            if (actionV == null)
            {
                return;
            }
            actionV.Invoke();
        }
    }
}
