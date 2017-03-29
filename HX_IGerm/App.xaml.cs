// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="恒星科技医疗器械有限公司">
//   © 2016 恒星科技医疗器械有限公司
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HX_IGerm
{
    #region References

    using System.Windows;

    using DevExpress.Xpf.Core;

    #endregion

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>The on app startup_ update theme name.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            ApplicationThemeHelper.UpdateApplicationThemeName();
        }
    }
}