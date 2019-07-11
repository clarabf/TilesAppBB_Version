using System;
using Xamarin.Forms;

namespace TilesApp
{
    public class HybridWebView : View
    {
        Action<string> action;
        Action<int> actionI;

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
            action = callback;
        }

        public void RegisterActionI(Action<int> callback)
        {
            actionI = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }

        public void Test(int num)
        {
            if (actionI == null || num == 0)
            {
                return;
            }
            actionI.Invoke(num);
        }
    }
}
