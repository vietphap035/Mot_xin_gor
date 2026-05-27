using Microsoft.EntityFrameworkCore;
using ShareModel; // using dự án ShareModel của bạn

namespace API_CHAT.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor này là BẮT BUỘC để Program.cs hoạt động
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo 4 bảng của bạn
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }

        // Hàm này để cấu hình các mối quan hệ (Khóa chính, Index...)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình cho bảng nối UserRoom (Mối quan hệ N-N) ---

            // 1. Định nghĩa Khóa chính tổng hợp (UserId + RoomId)
            modelBuilder.Entity<UserRoom>()
                .HasKey(ur => new { ur.UId, ur.RId });

            // 2. Cấu hình mối quan hệ (User -> UserRoom -> Room)
            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRooms) // Trỏ đến ICollection<UserRoom> trong User.cs
                .HasForeignKey(ur => ur.UId);

            // 3. Cấu hình mối quan hệ (Room -> UserRoom -> User)
            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.Room)
                .WithMany(r => r.UserRooms) // Trỏ đến ICollection<UserRoom> trong Room.cs
                .HasForeignKey(ur => ur.RId);


            // --- Cấu hình cho Message (Mối quan hệ 1-N) ---

            // 1. Cấu hình quan hệ User gửi Message
            modelBuilder.Entity<Messages>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages) // Trỏ đến ICollection<Message> trong User.cs
                .HasForeignKey(m => m.UId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Không xóa tin nhắn khi xóa user

            // 2. Cấu hình quan hệ Room chứa Message
            modelBuilder.Entity<Messages>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Messages) // Trỏ đến ICollection<Message> trong Room.cs
                .HasForeignKey(m => m.RId);


            // --- Thêm Index để tối ưu tốc độ truy vấn ---
            modelBuilder.Entity<Messages>()
                .HasIndex(m => new { m.RId, m.Timestamp })
                .HasDatabaseName("IX_Messages_RoomId_SendTime");
        }
    }
}