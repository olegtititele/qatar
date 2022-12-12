using Npgsql;

namespace PostgreSQL
{
    public static class DB
    {
        private static string db_connection = DBConfig.users_db;
        private static string blacklist_db = DBConfig.blacklist_db;

        public static List<long> GetAllUsersId()
        {
            List<long> usersID= new List<long>();
            using var con = new NpgsqlConnection(db_connection);
            con.Open();


            var sql = $"SELECT user_id FROM users_table";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    usersID.Add(reader.GetInt64(0));
                }
                con.Close();
                return usersID;
            }
        }

        public static void DropUserTable()
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"DROP TABLE users_table";
            cmd.ExecuteNonQuery();
        }

        public static void UpdateStates()
        {
            foreach(var userId in GetAllUsersId())
            {
                try
                {
                    if(GetParser(userId)=="Start")
                    {
                        UpdateParser(userId, "Stop");
                    }
                }
                catch(Exception){ }
            }
        }

        // User
        public static void CreateUsersTable()
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS users_table (user_id BIGINT PRIMARY KEY, username TEXT, start_page INT, seller_type TEXT, whatsapp_text TEXT, state TEXT, parser TEXT, link TEXT, parser_category TEXT, seller_total_ads TEXT, seller_reg_date TEXT, seller_rating REAL, platform TEXT, blacklist TEXT, timeout INT)";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void CreateUser(long userId, string username)
        {
            string whatsappText = "Hello, I want to buy this. In a good condition? @adlink";
            int startPage = 1;
            string sellerType = "Частное лицо";
            string state = "MainMenu";
            string parser = "Stop";
            string link = "null";
            string parserCategory = "all";
            string platform = "null";
            string sellerTotalAds = "Отключить";
            string sellerRegDate = "Отключить";
            string blacklist = "Включить";
            double sellerRating = 5;
            int timeout = 10;
            
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO users_table (user_id, username, start_page, seller_type, whatsapp_text, state, parser, link, seller_total_ads, seller_reg_date, seller_rating, platform, blacklist, timeout) VALUES (@user_id, @username, @start_page, @seller_type, @whatsapp_text, @state, @parser, @link, @seller_total_ads, @seller_reg_date, @seller_rating, @platform, @blacklist, @timeout)";
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@start_page", startPage);
            cmd.Parameters.AddWithValue("@seller_type", sellerType);
            cmd.Parameters.AddWithValue("@whatsapp_text", whatsappText);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@parser", parser);
            cmd.Parameters.AddWithValue("@link", link);
            cmd.Parameters.AddWithValue("@parser_category", parserCategory);
            cmd.Parameters.AddWithValue("@seller_total_ads", sellerTotalAds);
            cmd.Parameters.AddWithValue("@seller_reg_date", sellerRegDate);
            cmd.Parameters.AddWithValue("@seller_rating", sellerRating);
            cmd.Parameters.AddWithValue("@platform", platform);
            cmd.Parameters.AddWithValue("@blacklist", blacklist);
            cmd.Parameters.AddWithValue("@timeout", timeout);
            
            cmd.ExecuteNonQuery();  
        }

        public static bool CheckUser(long user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();
                var sql = $"SELECT * FROM users_table WHERE user_id = '{user_id}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetState(long user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(db_connection);
                con.Open();
                var sql = $"SELECT state FROM users_table WHERE user_id = '{user_id}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar();
                return result!.ToString()!;
            }
            catch(Exception)
            {
                return "MainMenu";
            }
        }
        public static void UpdateState(long user_id, string state)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET state = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", user_id);
                    command.Parameters.AddWithValue("q", state);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        public static string GetParser(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT parser FROM users_table WHERE user_id = '{userId}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateParser(long userId, string parserState)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET parser = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", parserState);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        public static string GetParserCategory(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            var sql = $"SELECT parser_category FROM users_table WHERE user_id = '{userId}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateParserCategory(long userId, string parserState)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET parser_category = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", parserState);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }
        
        public static string GetPlatform(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT Platform FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }


        public static void UpdatePlatform(long user_id, string platform)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET platform = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", user_id);
                    command.Parameters.AddWithValue("q", platform);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }


        public static string GetLink(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT Link FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateLink(long userId, string link)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET link = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", link);
                    int nRows = command.ExecuteNonQuery();                    
                }
            }
        }

        public static decimal GetSellerRating(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT seller_rating FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return decimal.Parse(result!.ToString()!);
        }



        public static void UpdateSellerRating(long userId, double sellerRating)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET seller_rating = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", sellerRating);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        public static string GetSellerTotalAds(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT seller_total_ads FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateSellerTotalAds(long userId, string sellerAdv)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET seller_total_ads = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", sellerAdv);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }

        public static int GetTimeout(long userId)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT timeout FROM users_table WHERE user_id = '{userId}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return Int32.Parse(result!.ToString()!);
        }

        public static void UpdateTimeout(long userId, int timeout)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET timeout = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", timeout);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }


        public static string GetSellerRegDate(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT seller_reg_date FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateSellerRegDate(long userId, string sellerRegData)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET seller_reg_date = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", userId);
                    command.Parameters.AddWithValue("q", sellerRegData);
                    int nRows = command.ExecuteNonQuery();
                    
                }
            }
        }
        

        public static string GetSellerType(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT seller_type FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!;
        }

        public static void UpdateSellerType(long user_id)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                if(GetSellerType(user_id) == "Частное лицо")
                {
                    using (var command = new NpgsqlCommand("UPDATE users_table SET seller_type = @q WHERE user_id = @n", conn))
                    {
                        command.Parameters.AddWithValue("n", user_id);
                        command.Parameters.AddWithValue("q", "Все типы аккаунтов");
                        int nRows = command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var command = new NpgsqlCommand("UPDATE users_table SET seller_type = @q WHERE user_id = @n", conn))
                    {
                        command.Parameters.AddWithValue("n", user_id);
                        command.Parameters.AddWithValue("q", "Частное лицо");
                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static string GetWhatsappText(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();
            
            var sql = $"SELECT whatsapp_text FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            if(result == null)
            {
                return "";
            }
            return result!.ToString()!;
                
        }
        
        public static void UpdateWhatsappText(long user_id, string whatsapp_text)
        {
            using (var conn = new NpgsqlConnection(db_connection))
            {
                
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET whatsapp_text = @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", user_id);
                    command.Parameters.AddWithValue("q", whatsapp_text);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }


        public static string GetBlackList(long user_id)
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            var sql = $"SELECT blacklist FROM users_table WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result!.ToString()!; 
        }

        public static void UpdateBlackList(long user_id)
        {
            string blackListValue;
            if(GetBlackList(user_id) == "Включить")
            {
                blackListValue = "Выключить";
            }
            else
            {
                blackListValue = "Включить";
            }
            using (var conn = new NpgsqlConnection(db_connection))
            {
                conn.Open();

                using (var command = new NpgsqlCommand("UPDATE users_table SET blacklist= @q WHERE user_id = @n", conn))
                {
                    command.Parameters.AddWithValue("n", user_id);
                    command.Parameters.AddWithValue("q", blackListValue);
                    int nRows = command.ExecuteNonQuery();
                }
            }
        }

        // BlackListDB

        public static void CreateBlackListTable(long user_id)
        {
            using var con = new NpgsqlConnection(blacklist_db);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS blacklist{user_id} (platform TEXT, ad_link TEXT, seller_link TEXT, seller_phone TEXT)";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static void DropBlacklistTable(long user_id)
        {
            using var con = new NpgsqlConnection(blacklist_db);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"TRUNCATE TABLE blacklist{user_id}";
            cmd.ExecuteNonQuery();
        }

        public static void CreateBlacklistLinksTable()
        {
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS blacklist_links (user_id BIGINT, link TEXT)";
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static List<string> GetUserBlacklistLinks(long user_id)
        {
            List<string> links= new List<string>();
            using var con = new NpgsqlConnection(db_connection);
            con.Open();


            var sql = $"SELECT link FROM blacklist_links WHERE user_id = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    links.Add(reader.GetString(0));
                }
                con.Close();
                return links;
            }
        }

        public static void InsertNewLink(long userId, string link)
        {   
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO blacklist_links (user_id, link) VALUES (@user_id, @link)";
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@link", link);
            
            cmd.ExecuteNonQuery();  
        }

        public static void DeleteAllLinks(long userId)
        {   
            using var con = new NpgsqlConnection(db_connection);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"DELETE FROM blacklist_links WHERE user_id = '{userId}'";
            cmd.ExecuteNonQuery();  
        }


        public static int BlacklistLength(long user_id)
        {
            using var con = new NpgsqlConnection(blacklist_db);
            con.Open();
            var sql = $"SELECT DISTINCT seller_phone FROM blacklist{user_id}";
            using var cmd = new NpgsqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            int length = 0;
            while (reader.Read())
            {
                length++;
            }
            return length;
        }

        public static bool CheckAdvestisement(long user_id, string adLink)
        {
            try
            {
                using var con = new NpgsqlConnection(blacklist_db);
                con.Open();
                var sql = $"SELECT * FROM blacklist{user_id} WHERE ad_link = '{adLink}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckPhoneNumber(long user_id, string sellerPhone)
        {
            try
            {
                using var con = new NpgsqlConnection(blacklist_db);
                con.Open();
                var sql = $"SELECT * FROM blacklist{user_id} WHERE seller_phone = '{sellerPhone}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar()!.ToString();
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        // public static bool CheckSellerlink(long user_id, string seller_link)
        // {
        //     try
        //     {
        //         using var con = new NpgsqlConnection(db_connection);
        //         con.Open();
                
        //         var sql = $"SELECT * FROM a{user_id} WHERE seller_link = '{seller_link}'";
        //         using var cmd = new NpgsqlCommand(sql, con);
        //         var result = cmd.ExecuteScalar()!.ToString();
        //         con.Close();
        //         return true;
        //     }
        //     catch (Exception)
        //     {
        //         return false;
        //     }
        // }


        public static void AddAdvertisementToBlackList(long user_id, string platform, string adLink, string sellerLink, string phoneNumber)
        {
            using var con = new NpgsqlConnection(blacklist_db);
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO blacklist{user_id} (platform, ad_link, seller_link, seller_phone) VALUES (@platform, @ad_link, @seller_link, @seller_phone)";
            cmd.Parameters.AddWithValue("@platform", platform);
            cmd.Parameters.AddWithValue("@ad_link", adLink);
            cmd.Parameters.AddWithValue("@seller_link", sellerLink);
            cmd.Parameters.AddWithValue("@seller_phone", phoneNumber);
            cmd.ExecuteNonQuery();
        }
    }
}