using PM2E13549.Controllers;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E13549
{
    public partial class App : Application
    {
        static Controllers.DBProc dBProc;

        public static Controllers.DBProc Instancia
        {
            get
            {
                if (dBProc == null)
                {
                    String dbname = "Proc.db3";
                    String dbpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    String dbfulp = Path.Combine(dbpath, dbname);
                    dBProc = new Controllers.DBProc(dbfulp);
                }
                return dBProc;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.PageInicial());
        }

        protected override void OnStart()
        {
           
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}
