/*
  In App.xaml:
  <Application.Resources>
      <vm:GeoToolViewModelLocator xmlns:vm="clr-namespace:De.AHoerstemeier.GeoTool"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
  
  OR (WPF only):
  
  xmlns:vm="clr-namespace:De.AHoerstemeier.GeoTool"
  DataContext="{Binding Source={x:Static vm:GeoToolViewModelLocator.ViewModelNameStatic}}"
*/

using De.AHoerstemeier.GeoTool.Model;
namespace De.AHoerstemeier.GeoTool.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the GeoToolViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:GeoToolViewModelLocator xmlns:vm="clr-namespace:De.AHoerstemeier.GeoTool"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the ViewModelName property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:De.AHoerstemeier.GeoTool"
    /// DataContext="{Binding Source={x:Static vm:GeoToolViewModelLocator.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class GeoToolViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the GeoToolViewModelLocator class.
        /// </summary>
        public GeoToolViewModelLocator()
        {
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view models
            ////}
            ////else
            ////{
            ////    // Create run time view models
            ////}

            CreateGeoData();
        }

        private static GeoDataViewModel _geoData;

        /// <summary>
        /// Gets the GeoDataStatic property.
        /// </summary>
        public static GeoDataViewModel GeoDataStatic
        {
            get
            {
                if ( _geoData == null )
                {
                    CreateGeoData();
                }

                return _geoData;
            }
        }

        /// <summary>
        /// Gets the GeoData property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GeoDataViewModel GeoData
        {
            get
            {
                return GeoDataStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the GeoDataStatic property.
        /// </summary>
        public static void ClearGeoData()
        {
            _geoData.Cleanup();
            _geoData = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the GeoDataStatic property.
        /// </summary>
        public static void CreateGeoData()
        {
            if ( _geoData == null )
            {
                var geoDataModel = new GeoDataModel();
                _geoData = new GeoDataViewModel(geoDataModel);
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearGeoData();
        }
    }
}