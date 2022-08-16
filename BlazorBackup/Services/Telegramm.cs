using System;
using Telegram.Bot;

namespace BlazorBackup.Services
{
    //https://cms3.ru/kak-poluchit-chat-id-telegram/
    public class Telegramm
    {
        private static string token = "2019720083:AAH7nRLP0Ws3AVUjSr_fDBCUtlSLV6v4rZQ";
       // private static readonly TelegramBotClient Bot = new TelegramBotClient(token);
        private static string telegramChatId = "-789973058";
        Logger logger = new Logger();
        public async void telegramAsync(string telegramSendMessange)
        {

            try
            {
                TelegramBotClient botClient = new TelegramBotClient(token);
                var me = botClient.GetMeAsync().Result;
                Console.WriteLine(me.Username);


                var chatId = telegramChatId;
                var t = await botClient.SendTextMessageAsync(chatId, telegramSendMessange);
            }
            catch (Exception ex)
            {
                logger.LogWrite($"Metod: telegramAsync >> {ex.Message}", Logger.typeLog.Error);
            }
          
        }

    }
}
