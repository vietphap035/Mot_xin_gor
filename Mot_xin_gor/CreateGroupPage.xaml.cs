using Microsoft.Maui.Controls.Shapes;
using ShareModel;
using System.Net.Http.Json;

namespace Mot_xin_gor;

public partial class CreateGroupPage : ContentPage
{
    private readonly List<string> _selectedUsers = new();
    public CreateGroupPage()
    {
        InitializeComponent();
    }

    private void OnUserCompleted(object sender, EventArgs e)
    {
        string input = UserNameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(input))
            return;

        if (!IsValidEmail(input))
        {
            UserErrorLabel.Text = "Email không đúng định dạng";
            UserErrorLabel.IsVisible = true;
            return;
        }

        if (_selectedUsers.Contains(input))
        {
            UserErrorLabel.Text = "Người dùng đã được thêm";
            UserErrorLabel.IsVisible = true;
            return;
        }
        if(input == Preferences.Default.Get("CurrentUserMail", String.Empty))
        {
            UserErrorLabel.Text = "Không thể thêm chính bạn";
            UserErrorLabel.IsVisible = true;
            return;
        }

        UserErrorLabel.IsVisible = false;
        _selectedUsers.Add(input);
        AddUserChip(input);

        UserNameEntry.Text = string.Empty;
        if (!GroupRadioButton.IsChecked)
        {
            UserNameEntry.IsEnabled = _selectedUsers.Count < 1;
        }
        
    }

    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void AddUserChip(string email)
    {
        var label = new Label
        {
            Text = email,
            TextColor = Colors.Black,
            FontSize = 13
        };

        var removeBtn = new Button
        {
            Text = "✕",
            FontSize = 12,
            Padding = 0,
            BackgroundColor = Colors.Transparent
        };

        var chipLayout = new HorizontalStackLayout
        {
            Spacing = 5,
            Padding = new Thickness(10, 5),
            BackgroundColor = Colors.LightGray
        };

        chipLayout.Children.Add(label);
        chipLayout.Children.Add(removeBtn);

        var border = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Content = chipLayout,
            Margin = new Thickness(4)
        };

        removeBtn.Clicked += (s, e) =>
        {
            SelectedUsersLayout.Children.Remove(border);
            _selectedUsers.Remove(email);
        };

        SelectedUsersLayout.Children.Add(border);
    }

    private void OnChatTypeChanged(object sender, CheckedChangedEventArgs e)
    {
        GroupNameEntry.IsVisible = GroupRadioButton.IsChecked;
        GroupNameEntry.Text = string.Empty;
        _selectedUsers.Clear();
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        string groupName = GroupNameEntry.Text;
        string currentUserEmail = Preferences.Default.Get("CurrentUserMail", String.Empty);
        if (string.IsNullOrWhiteSpace(groupName) && _selectedUsers.Count >= 2) // Nhóm nhưng không có tên
        {
            await DisplayAlert("Lỗi", "Tên nhóm không được để trống", "OK");
            return;
        }
        if (_selectedUsers.Count == 0) // Không có người dùng nào được thêm
        {
            await DisplayAlert("Lỗi", "Vui lòng thêm ít nhất một người dùng vào nhóm", "OK");
            return;
        }
        if (_selectedUsers.Count == 1) // Trò chuyện cá nhân, sử dụng email của người dùng được chọn làm tên nhóm
        {
            groupName = _selectedUsers[0];
        }
        // Thêm người dùng hiện tại vào danh sách
        _selectedUsers.Add(currentUserEmail);

        var roomDto = new CreateRoomDto
        {
            RoomName = groupName,
            UserRooms = _selectedUsers
        };
            try
            {
                using HttpClient client = new HttpClient();
                string address = Preferences.Default.Get("ServerAddress", "localhost");
                HttpResponseMessage response;
                response = await client.PostAsJsonAsync($"http://{address}:5000/api/Chat/createRoom",roomDto);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", $"Không thể tạo nhóm: {ex.Message}", "OK");
                return;
            }

            await Navigation.PopModalAsync();

    }
}