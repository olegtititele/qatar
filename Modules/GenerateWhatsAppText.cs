namespace Modules
{
    public static class LinkGenerator
    {
        public static string GenerateWhatsAppText(string whatsapp_text, string adlink, string adname, string adprice, string adlocation, string sellername)
        {
            while(true)
            {
                if(whatsapp_text.Contains("@adlink"))
                {
                    whatsapp_text = whatsapp_text.Replace("@adlink", adlink);
                }
                else if(whatsapp_text.Contains("@adname"))
                {
                    if(adname=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@adname", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@adname", adname);
                    }
                }
                else if(whatsapp_text.Contains("@adprice"))
                {
                    if(adprice=="Не указана")
                    {
                        whatsapp_text = whatsapp_text.Replace("@adprice", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@adprice", adprice);
                    }
                }
                else if(whatsapp_text.Contains("@adlocation"))
                {
                    if(adlocation=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@adlocation", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@adlocation", adlocation);
                    }
                }
                else if(whatsapp_text.Contains("@sellername"))
                {
                    if(sellername=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@sellername", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@sellername", sellername);
                    }
                }
                else{ return whatsapp_text; }
            }
        }
    }
}