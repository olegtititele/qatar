using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

using Modules;
using PostgreSQL;

namespace Parser
{
    public class QatarLiving
    {
        private static string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";
        private static string errorImageUri = "https://upload.wikimedia.org/wikipedia/commons/9/9a/%D0%9D%D0%B5%D1%82_%D1%84%D0%BE%D1%82%D0%BE.png";
        public static void StartParsing(ITelegramBotClient botClient, long userId, DateTime userExactTime)
        {
            var options = new FirefoxOptions();
            // options.AddArgument("--no-sandbox");
            // options.AddArgument("--disable-dev-shm-usage");
            // options.AddArgument($"--user-agent={userAgent}");
            // options.AddArgument("--disable-plugins-discovery");
            // options.AddArguments("--headless");
            options.SetPreference("permissions.default.image", 2);
            options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
            // options.AddArguments("--disable-blink-features=AutomationControlled");
            IWebDriver driver = new FirefoxDriver(options);


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

            try
            {
                List<string> phoneNumbers = new List<string>();
                List<string> passedLinks = new List<string>();
  
                while(true)
                {
                    if(DB.GetParser(userId) == "Start")
                    {
                        try
                        {
                            int page = 1;
                            ParseCategory(driver, botClient, userId, page, passedLinks, phoneNumbers, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories);
                        }
                        catch{ }
                    }
                    else
                    {
                        driver.Close();
                        return;
                    }
                    

                    System.Threading.Thread.Sleep(timeout);
                }
            }
            catch
            {
                DB.UpdateParser(userId, "Stop");
                driver.Close();
                return;
            }
        }

