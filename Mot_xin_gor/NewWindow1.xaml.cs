namespace Mot_xin_gor;

public partial class NewWindow1 : ContentPage
{
    WebView webView;
    string serverAddress;
    string roomId;
    public NewWindow1(string roomId)
    {
        this.roomId = roomId;
        serverAddress = Preferences.Default.Get("ServerAddress", "localhost");

        webView = new WebView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        webView.Navigated += WebView_Navigated;

        Content = webView;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        var url = $"http://{serverAddress}:5000/webrtc.html?roomId={Uri.EscapeDataString(roomId)}";
        webView.Source = url;
    }

    private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        // small delay can help ensure scripts are loaded
        await Task.Delay(300);

        try
        {
            // Optionally set roomId via JS if you prefer:
            // await webView.EvaluateJavaScriptAsync($"setRoomId('{roomId}');");

            // Start the call
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }
            if (status == PermissionStatus.Granted)
                await webView.EvaluateJavaScriptAsync("startCall();");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"JS call error: {ex}");
            await DisplayAlert("Error", "Cannot start call.", "OK");
        }
    }
}