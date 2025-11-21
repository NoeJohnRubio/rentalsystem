using System;
using System.Data;
using System.Data.SqlClient;

namespace rentalsystem
{
    public static class DataAccess
    {
        // Keep in sync with Form1 connection string
        public static readonly string ConnectionString = "Data Source=NOE\\SQLEXPRESS;Initial Catalog=rentease_db;Integrated Security=True;Encrypt=False;";

        public static DataTable GetProperties()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT property_id, property_name, address, description FROM properties ORDER BY property_id";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int AddProperty(string name, string address, string description)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO properties (property_name, address, description) VALUES (@n, @a, @d); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@a", address);
                cmd.Parameters.AddWithValue("@d", (object)description ?? DBNull.Value);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        public static int GetUserIdByUsername(string username)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT user_id FROM users WHERE username=@u";
                cmd.Parameters.AddWithValue("@u", username);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        public static DataTable GetLeasesForTenantId(int tenantId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT l.lease_id, p.property_name, l.rent_amount, l.start_date, l.end_date, l.status FROM leases l INNER JOIN properties p ON l.property_id = p.property_id WHERE l.tenant_id = @t";
                cmd.Parameters.AddWithValue("@t", tenantId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int AddPayment(int leaseId, decimal amount, string method)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO payments (lease_id, amount, payment_method, status) VALUES (@l, @amt, @m, 'Pending'); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@l", leaseId);
                cmd.Parameters.AddWithValue("@amt", amount);
                cmd.Parameters.AddWithValue("@m", (object)method ?? DBNull.Value);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        public static DataTable GetPaymentsForTenantId(int tenantId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT p.payment_id, p.lease_id, p.amount, p.payment_method, p.payment_date, p.status, p.receipt_url
                                    FROM payments p
                                    INNER JOIN leases l ON p.lease_id = l.lease_id
                                    WHERE l.tenant_id = @t
                                    ORDER BY p.payment_date DESC";
                cmd.Parameters.AddWithValue("@t", tenantId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int AddMaintenanceRequest(int tenantId, int propertyId, string title, string description)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO maintenance_requests (tenant_id, property_id, title, description, status) VALUES (@t, @p, @title, @desc, 'Pending'); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@t", tenantId);
                cmd.Parameters.AddWithValue("@p", propertyId);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        public static DataTable GetMaintenanceRequestsForTenantId(int tenantId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT request_id, property_id, title, description, status, created_at, updated_at
                                    FROM maintenance_requests
                                    WHERE tenant_id = @t
                                    ORDER BY created_at DESC";
                cmd.Parameters.AddWithValue("@t", tenantId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // New: Authentication helpers using hashed passwords
        public static Tuple<bool, string, int, string> ValidateUserCredentialsRaw(string username, string password)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT user_id, role, password FROM users WHERE username=@u";
                cmd.Parameters.AddWithValue("@u", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return Tuple.Create(false, (string)null, 0, (string)null);
                    var userId = reader.GetInt32(0);
                    var role = reader.GetString(1);
                    var stored = reader.GetString(2);

                    // First try hashed verification
                    if (PasswordHelper.VerifyPassword(stored, password))
                    {
                        return Tuple.Create(true, role, userId, stored);
                    }

                    // Fallback: if stored password is plaintext (legacy), allow login and upgrade to hashed password
                    if (string.Equals(stored, password, StringComparison.Ordinal))
                    {
                        var newHash = PasswordHelper.HashPassword(password);
                        try { UpdateUserPassword(userId, newHash); } catch { }
                        return Tuple.Create(true, role, userId, newHash);
                    }

                    return Tuple.Create(false, (string)null, 0, (string)null);
                }
            }
        }

        // Helper wrapper to support C# 7.3 deconstruction without ambiguity in older compiler contexts
        public static (bool success, string role, int userId, string storedHash) ValidateUserCredentials(string username, string password)
        {
            var t = ValidateUserCredentialsRaw(username, password);
            return (t.Item1, t.Item2, t.Item3, t.Item4);
        }

        public static bool UpdateUserPassword(int userId, string newHashedPassword)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET password=@p WHERE user_id=@id";
                cmd.Parameters.AddWithValue("@p", newHashedPassword);
                cmd.Parameters.AddWithValue("@id", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public static DataRow GetUserProfile(int userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT user_id, role, username, full_name, email, contact, created_at FROM users WHERE user_id=@id";
                cmd.Parameters.AddWithValue("@id", userId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public static bool UpdateUserProfile(int userId, string fullName, string email, string contact)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE users SET full_name=@fn, email=@em, contact=@ct WHERE user_id=@id";
                cmd.Parameters.AddWithValue("@fn", fullName);
                cmd.Parameters.AddWithValue("@em", email);
                cmd.Parameters.AddWithValue("@ct", (object)contact ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Admin helpers
        public static DataTable GetUsers()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT user_id, role, username, full_name, email, contact, created_at FROM users ORDER BY user_id";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int CreateUser(string role, string username, string password, string fullName, string email, string contact)
        {
            var hash = PasswordHelper.HashPassword(password);
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO users (role, username, password, full_name, email, contact) VALUES (@role, @username, @password, @fullName, @email, @contact); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", hash);
                cmd.Parameters.AddWithValue("@fullName", fullName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@contact", (object)contact ?? DBNull.Value);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        public static DataTable GetAllPayments()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT p.payment_id, p.lease_id, p.amount, p.payment_method, p.payment_date, p.status, l.tenant_id, t.username AS tenant_username
                                    FROM payments p
                                    LEFT JOIN leases l ON p.lease_id = l.lease_id
                                    LEFT JOIN users t ON l.tenant_id = t.user_id
                                    ORDER BY p.payment_date DESC";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetAllMessages()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT m.message_id, m.sender_id, s.username AS sender_username, m.receiver_id, r.username AS receiver_username, m.content, m.sent_at
                                    FROM messages m
                                    LEFT JOIN users s ON m.sender_id = s.user_id
                                    LEFT JOIN users r ON m.receiver_id = r.user_id
                                    ORDER BY m.sent_at DESC";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static int SendMessage(int senderId, int receiverId, string content)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO messages (sender_id, receiver_id, content) VALUES (@s, @r, @c); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@s", senderId);
                cmd.Parameters.AddWithValue("@r", receiverId);
                cmd.Parameters.AddWithValue("@c", content ?? string.Empty);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                var msgId = obj != null ? Convert.ToInt32(obj) : 0;

                // create a notification for the receiver so they see new message alerts
                try
                {
                    if (msgId > 0 && receiverId > 0)
                    {
                        var senderRow = GetUserProfile(senderId);
                        var senderName = senderRow != null ? senderRow["username"].ToString() : "Someone";
                        var note = $"New message from {senderName}";
                        CreateNotification(receiverId, note, "message");
                    }
                }
                catch { /* swallow notification errors */ }

                return msgId;
            }
        }

        public static DataTable GetAllMaintenanceRequests()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT mr.request_id, mr.tenant_id, u.username AS tenant_username, mr.property_id, p.property_name, mr.title, mr.description, mr.status, mr.created_at, mr.updated_at
                                    FROM maintenance_requests mr
                                    LEFT JOIN users u ON mr.tenant_id = u.user_id
                                    LEFT JOIN properties p ON mr.property_id = p.property_id
                                    ORDER BY mr.created_at DESC";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static bool UpdateMaintenanceRequestStatus(int requestId, string status)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE maintenance_requests SET status=@s, updated_at=GETDATE() WHERE request_id=@id";
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@id", requestId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Returns properties with occupancy status and current tenant info (if occupied)
        public static DataTable GetPropertiesWithStatus()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                // left join to active lease (status = 'active') to determine occupancy
                cmd.CommandText = @"SELECT p.property_id, p.property_name, p.address, p.description,
                                           CASE WHEN l.lease_id IS NULL THEN 'Available' ELSE 'Occupied' END AS availability,
                                           l.lease_id, l.tenant_id, l.rent_amount, l.start_date, l.end_date
                                    FROM properties p
                                    LEFT JOIN leases l ON p.property_id = l.property_id AND LOWER(l.status) = 'active'
                                    ORDER BY p.property_id";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Check if a property currently has an active lease
        public static bool IsPropertyAvailable(int propertyId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(1) FROM leases WHERE property_id=@p AND LOWER(status)='active'";
                cmd.Parameters.AddWithValue("@p", propertyId);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                var count = obj != null ? Convert.ToInt32(obj) : 0;
                return count == 0;
            }
        }

        // Safely create a lease: returns 0 if property not available
        public static int CreateLease(int tenantId, int propertyId, decimal rentAmount, DateTime startDate, DateTime? endDate)
        {
            // Prevent double-assignment
            if (!IsPropertyAvailable(propertyId)) return 0;

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO leases (tenant_id, property_id, rent_amount, start_date, end_date, status) VALUES (@t, @p, @rent, @start, @end, @status); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@t", tenantId);
                cmd.Parameters.AddWithValue("@p", propertyId);
                cmd.Parameters.AddWithValue("@rent", rentAmount);
                cmd.Parameters.AddWithValue("@start", startDate);
                if (endDate.HasValue) cmd.Parameters.AddWithValue("@end", endDate.Value); else cmd.Parameters.AddWithValue("@end", DBNull.Value);
                cmd.Parameters.AddWithValue("@status", "active");
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        // Release (terminate) an active lease for a property or a specific lease id
        public static bool ReleaseLease(int leaseId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE leases SET status=@s, end_date=GETDATE() WHERE lease_id=@id";
                cmd.Parameters.AddWithValue("@s", "terminated");
                cmd.Parameters.AddWithValue("@id", leaseId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Get list of tenants (users with role tenant) - case insensitive
        public static DataTable GetTenants()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT user_id, username, full_name, email, contact FROM users WHERE LOWER(role) = 'tenant' ORDER BY user_id";
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Update payment status (e.g., confirm payment)
        public static bool UpdatePaymentStatus(int paymentId, string newStatus)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE payments SET status=@s, payment_date=GETDATE() WHERE payment_id=@id";
                cmd.Parameters.AddWithValue("@s", newStatus);
                cmd.Parameters.AddWithValue("@id", paymentId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // Convenience: confirm payment (set to 'Paid')
        public static bool ConfirmPayment(int paymentId)
        {
            return UpdatePaymentStatus(paymentId, "Paid");
        }

        // Get a single lease by id with property and tenant info
        public static DataRow GetLeaseById(int leaseId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT l.lease_id, l.tenant_id, u.username AS tenant_username, l.property_id, p.property_name, l.rent_amount, l.start_date, l.end_date, l.status
                                    FROM leases l
                                    LEFT JOIN users u ON l.tenant_id = u.user_id
                                    LEFT JOIN properties p ON l.property_id = p.property_id
                                    WHERE l.lease_id = @id";
                cmd.Parameters.AddWithValue("@id", leaseId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        // Notifications for a user
        public static DataTable GetNotificationsForUser(int userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                if (userId <= 0)
                {
                    // return all notifications for admin overview
                    cmd.CommandText = "SELECT notification_id, user_id, message, [type], is_read, created_at FROM notifications ORDER BY created_at DESC";
                }
                else
                {
                    cmd.CommandText = "SELECT notification_id, user_id, message, [type], is_read, created_at FROM notifications WHERE user_id=@id ORDER BY created_at DESC";
                    cmd.Parameters.AddWithValue("@id", userId);
                }

                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static bool MarkNotificationRead(int notificationId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE notifications SET is_read=1 WHERE notification_id=@id";
                cmd.Parameters.AddWithValue("@id", notificationId);
                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public static int CreateNotification(int userId, string message, string type)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO notifications (user_id, message, [type], is_read) VALUES (@u, @m, @t, 0); SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@m", message ?? string.Empty);
                cmd.Parameters.AddWithValue("@t", type ?? "");
                conn.Open();
                var obj = cmd.ExecuteScalar();
                return obj != null ? Convert.ToInt32(obj) : 0;
            }
        }

        // Messages for a user (inbox and sent) - returns messages where user is sender or receiver
        public static DataTable GetMessagesForUser(int userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT m.message_id, m.sender_id, s.username AS sender_username, m.receiver_id, r.username AS receiver_username, m.content, m.sent_at
                                    FROM messages m
                                    LEFT JOIN users s ON m.sender_id = s.user_id
                                    LEFT JOIN users r ON m.receiver_id = r.user_id
                                    WHERE m.sender_id = @id OR m.receiver_id = @id
                                    ORDER BY m.sent_at DESC";
                cmd.Parameters.AddWithValue("@id", userId);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Convenience: set all leases' rent_amount to a fixed value (e.g., 4500)
        public static bool SetAllLeaseRentsTo(decimal amount)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE leases SET rent_amount = @amt";
                cmd.Parameters.AddWithValue("@amt", amount);
                conn.Open();
                // ExecuteNonQuery returns number of rows affected; consider success if >=0
                var rows = cmd.ExecuteNonQuery();
                return rows >= 0;
            }
        }

        // Update rent for a specific lease and notify tenant
        public static bool UpdateLeaseRent(int leaseId, decimal newRent)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                // get tenant id first
                cmd.CommandText = "SELECT tenant_id FROM leases WHERE lease_id=@id";
                cmd.Parameters.AddWithValue("@id", leaseId);
                conn.Open();
                var obj = cmd.ExecuteScalar();
                int tenantId = obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
                cmd.Parameters.Clear();

                // update rent
                cmd.CommandText = "UPDATE leases SET rent_amount=@amt WHERE lease_id=@id";
                cmd.Parameters.AddWithValue("@amt", newRent);
                cmd.Parameters.AddWithValue("@id", leaseId);
                var rows = cmd.ExecuteNonQuery();

                // notify tenant if present and update succeeded
                if (rows > 0 && tenantId > 0)
                {
                    try
                    {
                        var note = $"Your rent has been updated to {newRent:C}";
                        CreateNotification(tenantId, note, "rent_change");
                    }
                    catch { }
                }

                return rows == 1;
            }
        }
    }
}