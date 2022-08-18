using System;
using Telegram.Bot;

namespace BlazorBackup.Services
{
    //https://cms3.ru/kak-poluchit-chat-id-telegram/
    public class Telegramm
    {
        //private static string token;// = "2019720083:AAH7nRLP0Ws3AVUjSr_fDBCUtlSLV6v4rZQ";
       // private static readonly TelegramBotClient Bot = new TelegramBotClient(token);
        ///private static string telegramChatId; //= "-789973058";
        private Settings settings;
        public Telegramm()
        {
            JsonServiceSetting serviceSetting = new JsonServiceSetting();
            settings = serviceSetting.ReadJsonConf();

        }

        Logger logger = new Logger();
        public async void telegramAsync(string telegramSendMessange)
        {

            try
            {
                TelegramBotClient botClient = new TelegramBotClient(settings.TelegramToken);
                var me = botClient.GetMeAsync().Result;
                Console.WriteLine(me.Username);

                 await botClient.SendTextMessageAsync(settings.TelegramChtid, telegramSendMessange);
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: telegramAsync >> {ex.Message}", Logger.typeLog.Error);
            }
          
        }

    }
}
