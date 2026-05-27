using ShareModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Mot_xin_gor;

public partial class HomePage : ContentPage
{
    public ObservableCollection<ChatConversation> Conversations { get; set; } = new();
    public ObservableCollection<ShareModel.Messages> MessagesCollection { get; set; } = new();
    public HomePage()
	{
		InitializeComponent();
        lstRooms.ItemsSource = Conversations;
        lstMessages.ItemsSource = MessagesCollection;
        this.BindingContext = this;
    }

	protected async override void OnAppearing()
	{
        base.OnAppearing();
        await LoadRoomsAsync();
    }

    private async Task LoadRoomsAsync()
    {
        string currentUserId = Preferences.Default.Get("CurrentUserId", string.Empty);
        if (string.IsNullOrEmpty(currentUserId))
            return;

        Debug.WriteLine(currentUserId);

        var dataRoom = await LoadChatConversations(currentUserId);

        Conversations.Clear();
        foreach (var item in dataRoom)
        {
            Conversations.Add(item);
            Debug.WriteLine(item.displayName);
        }
    }

    private async Task<List<ChatConversation>> LoadChatConversations(string currentUserId)
	{
		try
		{
			using(HttpClient client = new HttpClient())
			{
				HttpResponseMessage response;
				string address = Preferences.Default.Get("ServerAddress", string.Empty);
                Debug.WriteLine(address);
                if (!string.IsNullOrWhiteSpace(address))
                {
                    response = await client.GetAsync(
                        $"http://{address}:5000/api/Chat/loadFriendList?currentUserId={currentUserId}");
                }
                else
                {
                    response = await client.GetAsync(
                        $"http://localhost:7051/api/Chat/loadFriendList?currentUserId={currentUserId}");
                }

                Debug.WriteLine(response);
                if (response.IsSuccessStatusCode)
                { 
                    var result = await response.Content.ReadFromJsonAsync<List<ChatConversation>>();
                    Debug.WriteLine(result);
                    return result ?? new List<ChatConversation>();
                }
                else
                {
                    return new List<ChatConversation>();
                }

            }
		}catch (Exception ex)
		{
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            return new List<ChatConversation>();
        }
	}

