using ModelLibrary;
using System;
using System.Windows.Forms;

namespace PBA
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var View = new MainForm();
            var Model = new Model();
            new Presenter.Presenter(View, Model);
            Application.Run(View);
        }
    }
}
