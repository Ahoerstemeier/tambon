using System;
using System.Collections.Generic;
using De.AHoerstemeier.Geo;
using De.AHoerstemeier.Tambon;
using De.AHoerstemeier.GeoTool.Model;
using GalaSoft.MvvmLight;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace De.AHoerstemeier.GeoTool.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    [System.Runtime.InteropServices.GuidAttribute("B10CB298-EC04-44BD-84A2-A1087FF95AF4")]
    public class GeoDataMapViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the GeoDataMapViewModel class.
        /// </summary>
        /// <param name="model">The model with the actual data.</param>
        public GeoDataMapViewModel(GeoDataModel model)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}

            Model = model;
            Model.PropertyChanged += (s, e) =>
            {
                if ( e.PropertyName == GeoDataModel.LocationPropertyName )
                {
                    RaisePropertyChanged(PushPinLocationPropertyName);
                }
            };

        }

        private GeoDataModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Bing Map usage credentials containing the key.
        /// </summary>
        /// <value>The Bing Map usage credentials containing the key.</value>
        public CredentialsProvider BingMapCredentials
        {
            get
            {
                var value = new ApplicationIdCredentialsProvider();
                value.ApplicationId = GeoDataGlobals.Instance.BingMapsKey;
                return value;
            }
        }

        /// <summary>
        /// The <see cref="PushPinLocation" /> property's name.
        /// </summary>
        public const String PushPinLocationPropertyName = "PushPinLocation";
        /// <summary>
        /// Gets or sets the location of the push pin.
        /// </summary>
        /// <value>The location of the push pin.</value>
        public Location PushPinLocation
        {
            get { return new Location(Model.Location.Latitude, Model.Location.Longitude); }
            set
            {
                Model.SetGeoLocation(value.Latitude, value.Longitude);
            }
        }

    }

}