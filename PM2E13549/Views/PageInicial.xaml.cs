using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Essentials;
using System.Diagnostics;
using PM2E13549.Controllers;

namespace PM2E13549.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageInicial : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile photo = null;

        public PageInicial()
        {
            InitializeComponent();
            CheckGpsStatus();
        }
        private async void CheckGpsStatus()
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

                if (!(isGeolocationAvailable && isGeolocationEnabled))
                {
                    await DisplayAlert("GPS Desactivado", "Active el GPS", "OK");
                }
            }
            else
            {
                await DisplayAlert("Permiso Denegado", "Permisos de GPS Denegados Activelos en la Configurasion", "OK");
            }
        }
        public string Getimage64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();

                    String Base64 = Convert.ToBase64String(fotobyte);

                    return Base64;
                }
            }
            return null;
        }

        public byte[] GetimageBytes()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();

                    return fotobyte;
                }
            }
            return null;
        }

        private async void Btnguardar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Descripcion.Text) || Foto.Source == null || string.IsNullOrEmpty(Latitud.Text) || string.IsNullOrEmpty(Longitud.Text))
            {
                // Al menos uno de los campos está vacío, muestra un mensaje de error o realiza alguna acción
                await DisplayAlert("Error", "Por favor, complete todos los campos y agregue una foto.", "Aceptar");
            }
            else
            {
                // Todos los campos están llenos, puedes proceder con la acción de agregar
                var siti = new Models.Sitios
                {
                    Latitud = Latitud.Text,
                    Longitud = Longitud.Text,                   
                    Descripcion = Descripcion.Text,
                    Foto = GetimageBytes()
                };

                if (await App.Instancia.AddSitio(siti) > 0)
                {
                    await DisplayAlert("Aviso", "Sitio ingreso con exito", "OK");
                }
                else
                    await DisplayAlert("Aviso", "A ocurrido un error", "OK");
            }
        }

        private async void Btnfoto_Clicked(object sender, EventArgs e)
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
                        var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));

                        double latitude = position.Latitude;
                        double longitude = position.Longitude;

                        Latitud.Text = latitude.ToString();
                        Longitud.Text = longitude.ToString();

                        photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            Directory = "MYAPP",
                            Name = "Foto.jpg",
                            SaveMetaData = true,
                        });

                        if (photo != null)
                        {
                            Foto.Source = ImageSource.FromStream(() => photo.GetStream());
                        }
                    }
                    else
                    {
                        // Geolocation is not enabled, show a message to the user
                        await DisplayAlert("GPS Desactivado", "Active el GPS", "OK");
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

        private async void Btnlista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.PageListSitio());
        }

        private  void Btnsalir_Clicked(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                // Mostrar un mensaje de error en otras plataformas que no admiten el cierre de la aplicación
                DisplayAlert("Error", "No se puede cerrar la aplicación en esta plataforma.", "Aceptar");
            }
        }
    }
}