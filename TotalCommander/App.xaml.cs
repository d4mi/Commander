using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace TotalCommander
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ResourceDictionary dict = new ResourceDictionary();
        //private static bool locale = false;

        public App ()
	    {
            dict.Source = new Uri("Properties\\Resources\\StringResources.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(dict);

	    }

        public static void Change(string language)
        {
            switch (language)
            {
                case "PL":
                    dict.Source = new Uri("Properties\\Resources\\StringResources_PL.xaml", UriKind.Relative);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");

                    break;
                case "EN":
                    dict.Source = new Uri("Properties\\Resources\\StringResources.xaml", UriKind.Relative);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

                    break;
                default:
                    break;
            }            

            App.Current.Resources.MergedDictionaries.Add(dict);

   
        }

        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
            System.Windows.Markup.XmlLanguage.GetLanguage(System.Threading.Thread.CurrentThread.CurrentUICulture.IetfLanguageTag)));

        }
    }
}
