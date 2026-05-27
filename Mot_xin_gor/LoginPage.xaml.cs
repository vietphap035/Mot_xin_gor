using ShareModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Mot_xin_gor;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        string address = AddressEntry.Text;
        string urlApi = "";

        if (address == null)
        {
            urlApi = "http://localhost:5000/api/Auth/login";
        }
        else
        {
            urlApi = $"http://{address}:5000/api/Auth/login";
        }
        Preferences.Default.Set("ServerAddress", address ?? "localhost");


        var loginRequest = new LoginModel
        {
            Username = username,
            Password = password
        };

        try
        {
            using(HttpClient client = new HttpClient())
            {
                string jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8,"application/json");

                StatusLabel.Text = "Logging in...";
                var response = await client.PostAsync(urlApi, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var userData = JsonSerializer.Deserialize<User>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    StatusLabel.Text = "Login Successful";
                    var mainWindow = Application.Current?.Windows.FirstOrDefault();
                    Preferences.Default.Set("CurrentUserId", userData?.UId);
                    Preferences.Default.Set("CurrentUsername", userData?.Username);
                    Preferences.Default.Set("CurrentUserMail", userData?.Email);
                    ShareModel.ApiConfig.BaseUrl = $"http://{Preferences.Default.Get("ServerAddress", "localhost")}:5000";

                    Debug.WriteLine($"Logged in user: {userData}");
                    if (mainWindow != null)
                    {
                        mainWindow.Page = new HomePage();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    StatusLabel.Text = "Invalid username or password.";
                }
                else
                {
                    StatusLabel.Text = $"Error: {response.StatusCode}";
                }
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Network Error: Could not connect to the server. ({ex.Message})";
        }
    }

}