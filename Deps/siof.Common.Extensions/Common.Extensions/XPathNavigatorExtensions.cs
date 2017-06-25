using System.Xml.XPath;

namespace siof.Common.Extensions
{
    public static class XPathNavigatorExtensions
    {
        public static string GetAttribute(this XPathNavigator navigator, string name)
        {
            return navigator.GetAttribute(name, "");
        }

        public static void SetOrCreateAttribute(this XPathNavigator navigator, string name, string value)
        {
            var navi = navigator.CreateNavigator();
            if (navi.MoveToAttribute(name))
                navi.SetValue(value);
            else
                navi.CreateAttribute(name, value);
        }

        public static bool MoveToAttribute(this XPathNavigator navigator, string name)
        {
            return navigator.MoveToAttribute(name, "");
        }

        public static void CreateAttribute(this XPathNavigator navigator, string name, string value)
        {
            navigator.CreateAttribute("", name, "", value);
        }

        public static void RemoveAttribute(this XPathNavigator navigator, string name)
        {
            var navi = navigator.CreateNavigator();
            if (navi.MoveToAttribute(name))
                navi.DeleteSelf();
        }

        public static XPathNodeIterator SelectChildren(this XPathNavigator navigator, string name)
        {
            return navigator.SelectChildren(name, "");
        }
    }
}
