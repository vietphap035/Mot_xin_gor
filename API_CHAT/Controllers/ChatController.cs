using API_CHAT.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareModel;
using System.Collections.Generic;

namespace API_CHAT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private ApplicationDbContext _context;
        public ChatController(ILogger<ChatController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("loadFriendList")] // similar to loadRoom
        public async Task<List<ChatConversation>> GetConversationsAsync(string currentUserId)
        {
            var rooms = await _context.Rooms
                    .Where(r => r.UserRooms.Any(ur => ur.UId == currentUserId)) // Lọc những phòng user tham gia
                    .Select(r => new
                    {
                        r.RId,
                        r.RoomName,
                        // Chỉ lấy thông tin của tin nhắn mới nhất từ DB
                        LatestMessage = r.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => new { m.Content, m.Timestamp })
                        .FirstOrDefault()
                    })
                .OrderByDescending(x => x.LatestMessage != null ? x.LatestMessage.Timestamp : DateTime.MinValue)
                .ToListAsync();
            return rooms.Select(r => new ChatConversation
            {
                rId = r.RId,
                displayName = r.RoomName,
                lastMessage = r.LatestMessage?.Content ?? "Chưa có tin nhắn",
                timeAgo = r.LatestMessage != null
            ? CalculateTimeAgo(r.LatestMessage.Timestamp)
            : ""
            }).ToList();

        }

        private string CalculateTimeAgo(DateTime timestamp)
        {
            var diff = DateTime.UtcNow - timestamp;
            if (diff.TotalMinutes < 1) return "Vừa xong";
            if (diff.TotalHours < 1) return $"{diff.Minutes} phút";
            if (diff.TotalDays < 1) return $"{diff.Hours} giờ";
            return $"{diff.Days} ngày";
        }


        [HttpGet("loadMessages")]
        public async Task<List<Messages>> GetMessagesAsync(string currentRoomId)
        {
            var messages = await _context.Messages
                .Where(r => r.RId == currentRoomId)
                // Sắp xếp giảm dần để lấy các tin nhắn MỚI NHẤT trước
                .OrderByDescending(m => m.Timestamp)
                .Take(20) // Giới hạn số lượng tin nhắn tải về
                .Select(m => new Messages
                {
                    MId = m.MId,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    UId = m.UId,
                    RId = m.RId,
                    MessageType = m.MessageType,
                    Url = m.Url
                })
                .ToListAsync();
            // Sau khi lấy được N tin nhắn mới nhất, ta đảo ngược lại 
            // để hiển thị đúng thứ tự thời gian từ cũ đến mới trên màn hình
            return messages.OrderBy(m => m.Timestamp).ToList();
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> sendMessage([FromBody] SendMessageDto messages)
        {
            try
            {
                // 1. Gán ID mới  và thời gian server
                var message = new Messages
                {
                    MId = Guid.NewGuid().ToString(),
                    UId = messages.UId,
                    RId = messages.RId,
                    Content = messages.Content,
                    Url = null,
                    Timestamp = DateTime.Now
                };
                // 2. Thêm vào Context
                _context.Messages.Add(message);
                // 3. Lưu thay đổi xuống Database
                await _context.SaveChangesAsync();

                // 4. Trả về thông tin tin nhắn vừa tạo thành công
                return Ok(messages);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPost("sendImage")]
        public async Task<IActionResult> SendImage([FromForm] IFormFile file, [FromForm] string roomId, [FromForm] string userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine("wwwroot/images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            // Lưu message
            var message = new ShareModel.Messages
            {
                MId = Guid.NewGuid().ToString(),
                RId = roomId,
                UId = userId,
                Content = null,
                MessageType = MessageType.Image,
                Url = $"/images/{fileName}",
                Timestamp = DateTime.Now
            };


            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("createRoom")]
        public async Task<IActionResult> createRoom([FromBody] CreateRoomDto createRoomDto)
        {
            var numberOfUsersFail = 0;
            var RID = Guid.NewGuid().ToString();
            try
            {
                var room = new Room
                {
                    RId = RID,
                    RoomName = createRoomDto.RoomName
                };
                _context.Rooms.Add(room);

                foreach (var email in createRoomDto.UserRooms)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        numberOfUsersFail++;
                    }
                    else
                    {
                        var userRoom = new UserRoom
                        {
                            UId = user.UId,
                            RId = RID
                        };
                        _context.UserRooms.Add(userRoom);
                    }
                }
                if (numberOfUsersFail == createRoomDto.UserRooms.Count)
                {
                    _context.Rooms.Remove(room);
                    return BadRequest("Không tìm thấy người dùng hợp lệ nào để thêm vào phòng.");
                }
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    RoomId = RID,
                    FailCount = numberOfUsersFail,
                    Total = createRoomDto.UserRooms.Count - numberOfUsersFail
                });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpDelete("leaveRoom")]
        public async Task<IActionResult> LeaveRoom(string roomId, string userId)
        {
            try
            {
                var userRoom = await _context.UserRooms
                    .FirstOrDefaultAsync(ur => ur.RId == roomId && ur.UId == userId);
                if (userRoom == null)
                {
                    return NotFound("Người dùng không thuộc phòng này.");
                }
                _context.UserRooms.Remove(userRoom);
                await _context.SaveChangesAsync();
                return Ok("Rời phòng thành công.");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