        private static void ParseCategory(IWebDriver driver, ITelegramBotClient botClient, long userId, int page, List<string> passedLinks, List<string> phoneNumbers, DateTime exactTime, string userPlatform, string userLink, string userSellerTotalAds, string userSellerRegDate, decimal userSellerRating, string userSellerType, string blacklist, string parserCategory, List<string> blacklistCategories)
        {
            string adLink = "";
            List<string> advertisementsLinks = new List<string>();
            string categoryLink = GenerateLink(page, parserCategory, userLink);
            
            driver.Navigate().GoToUrl(categoryLink);

            var advertisements = driver.FindElements(By.XPath("//div[@class=\"vehicle-row\"]"));
            
            if(advertisements != null)
            {
                
                foreach(var advertisement in advertisements)
                {
                    try
                    {
                        var promoted = driver.FindElement(By.XPath(".//span[@class=\"singleVehicle-tag-name singleVehicle-tag-name-list promoted-color-tag\"]")).Text;
                        continue;
                    }
                    catch
                    {
                        adLink = advertisement.FindElement(By.XPath(".//a[@class=\"vehicle-row-data\"]")).GetAttribute("href");
                        advertisementsLinks.Add(adLink);
                    }
                }
            }


            if(advertisementsLinks.Count > 0)
            {
                foreach (var link in advertisementsLinks)
                {   
                    if(DB.GetParser(userId) == "Start")
                    { 
                        if(passedLinks.Contains(link))
                        {
                            return;
                        }
                        else
                        {
                            passedLinks.Add(link);
                            if(!DB.CheckAdvestisement(userId, link))
                            {
                                if(!ParsePageInfo(driver, botClient, userId, link, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories, phoneNumbers))
                                {
                                    return;
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
            }


            page++;
            ParseCategory(driver, botClient, userId, page, passedLinks, phoneNumbers, exactTime, userPlatform, userLink, userSellerTotalAds, userSellerRegDate, userSellerRating, userSellerType, blacklist, parserCategory, blacklistCategories);
        }

        static bool ParsePageInfo(IWebDriver driver, ITelegramBotClient botClient, long userId, string adLink, DateTime exactTime, string userPlatform, string userLink, string userSellerTotalAds, string userSellerRegDate, decimal userSellerRating, string userSellerType, string blacklist, string parserCategory, List<string> blacklistCategories, List<string> phoneNumbers)
        {
            string adCategory = "";
            string adPrice = "";
            string adTitle = "";
            string adImage = "";
            string sellerType = "Частное лицо";
            string sellerLink = "";
            string sellerName = "";
            string adDescription = "";
            string adLocation = "";
            int sellerTotalAds = 1;
            string sellerPhoneNumber = "";
            DateTime adRegDate = DateTime.Today;
            DateTime sellerRegDate = DateTime.Today;

            driver.Navigate().GoToUrl(adLink);
            var categories = driver.FindElements(By.XPath("//p[@class=\"category\"]//a"));
            
            foreach(var category in categories)
            {
                adCategory = category.GetAttribute("href");

                if(blacklistCategories.Contains(adCategory))
                {
                    return true;
                }
            }
            
            string adDataJson = driver.FindElement(By.XPath("//script[@id=\"__NEXT_DATA__\"]")).GetAttribute("innerHTML");
            JObject jObject = JObject.Parse(adDataJson);
            var _source = jObject["props"]!["pageProps"]!["classifiedContent"]!["classified"]![0]!["_source"]!;


            // Ad reg gate
            long unixDate = Int64.Parse(_source["refresh"]!.ToString());
            adRegDate = Functions.UnixTimeToDateTime(unixDate);
            
            if(Functions.CheckAdRegDate(exactTime, adRegDate)){  }else{ return false; }

            
            // Phone Number
            try
            {
                sellerPhoneNumber = _source["phone"]!.ToString();
            }
            catch
            {
                return true;
            }
            
            if(Functions.CheckBlacklistAds(userId, sellerPhoneNumber, blacklist)){ }else{ return true; }
            

            // Ad title
            try
            {
                adTitle = _source["title"]!.ToString();
            }
            catch
            {
                adTitle = "Не указано";
            }


            // Ad price
            try
            {
                adPrice = Functions.ConvertPrice(_source["price"]!.ToString(), "QAR");
            }
            catch
            {
                adPrice = "Не указана";
            }
        

            // Ad description
            try
            {
                adDescription = _source["desc"]!.ToString();
            }
            catch
            {
                adDescription = "Не указано";
            }


            // Ad image
            try
            {
                adImage = "https://files.qatarliving.com/styles/listing_ratio_3_2/s3/" + _source["image"]![0]!.ToString();
            }
            catch
            {
                adImage = errorImageUri;
            }
            

            // // Ad location
            // try
            // {
            //     adLocation = _source["location"]!["name"]!.ToString();
            // }
            // catch
            // {
            //     adLocation = "Не указана";
            // }


            
            // Seller link
            try
            {
                sellerLink = "https://www.qatarliving.com/" + _source["author"]!["slug"]!.ToString();
                driver.Navigate().GoToUrl(sellerLink);

                var scripts = driver.FindElements(By.XPath("//script"));
                
                foreach(var script in scripts)
                {
                    string sellerDataJson = script.GetAttribute("innerHTML");

                    if(sellerDataJson.Contains("dataLayer = ["))
                    {
                        sellerDataJson = sellerDataJson.Split("dataLayer = [")[1];
                        char[] MyChar = {';',')', ']'};
                        sellerDataJson = sellerDataJson.TrimEnd(MyChar);
                        jObject = JObject.Parse(sellerDataJson);
                        break;
                    }
                    
                }

                
                // Seller reg
                try
                {
                    unixDate = Int64.Parse(jObject["entityCreated"]!.ToString()) * 1000;
                    Console.WriteLine(unixDate);
                    sellerRegDate = Functions.UnixTimeToDateTime(unixDate);
                }
                catch{ }
                
                try
                {
                    sellerTotalAds = Int32.Parse(driver.FindElement(By.XPath("//a[@class=\"b-profile-head--el-statistic-link\"]//b")).Text);
                }
                catch{ }
                
            }
            catch
            {
                sellerLink = "Не указана";
                sellerRegDate = DateTime.Now;
                sellerTotalAds = 1;
            }
            
            
            if(Functions.CheckSellerRegDate(userSellerRegDate, sellerRegDate)){ }else{ return true; }
            if(Functions.CheckSellerTotalAds(userSellerTotalAds, sellerTotalAds)){  }else{ return true; }

            // Seller Name
            try
            {
                sellerName = _source["author"]!["username"]!.ToString();
                if(String.IsNullOrEmpty(sellerName))
                {
                    sellerName = "Не указано";
                }
            }
            catch
            {
                sellerName = "Не указано";
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
       

        static string GenerateLink(int page, string parserCategory, string userLink)
        {
            string newLink = "";

            if(parserCategory == "all-categories")
            {
                newLink = $"https://www.qatarliving.com/classifieds?search_input=&page={page}";
            }
            else
            {
                // if(userLink!.Contains(domen!))
                // {
                //     if(userLink[^1] == '/')
                //     {
                //         newLink = userLink + "?page=" + page.ToString();
                //     }
                //     else
                //     {
                //         newLink = userLink + "/?page=" + page.ToString();
                //     }
                // }
                // else
                // {
                //     newLink = $"https://www.olx.qa/ads/q-{userLink}/?page={page}";
                // }
            } 

            return newLink;           
        }
    }
}
