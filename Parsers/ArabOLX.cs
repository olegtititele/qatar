using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;


using Modules;
using PostgreSQL;

namespace Parser
{
    public class ArabOLX
    {
        private static string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";
        private static string errorImageUri = "https://upload.wikimedia.org/wikipedia/commons/9/9a/%D0%9D%D0%B5%D1%82_%D1%84%D0%BE%D1%82%D0%BE.png";
        public static void StartParsing(ITelegramBotClient botClient, long userId, DateTime userExactTime)
        {
            HtmlWeb web = new HtmlWeb();
            DateTime exactTime = userExactTime;
            string userPlatform = DB.GetPlatform(userId);
            string userLink = DB.GetLink(userId);
            string userSellerTotalAds = DB.GetSellerTotalAds(userId);
            string userSellerRegDate = DB.GetSellerRegDate(userId);
            decimal userSellerRating = DB.GetSellerRating(userId);
            string userSellerType = DB.GetSellerType(userId);
            string blacklist = DB.GetBlackList(userId);
            string parserCategory = DB.GetParserCategory(userId);
            List<string> blacklistCategories = DB.GetUserBlacklistLinks(userId);
            int timeout = DB.GetTimeout(userId)*1000;
            web.UserAgent = userAgent;

            try
            {
                string domen = "";
                string currency = "";
                string telCode = "";

                switch(userPlatform)
                {
                    case "olx.qa":
                        domen = "https://www.olx.qa";
                        currency = "QAR";
                        telCode = "974";
                        break;
                    case "olx.com.om":
                        domen = "https://www.olx.com.om";
                        currency = "OMR";
                        telCode = "968";
                        break;
                    case "olx.com.bh":
                        domen = "https://www.olx.com.bh";
                        currency = "BHD";
                        telCode = "973";
                        break;
                    case "olx.com.kw":
                        domen = "https://www.olx.com.kw";
                        currency = "KWD";
                        telCode = "965";
                        break;

                }

                List<string> phoneNumbers = new List<string>();
                List<string> passedLinks = new List<string>();
  
                while(true)
                {
                    try
                    {
                        int page = 1;
                        ParseCategory(web, botClient, userId, page, passedLinks, phoneNumbers, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories, domen, currency, telCode);
                    }
                    catch{ }

                    System.Threading.Thread.Sleep(timeout);
                }
            }
            catch
            {
                DB.UpdateParser(userId, "Stop");
                return;
            }
        }

