using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;

namespace OpenLayers.Blazor.FlightTracker.Components.Pages;
public partial class FlightTracker
{
    private OpenStreetMap? _map;

    [Inject]
    HttpClient _httpClient { get; set; }

    private System.Timers.Timer? _timer;

    [Inject]
    private ILogger<FlightTracker> _logger { get; set; }

    private AdsbResponse? _flightData;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            try
            {
                //var location = await Geolocation.GetLocationAsync();
                var location = await Geolocation.Default.GetLastKnownLocationAsync();
                if (location != null)
                {
                    _map.Center = new Coordinate(location.Longitude, location.Latitude);
                }
            }
            catch (Exception exp)
            {
                _logger.LogWarning(exp, "Cannot get location.");
            }

            _map.Zoom = 9;

            await LoadTrackingData(_map.Center);

            _timer = new System.Timers.Timer(new TimeSpan(0, 0, 3));
            _timer.Elapsed += async (sender, args) => await LoadTrackingData(_map.Center);
            _timer.Start();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadTrackingData(Coordinate center)
    {
        try
        {
            if (_map == null || _map.VisibleExtent == null)
                return;

            var c1 = new Coordinate(_map.VisibleExtent.X1, _map.VisibleExtent.Y1);
            var c2 = new Coordinate(_map.VisibleExtent.X2, _map.VisibleExtent.Y2);
            var visibleDistance = c1.DistanceTo(c2);
            var nmVisibleRadius = visibleDistance / 2 * 0.5399568; // to nm

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.adsb.lol/v2/point/{center.Y}/{center.X}/{(int)nmVisibleRadius}");

            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
            //{
            //    request.Headers.Add("Access-Control-Allow-Origin", "*");
            //    request.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, Accept");
            //}

            var response = await _httpClient.SendAsync(request);

            _flightData = await response.Content.ReadFromJsonAsync<AdsbResponse>();

            foreach (var ac in _flightData.Flights)
            {
                var marker = _map.MarkersList.OfType<Marker>().FirstOrDefault(p => p.Id == ac.Id);

                if (marker == null)
                {
                    marker = new Marker(new Coordinate(ac.Lon, ac.Lat), "/airplane.png", 512, 512, 256, 256)
                    { Scale = 0.05, Popup = true, Text = ac.Name, Rotation = ac.TrueHeading * Math.PI / 180 };
                    marker.Id = ac.Id;
                    marker.Styles[0].Text.OffsetY = 24;
                    _map.MarkersList.Add(marker);
                }
                {
                    marker.Coordinate = new Coordinate(ac.Lon, ac.Lat);
                    marker.Rotation = ac.TrueHeading * Math.PI / 180;
                    await marker.UpdateShape();
                }
            }
        }
        catch (Exception exp)
        {
            _logger.LogError(exp, "Error reading flight tracking data");
        }

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public class AdsbResponse
    {
        [JsonPropertyName("ac")]
        public AdsbFlight[] Flights { get; set; }
    }

    public class AdsbFlight
    {
        [JsonPropertyName("hex")]
        public string Id { get; set; }

        [JsonPropertyName("flight")]
        public string? Name { get; set; }

        public double Lon { get; set; }
        public double Lat { get; set; }

        [JsonPropertyName("t")]
        public string? Type { get; set; }

        [JsonPropertyName("r")]
        public string? Registration { get; set; }

        [JsonPropertyName("true_heading")]
        public double? TrueHeading { get; set; }

        [JsonPropertyName("alt_baro")]
        public object? AltitudeBaro { get; set; }

        [JsonPropertyName("gs")]
        public double? GroundSpeed { get; set; }
    }

}