    private async void UserSelection(object sender, TappedEventArgs e)
    {
        var tappedGrid = sender as Grid;
        if (tappedGrid != null)
        {
            var conversation = tappedGrid.BindingContext as ChatConversation;
            if (conversation != null)
            {
                Preferences.Default.Set("CurrentRoomId", conversation.rId);
                lblChatHeaderName.Text = conversation.displayName;
                await LoadMessagesCollection(conversation.rId);
            }
        }
    }
    private async Task LoadMessagesCollection(string roomId)
    {
        try
        {
            using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetFromJsonAsync<List<ShareModel.Messages>>(
                                    $"http://localhost:5000/api/Chat/loadMessages?currentRoomId={roomId}"); ;
                        string address = Preferences.Default.Get("ServerAddress", string.Empty);
                        if (!string.IsNullOrEmpty(address))
                        {
                            response = await client.GetFromJsonAsync<List<ShareModel.Messages>>(
                                    $"http://{address}:5000/api/Chat/loadMessages?currentRoomId={roomId}");
                        }
                        if (response != null)
                        {
                            MessagesCollection.Clear();
                            foreach (var msg in response)
                            {
                                MessagesCollection.Add(msg);
                            }
                            Debug.WriteLine($"Messages loaded: {MessagesCollection.Count}");
                }
            }
        }catch(Exception ex)
        {
            Debug.WriteLine($"Lỗi tải tin nhắn: {ex.Message}");
        }
        
    }

    private async void SendMessage(object sender, EventArgs e)
    {
        // Kiểm tra sender là Button
        if (sender is Button btn)
        {
            Debug.WriteLine("Nút gửi tin nhắn được bấm.");
            string currentUserId = Preferences.Default.Get("CurrentUserId", string.Empty);
            string currentRoomId = Preferences.Default.Get("CurrentRoomId", string.Empty);
            if (string.IsNullOrEmpty(currentRoomId))
            {
                await DisplayAlert("Thông báo", "Vui lòng chọn một phòng chat trước khi gửi.", "OK");
                return;
            }

            // 1. Xác định nội dung: Nếu Entry rỗng thì gửi Like
            string content = string.IsNullOrWhiteSpace(messageInput.Text) ? "👍" : messageInput.Text;

            var newMessage = new ShareModel.Messages
            {
                MessageType = MessageType.Text,
                RId = currentRoomId,
                UId = currentUserId,
                Content = content,
                Timestamp = DateTime.Now // Dùng giờ hiện tại của hệ thống
            };

            Debug.WriteLine($"Đang gửi tin nhắn: {newMessage.Content} từ User: {newMessage.UId} trong Room: {newMessage.RId}");

            try
            {
                btn.IsEnabled = false;
                using (HttpClient client = new HttpClient())
                {
                    // Ưu tiên lấy địa chỉ Server từ Preferences
                    string address = Preferences.Default.Get("ServerAddress", "10.0.2.2");
                    string url = $"http://{address}:5000/api/Chat/sendMessage";

                    // Vô hiệu hóa nút để tránh bấm liên tục (Spam)
                    btn.IsEnabled = false;

                    var response = await client.PostAsJsonAsync(url, newMessage);
                    Debug.WriteLine($"Phản hồi từ Server: {response.StatusCode}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Lấy dữ liệu tin nhắn đã được Server lưu (có ID thật)
                        var savedMsg = await response.Content.ReadFromJsonAsync<ShareModel.Messages>();

                        // Cập nhật danh sách hiển thị
                        MessagesCollection.Add(savedMsg ?? newMessage);

                        // Xóa nội dung trong Entry sau khi gửi
                        messageInput.Text = string.Empty;

                        // Tự động cuộn xuống cuối danh sách tin nhắn
                        if (MessagesCollection.Count > 0)
                        {
                            lstMessages.ScrollTo(MessagesCollection.Last(), position: ScrollToPosition.End);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi gửi tin nhắn: {ex.Message}");
                await DisplayAlert("Lỗi", "Không thể gửi tin nhắn. Vui lòng kiểm tra kết nối.", "OK");
            }
            finally
            {
                // Kích hoạt lại nút bấm
                btn.IsEnabled = true;
            }
        }
    }

    private async void ChooseImage(object sender, EventArgs e)
    {
        string roomId = Preferences.Default.Get("CurrentRoomId", string.Empty);

        if (string.IsNullOrEmpty(roomId))
        {
            await DisplayAlert("Lỗi", "Chưa chọn phòng chat", "OK");
            return;
        }

        await SendImage(roomId);
    }

    private async Task SendImage(string roomId)
    {
        if (string.IsNullOrEmpty(roomId))
        {
            await DisplayAlert("Lỗi", "Chưa chọn phòng chat", "OK");
            return;
        }

        var photo = await PickImageAsync();
        if (photo == null) return;

        using var stream = await photo.OpenReadAsync();

        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "file", photo.FileName);
        Debug.WriteLine(photo.FileName);
        content.Add(new StringContent(roomId), "roomId");
        content.Add(new StringContent(Preferences.Default.Get("CurrentUserId", string.Empty)), "userId");

        using HttpClient client = new HttpClient();
        string address = Preferences.Default.Get("ServerAddress", "localhost");

        var response = await client.PostAsync(
            $"http://{address}:5000/api/Chat/sendImage",
            content);

        if (response.IsSuccessStatusCode)
        {
            Debug.WriteLine("Ảnh đã được gửi thành công.");
            await LoadMessagesCollection(roomId);
        }
        else
        {
            Debug.WriteLine("Lỗi khi gửi ảnh.");
        }
    }

    private async Task<FileResult?> PickImageAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            return null;

        var results = await MediaPicker.Default.PickPhotosAsync(
            new MediaPickerOptions
            {
                Title = "Chọn ảnh"
            });

        return results?.FirstOrDefault();
    }


    private void OnMessageTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            btnSendOrLike.Text = "▶️";
        }
        else
        {
            btnSendOrLike.Text = "👍";
        }
    }

    private async void OnCreateGroupClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new CreateGroupPage());
    }

    private async void LeaveRoom(object sender, EventArgs e)
    {
        string currentRoomId = Preferences.Default.Get("CurrentRoomId", string.Empty);
        string currentUserId = Preferences.Default.Get("CurrentUserId", string.Empty);
        if (string.IsNullOrEmpty(currentRoomId))
        {
            await DisplayAlert("Thông báo", "Vui lòng chọn một phòng chat để rời khỏi.", "OK");
            return;
        }
        using HttpClient client = new HttpClient();
        string address = Preferences.Default.Get("ServerAddress", "localhost");
        var response = await client.DeleteAsync(
            $"http://{address}:5000/api/Chat/leaveRoom?roomId={currentRoomId}&userId={currentUserId}");
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Thông báo", "Bạn đã rời khỏi phòng chat.", "OK");
            Conversations.Clear();
            MessagesCollection.Clear();
            lblChatHeaderName.Text = "Chọn phòng chat";
            await LoadRoomsAsync();
        }
        else
        {
            await DisplayAlert("Lỗi", "Không thể rời khỏi phòng chat. Vui lòng thử lại.", "OK");
        }
    }

    private async void OnCallClicked(object sender, EventArgs e)
    {
        string currentRoomId = Preferences.Default.Get("CurrentRoomId", string.Empty);
        if (string.IsNullOrEmpty(currentRoomId))
        {
            await DisplayAlert("Info", "Please select a room first.", "OK");
            return;
        }

        var win = new Window(new NavigationPage(new NewWindow1(currentRoomId)));
        Application.Current.OpenWindow(win);
    }

}