        private static void ParseCategory(HtmlWeb web, ITelegramBotClient botClient, long userId, int page, List<string> passedLinks, List<string> phoneNumbers, DateTime exactTime, string userPlatform, string userLink, string userSellerTotalAds, string userSellerRegDate, decimal userSellerRating, string userSellerType, string blacklist, string parserCategory, List<string> blacklistCategories, string domen, string currency, string telCode)
        {
            string adLink = "";
            string categoryLink = GenerateLink(page, parserCategory, domen, userLink);
            HtmlDocument document = web.Load(categoryLink);
                    
            var advertisements = document.DocumentNode.SelectNodes("//li[@aria-label=\"Listing\"]");
            if(advertisements != null)
            {
                foreach (HtmlNode advertisement in advertisements)
                {
                    
                    if(DB.GetParser(userId) == "Start")
                    {
                        try
                        {
                            var aaa = advertisement.SelectSingleNode(".//div[@aria-label=\"Featured\"]//span[@class=\"_151bf64f\"]").InnerHtml;
                            continue;
                        }
                        catch
                        {
                            adLink = domen + advertisement.SelectSingleNode(".//div[@class=\"ee2b0479\"]//a").GetAttributeValue("href", "");
                            
                            if(passedLinks.Contains(adLink))
                            {
                                return;
                            }
                            
                            else
                            {
                                passedLinks.Add(adLink);
                                if(!DB.CheckAdvestisement(userId, adLink))
                                {
                                    if(!ParsePageInfo(web, botClient, userId, adLink, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories, domen, currency, telCode, phoneNumbers))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        passedLinks = new List<string>();
                        DB.UpdateParser(userId, "Stop");
                        return;
                    }
                }
                page++;
                ParseCategory(web, botClient, userId, page, passedLinks, phoneNumbers, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories, domen, currency, telCode);
            }
        }

        static bool ParsePageInfo(HtmlWeb web, ITelegramBotClient botClient, long userId, string adLink, DateTime exactTime, string userPlatform, string userLink, string userSellerTotalAds, string userSellerRegDate, decimal userSellerRating, string userSellerType, string blacklist, string parserCategory, List<string> blacklistCategories, string domen, string currency, string telCode, List<string> phoneNumbers)
        {
            string adCategory = "";
            string adPrice = "";
            string adTitle = "";
            string adImage = "";
            string sellerType = "";
            string sellerLink = "";
            string sellerName = "";
            string adDescription = "";
            string adLocation = "";
            int sellerTotalAds = 1;
            DateTime adRegDate = DateTime.Today;
            DateTime sellerRegDate = DateTime.Today;

            
            HtmlDocument document = web.Load(adLink);

            var categories = document.DocumentNode.SelectNodes("//li[@itemprop=\"itemListElement\"]");
            
            foreach(var category in categories)
            {
                try
                {
                    adCategory = domen + category.SelectSingleNode(".//a[@itemprop=\"item\"]").GetAttributeValue("href", "");
                    if(blacklistCategories.Contains(adCategory))
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            string adId = adLink.Split("ID")[1].Split('.')[0];

            var sellerPhoneNumber = GetPhoneNumber(adId, domen, telCode).Result;

            if(sellerPhoneNumber == "null")
            {
                return true;
            }

            if(Functions.CheckBlacklistAds(userId, sellerPhoneNumber, blacklist)){ }else{ return true; }

            var scripts = document.DocumentNode.SelectNodes("//script");

            foreach (HtmlNode script in scripts)
            {
                if(script.InnerHtml.Contains("priceValidUntil"))
                {
                    string json = script.InnerText;
                    JObject jObject = JObject.Parse(json);
                    adRegDate = Convert.ToDateTime(jObject["offers"]![0]!["priceValidUntil"]!.ToString()).AddDays(-30).AddHours(3);
                    break;
                }
            }

            if(Functions.CheckAdRegDate(exactTime, adRegDate)){  }else{ return false; }

            foreach (HtmlNode script in scripts)
            {
                if(script.InnerHtml.Contains("window['dataLayer'] = window['dataLayer'] || [];window['dataLayer'].push("))
                {
                    string json = script.InnerHtml.Split("window['dataLayer'] = window['dataLayer'] || [];window['dataLayer'].push(")[1];
                    char[] MyChar = {';',')'};
                    json = json.TrimEnd(MyChar);
                    JObject jObject = JObject.Parse(json);

                    try
                    {
                        adPrice = Functions.ConvertPrice(jObject["price"]!.ToString(), currency);
                    }
                    catch
                    {
                        adPrice = "Не указана";
                    }

                    adTitle = jObject["ad_title"]!.ToString();
                    sellerType = jObject["seller_type"]!.ToString();

                    if(sellerType == "individual")
                    {
                        sellerType = "Частное лицо";
                    }
                    else
                    {
                        sellerType = "Бизнесс аккаунт";
                    }
                    break;
                }
            }

            if(Functions.CheckSellerType(userSellerType, sellerType)){ }else{ return true; }

            foreach (HtmlNode script in scripts)
            {
                if(script.InnerHtml.Contains("window.state = "))
                {
                    string json = script.InnerHtml.Split("window.state = ")[1].Split("\n")[0];
                    char[] MyChar = {';',')'};
                    json = json.TrimEnd(MyChar);
                    JObject jObject = JObject.Parse(json);

                    var sellerProfile = jObject["sellerProfile"]!;
                    string sellerId = sellerProfile["data"]!["externalID"]!.ToString();
                    sellerLink = $"{domen}/en/profile/{sellerId}";
                    sellerName = sellerProfile["data"]!["name"]!.ToString();
                    sellerRegDate = Convert.ToDateTime(sellerProfile["data"]!["createdAt"]!.ToString());
                    break;
                }
            }

            if(Functions.CheckSellerRegDate(userSellerRegDate, sellerRegDate)){ }else{ return true; }

            try
            {
                HtmlDocument sellerDocument = web.Load(sellerLink);
                
                sellerTotalAds = Functions.LeaveOnlyNumbers(sellerDocument.DocumentNode.SelectSingleNode("//span[@class=\"_34a7409b _6a44af43 _2e82a662\"]").InnerText);
            }
            catch
            {
                sellerTotalAds = 1;
            }
            
            if(Functions.CheckSellerTotalAds(userSellerTotalAds, sellerTotalAds)){  }else{ return true; }

            
            try
            {
                adDescription = document.DocumentNode.SelectSingleNode("//div[@class=\"_0f86855a\"]//span").InnerText;
            }
            catch
            {
                adDescription = "Не указано";
            }

            try
            {
                adLocation = document.DocumentNode.SelectSingleNode("//span[@aria-label=\"Location\"]").InnerText;
            }
            catch
            {
                adLocation = "Не указано";
            }

            try
            {
                adImage = document.DocumentNode.SelectSingleNode("//img[@role=\"presentation\"]").GetAttributeValue("src", "").Replace("240x180.jpeg", "400x300.jpeg");
            }
            catch
            {
                adImage = errorImageUri;
            }
            
            
            Functions.AddToBlacklist(userId, userPlatform!, adLink, sellerLink, sellerPhoneNumber);

            SendLogToTg(botClient, userId, adLink, adTitle, adDescription, adPrice, adLocation, adImage, adRegDate, sellerPhoneNumber, sellerName, sellerLink, sellerTotalAds, sellerRegDate, sellerType, phoneNumbers);

            System.Threading.Thread.Sleep(2000);

            return true;
        }


        static async void SendLogToTg(ITelegramBotClient botClient, long userId, string adLink, string adTitle, string adDescription, string adPrice, string adLocation, string adImage, DateTime adRegDate, string sellerPhoneNumber, string sellerName, string sellerLink, int sellerTotalAds, DateTime sellerRegDate, string sellerType, List<string> phoneNumbers)
        {
            phoneNumbers.Add(sellerPhoneNumber);

            adDescription = adDescription.Replace('<', '`').Replace('>', '`').Replace('"', '\"');
            adTitle = adTitle.Replace('<', '`').Replace('>', '`').Replace('"', '\"');

            string whatsappText = LinkGenerator.GenerateWhatsAppText(DB.GetWhatsappText(userId), adLink, adTitle, adPrice, adLocation, sellerName);

            string adInfo = $"<b>📦 Название: </b><code>{adTitle}</code>\n<b>📞 Номер: </b><code>{sellerPhoneNumber}</code>\n<b>💲 Цена: </b>{adPrice}\n<b>🧔🏻 Продавец: </b><a href=\"{sellerLink}\">{sellerName}</a>\n\n<b>📅 Добавлено: </b><b>{adRegDate.ToString().Split(' ')[0]}</b> <code>{adRegDate.ToString().Split(' ')[1]}</code>\n<b>📝 Количество объявлений: </b><b>{sellerTotalAds}</b>\n<b>📆 Дата регистрации: </b><b>{sellerRegDate.ToString("dd.MM.yyyy")}</b>\n\n<b>📷 Фото: </b><code>{adImage}</code>\n\n<b>🖨 Описание: </b>{adDescription}\n\n<a href=\"{adLink}\">Переход на объявление</a>\n<a href=\"https://api.whatsapp.com/send?phone={sellerPhoneNumber}&text={whatsappText}\">Написать WhatsApp</a>";

            try
            {
                try
                {
                    await botClient.SendPhotoAsync(
                        chatId: userId,
                        photo: adImage,
                        caption: adInfo,
                        parseMode: ParseMode.Html
                    );
                }
                catch
                {
                    await botClient.SendPhotoAsync(
                        chatId: userId,
                        photo: errorImageUri,
                        caption: adInfo,
                        parseMode: ParseMode.Html
                    );
                }
            }
            catch
            {
                await botClient.SendTextMessageAsync(
                    chatId: userId,
                    text: $"<b>📦 Название: </b><code>{adTitle}</code>\n<b>📞 Номер: </b><code>{sellerPhoneNumber}</code>\n<b>💲 Цена: </b>{adPrice}\n<b>🧔🏻 Продавец: </b><a href=\"{sellerLink}\">{sellerName}</a>\n\n<b>📅 Добавлено: </b><b>{adRegDate.ToString().Split(' ')[0]}</b> <code>{adRegDate.ToString().Split(' ')[1]}</code>\n<b>📝 Количество объявлений: </b><b>{sellerTotalAds}</b>\n<b>📆 Дата регистрации: </b><b>{sellerRegDate.ToString("dd.MM.yyyy")}</b>\n\n<b>📷 Фото: </b>{adImage}\n\n<a href=\"{adLink}\">Переход на объявление</a>\n<a href=\"https://api.whatsapp.com/send?phone={sellerPhoneNumber}&text={whatsappText}\">Написать WhatsApp</a>",
                    parseMode: ParseMode.Html
                );
            }

            if(phoneNumbers.Count == 5)
            {
                string path = "phones.vcf";
            
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach(string phone in phoneNumbers)
                    {
                        await writer.WriteLineAsync("BEGIN:VCARD");
                        await writer.WriteLineAsync("VERSION:2.1");
                        await writer.WriteLineAsync($"TEL;CELL:{phone}");
                        await writer.WriteLineAsync("END:VCARD");
                    }
                }

                using (Stream stream = System.IO.File.OpenRead(path))
                {
                    await botClient.SendDocumentAsync(
                        chatId: userId,
                        document: new InputOnlineFile(content: stream, fileName: path)
                    );
                }

                phoneNumbers.Clear();
            }
            return;
        }
       


        static async Task<String> GetPhoneNumber(string adId, string domen, string telCode)
        {
            try
            {
                string link = $"{domen}/api/listing/{adId}/contactInfo/";
                    
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                HttpResponseMessage response = await client.GetAsync(link);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(json);
                string phoneNumber = Functions.ConvertPhone(jObject["mobile"]!.ToString());

                if(String.IsNullOrEmpty(phoneNumber))
                {
                    return "null";
                }

                if(!phoneNumber.Contains("+"))
                {
                    if(phoneNumber.Substring(0, 3) == telCode)
                    {
                        phoneNumber = $"+{phoneNumber}";
                    }
                    else
                    {
                        phoneNumber = $"+{telCode}{phoneNumber}";
                    }
                }

                return phoneNumber;
            }
            catch
            {
                return "null";
            }
        }

        static string GenerateLink(int page, string parserCategory, string domen, string userLink)
        {
            string newLink = "";

            if(parserCategory == "all-categories")
            {
                newLink = $"{domen}/en/ads/?page={page}";
            }
            else
            {
                if(userLink!.Contains(domen!))
                {
                    if(userLink[^1] == '/')
                    {
                        newLink = userLink + "?page=" + page.ToString();
                    }
                    else
                    {
                        newLink = userLink + "/?page=" + page.ToString();
                    }
                }
                else
                {
                    newLink = $"https://www.olx.qa/ads/q-{userLink}/?page={page}";
                }
            } 

            return newLink;           
        }
    }
}
