using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace CoreUpdater
{
    class Program
    {
        private static string ActualVer { get; set; }
        
        static void Main(string[] args)
        {
            Console.WriteLine("============================================");
            Console.WriteLine("");
            Console.WriteLine("╦═╗┌─┐┌─┐┬┬─┐┌─┐┌┬┐┌─┐┌┐┌┌┬┐╔═╗┌─┐┬─┐┌─┐");
            Console.WriteLine("╠╦╝├┤ │ ┬│├┬┘└─┐ │ ├─┤│││ │ ║  │ │├┬┘├┤ ");
            Console.WriteLine("╩╚═└─┘└─┘┴┴└─└─┘ ┴ ┴ ┴┘└┘ ┴ ╚═╝└─┘┴└─└─┘");
            Console.WriteLine("");
            Console.WriteLine("============================================");
            Logger.LogWithTime("Registrant Core Updater");
            Logger.LogWithTime("v.0.1 by Alexey Kulagin (TheCrazyWolf)");
            Logger.LogWithTime("Инициализирован старт обновления");
            Console.WriteLine("");
            Console.WriteLine("");
            CheckVersion();
            DownloadPackage();
            Unpack();
#if !DEBUG
            Cleanup();
#endif
            Logger.LogWithTime("Обновление завершено");
            Process.Start("Registrant.exe");
            Console.ReadKey();
        }

        public static void CheckVersion()
        {
            Logger.LogWithTime("Получение списка актуальных версии");
            try
            {
                WebClient web = new WebClient();
                ActualVer = web.DownloadString("https://raw.githubusercontent.com/TheCrazyWolf/RegistrantCore/master/Registrant/ActualVer.txt");
                Logger.LogWithTime($"Последняя версия: {ActualVer}");
            }
            catch (Exception)
            {
                Logger.LogWithTime("ОШИБКА ОБНОВЛЕНИЯ");
                Logger.LogWithTime("Не удалось получить список актуальных версии программного обеспечения.");
                Console.ReadKey();
                Console.ReadKey();
            }
        }

        public static void DownloadPackage()
        {
            Logger.LogWithTime($"Загрузка выбранной версии {ActualVer}");
            try
            {
                string url = "https://github.com/TheCrazyWolf/RegistrantCore/releases/download/" + ActualVer + "/package.zip";
                
                Logger.LogWithTime($"Загрузка началась ({url})");
                WebClient web = new WebClient();
                web.DownloadFile(@url, @"package.zip");

                string package = @"package.zip";
                if (File.Exists(package))
                {
                    Logger.LogWithTime("Пакет загружен");
                }
                else
                {
                    Logger.LogWithTime("Что то пошло не так");
                    Console.ReadKey();
                }

            }
            catch (Exception)
            {
                Logger.LogWithTime("ОШИБКА ОБНОВЛЕНИЯ");
                Logger.LogWithTime("Не удалось получить список актуальных версии программного обеспечения.");
                Console.ReadKey();
                Console.ReadKey();
            }
        }

        public static void Unpack()
        {
            Logger.EmptyString();
            Logger.LogWithTime("ВНИМАНИЕ!");
            Logger.LogWithTime("Требуется подключение к интернету, во время обновления не пытайтесь");
            Logger.LogWithTime("закрыть это окно, это может привести к сбою обновления и может откразится на");
            Logger.LogWithTime("работе программы");
            Logger.EmptyString();
            Logger.LogWithTime("Убедитесь, что RegistrantCore сейчас закрыт");
            Logger.LogWithTime("Ожидание");
            Thread.Sleep(3000);
            try
            {
                Logger.LogWithTime("Распаковка пакета и применение обновления");
                ZipFile.OpenRead("./package.zip").ExtractToDirectory("./", true);
                Logger.LogWithTime("Развертывание завершено");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        public static void Cleanup()
        {
            Logger.LogWithTime("Очистка кеша");
            File.Delete(@"package.zip");
            Logger.LogWithTime("Очистка завершена");
        }
    }
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }

    }
}
