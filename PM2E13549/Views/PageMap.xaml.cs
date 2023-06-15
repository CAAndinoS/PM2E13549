using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using PM2E13549.Models;
using System.IO;
using Plugin.Media;

namespace PM2E13549.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageMap : ContentPage
    {
        private Sitios selectedSitio; // Declarar selectedSitio como variable miembro

        public PageMap(Sitios selectedSitio)
        {
            InitializeComponent();
            this.selectedSitio = selectedSitio; // Asignar el valor de selectedSitio al miembro de la clase
            CheckLocationPermissionAndGPS();
            Mapa.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Xamarin.Forms.Maps.Position(Convert.ToDouble(selectedSitio.Latitud), Convert.ToDouble(selectedSitio.Longitud)),
                Distance.FromMiles(1)));

            // Mostrar la descripción en un pin en el mapa
            var pin = new Pin
            {
                Position = new Xamarin.Forms.Maps.Position(Convert.ToDouble(selectedSitio.Latitud), Convert.ToDouble(selectedSitio.Longitud)),
                Label = "Ubicacion",
                Address = selectedSitio.Descripcion
            };
            Mapa.Pins.Add(pin);
        }
        private async void CheckLocationPermissionAndGPS()       
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;

                    bool isGeolocationAvailable = locator.IsGeolocationAvailable;
                    bool isGeolocationEnabled = locator.IsGeolocationEnabled;

                    if (isGeolocationAvailable && isGeolocationEnabled)
                    {
                        Mapa.IsShowingUser = true;
                    }
                    else
                    {
                        // Geolocation is not enabled, show a message to the user
                        await DisplayAlert("GPS Disabled", "Please enable GPS to proceed.", "OK");
                    }
                }
                else
                {
                    // Geolocation permission denied
                    // Handle the case where the user denied the permission
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the geolocation process
                Console.WriteLine($"Geolocation error: {ex.Message}");
            }
        }
            

        private void Btncompartir_Clicked(object sender, EventArgs e)
        {
            byte[] fotoBytes = Convert.FromBase64String(Convert.ToBase64String(selectedSitio.Foto));

            // Crear un archivo temporal para almacenar la imagen
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_image.jpg");
            File.WriteAllBytes(tempFilePath, fotoBytes);

            // Crear la solicitud de archivo para compartir
            var fileRequest = new ShareFileRequest
            {
                Title = "Compartir imagen",
                File = new ShareFile(tempFilePath)
            };

            // Mostrar el diálogo para compartir
            Share.RequestAsync(fileRequest);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.PageListSitio());
        }
    }
}