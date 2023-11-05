using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace GMA_Extractor
{
    internal static class Program
    {
        private const string CommandsList = "Список актуальных команд:\n\r7z? - подробная информация о файлах 7z.exe и 7z.dll [7-Zip by Igor Pavlov]\n\rextractGma <Path> [<ExtractPath>] - распаковка .gma аддона или карты [НЕ ПОДДЕРЖИВАЮТСЯ ПРОБЕЛЫ В ПУТЯХ]\n\rextractGmaBin <Path> [<ExtractPath>] - распаковка .gma аддона или карты который(ая) сжат(а) в .bin [LZMA] файл [НЕ ПОДДЕРЖИВАЮТСЯ ПРОБЕЛЫ В ПУТЯХ]\n\rА также доступны команды в виде путей к файлам [НЕ ПОДДЕРЖИВАЮТСЯ ПРОБЕЛЫ]";
        private static void Main(string[] args)
        {
            Console.Title = ".gma & .gma.bin Extractor";
            Error("Данная программа является устаревшей, для получения полного функционала требуется перейти на версию 2.*\r\nСкачать новую версию можно здесь: ");
            Info(".gma & .gma.bin Extractor:\n\rВерсия: 1, редакция 3 [1.3]\n\rПоддерживаемые платформы: Windows 7+\n\rО последних обновлениях:\n\r<Малые изменения>\n\rИспользуется 7z.exe и 7z.dll, подробнее в команде '7z?'\n\r\n\rАвтор программы: Spearton");
            Error(CommandsList);
            if (args.Length > 0)
                if (File.Exists(args[0]))
                {
                    string Dir = Path.GetDirectoryName(args[0]);
                    try
                    {
                        switch (Extract(args[0], Dir))
                        {
                            case 1:
                                Error($"Не удалось распаковать '{Path.GetFileName(args[0])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', заголовок файла не соответствует формату GMA [файл не является GMA аддоном]");
                                break;
                            case 2:
                                Error($"Не удалось распаковать '{Path.GetFileName(args[0])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', полезные блоки данных не найдены [файл пуст]");
                                break;
                            default:
                                Completed($"'{Path.GetFileName(args[0])}' успешно распакован в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}'");
                                break;
                        }
                        Commands();
                    }
                    catch (EndOfStreamException)
                    {
                        Error($"Не удалось распаковать '{Path.GetFileName(args[0])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', файл пуст!");
                        Commands();
                    }
                    catch (Exception ex)
                    {
                        Error($"Не удалось распаковать '{Path.GetFileName(args[0])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', возможно этот файл не является GMA аддоном!\n\rПроизошла следующая ошибка:\r\n{ex}");
                        Commands();
                    }
                }
                else
                {
                    string Command = null;
                    foreach (string arg in args)
                        if (Command is null)
                            Command = arg;
                        else
                            Command += $" {arg}";
                    Commands(Command);
                }
            else
                Commands();
        }
        private static void Commands(string Command = null)
        {
            if (Command is null)
            {
                Info("Для того чтобы использовать команды нажмите enter");
                Console.ReadLine();
                goto WaitingACommand;
            }
            else
            {
                EnterCommand(Command);
                goto WaitingACommand;
            }
        WaitingACommand:
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Введите команду: ");
            Console.ForegroundColor = ConsoleColor.White;
            EnterCommand(Console.ReadLine());
            goto WaitingACommand;
        }
        private static void EnterCommand(string command)
        {
            if (command is null)
            {
                Error(CommandsList);
                return;
            }
            string[] Command = command.Split(' ');
            switch (Command[0].ToLower())
            {
                default:
                    if (File.Exists(Command[0]))
                    {
                        string extension = Path.GetExtension(Command[0]).ToLower();
                        if (extension == ".gma")
                        {
                            Completed("Найдено расширение .gma, оставьте поле пустым (нажмите пробел), чтобы распаковать как gma");
                        WaitingAnswerGMA:
                            Console.Write("Выберите, каким способом попробовать распаковать файл [gma or bin]: ");
                            string AnswerGMA = Console.ReadLine().ToLower();
                            if (AnswerGMA == "gma" || AnswerGMA == "" || AnswerGMA == " ")
                                EnterCommand($"extractgma {Command[0]}");
                            else if (AnswerGMA == "bin")
                                EnterCommand($"extractgmabin {Command[0]}");
                            else
                            {
                                Error("Некорректный выбор, доступны только режим BIN и GMA, также вы можете просто пропустить выбор, тогда будет распаковка в режиме GMA!");
                                goto WaitingAnswerGMA;
                            }
                        }
                        else if (extension == ".bin")
                        {
                            Completed("Найдено расширение .bin, оставьте поле пустым (нажмите пробел) чтобы распаковать как BIN");
                        WaitingAnswerBIN:
                            Console.Write("Выберите, каким способом попробовать распаковать файл [gma or bin]: ");
                            string AnswerBIN = Console.ReadLine().ToLower();
                            if (AnswerBIN == "gma")
                                EnterCommand($"extractgma {Command[0]}");
                            else if (AnswerBIN == "bin" || AnswerBIN == "" || AnswerBIN == " ")
                                EnterCommand($"extractgmabin {Command[0]}");
                            else
                            {
                                Error("Некорректный выбор, доступны только режим BIN и GMA, также вы можете просто пропустить выбор, тогда будет распаковка в режиме BIN!");
                                goto WaitingAnswerBIN;
                            }
                        }
                        else
                        {
                        WaitingAnswer:
                            Console.Write("Выберите, каким способом попробовать распаковать файл [gma or bin]: ");
                            string Answer = Console.ReadLine().ToLower();
                            if (Answer == "gma")
                                EnterCommand($"extractgma {Command[0]}");
                            else if (Answer == "bin")
                                EnterCommand($"extractgmabin {Command[0]}");
                            else
                            {
                                Error("Некорректный выбор, доступны только режим BIN и GMA!");
                                goto WaitingAnswer;
                            }
                        }
                    }
                    else
                        Error(CommandsList);
                    break;
                case "extractgma":
                    if (Command.Length > 1)
                    {
                        string Dir;
                        if (Command.Length > 2)
                        {
                            if (!Directory.Exists(Command[2]))
                            {
                                Directory.CreateDirectory(Command[2]);
                                Info($"Создание '{Command[2]}' [Путь распаковки]");
                            }
                            Dir = Command[2];
                        }
                        else
                            Dir = Path.GetDirectoryName(Command[1]);
                        Info($"Назначение '{Dir}' в качестве пути распаковки");
                        try
                        {
                            switch (Extract(Command[1], Dir))
                            {
                                case 1:
                                    Error($"Не удалось распаковать '{Path.GetFileName(Command[1])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', заголовок файла не соответствует формату GMA [файл не является GMA аддоном]");
                                    break;
                                case 2:
                                    Error($"Не удалось распаковать '{Path.GetFileName(Command[1])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', полезные блоки данных не найдены [файл пуст]");
                                    break;
                                default:
                                    Completed($"'{Path.GetFileName(Command[1])}' успешно распакован в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}'");
                                    break;
                            }
                            Commands();
                        }
                        catch (EndOfStreamException)
                        {
                            Error($"Не удалось распаковать '{Path.GetFileName(Command[1])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', файл пуст!");
                            Commands();
                        }
                        catch (Exception ex)
                        {
                            Error($"Не удалось распаковать '{Path.GetFileName(Command[1])}' в '{Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}', возможно этот файл не является GMA аддоном!\n\rПроизошла следующая ошибка:\r\n{ex}");
                            Commands();
                        }
                    }
                    else
                        Error(@"Пропущен обязательный параметр 'Path' [Путь к файлу], примеры использования данной команды: extractGma D:\extractedAddon.gma; extractGma D:\extractedAddon.gma D:\addon; [НЕ ПОДДЕРЖИВАЮТСЯ ПРОБЕЛЫ В ПУТЯХ!]");
                    break;
                case "extractgmabin":
                    if (Command.Length > 1)
                    {
                        Info($"Назначение {Path.Combine(Path.GetDirectoryName(Command[1]), "GMA_Extractor_Temp")} в качестве TEMP");
                        string Dir = Path.Combine(Path.GetDirectoryName(Command[1]), "GMA_Extractor_Temp");
                        if (!Directory.Exists(Dir))
                        {
                            Info($"Создание {Dir} [TEMP]");
                            Directory.CreateDirectory(Dir);
                        }
                        if (Command.Length > 2)
                        {
                            if (!Directory.Exists(Command[2]))
                            {
                                Info($"Создание {Command[2]} [Путь распаковки]");
                                Directory.CreateDirectory(Command[2]);
                            }
                            Info($"Назначение {Path.Combine(Command[2], "GMA_Extractor_Temp")} в качестве TEMP");
                            Dir = Path.Combine(Command[2], "GMA_Extractor_Temp");
                            if (!Directory.Exists(Dir))
                            {
                                Info($"Создание {Dir} [TEMP]");
                                Directory.CreateDirectory(Dir);
                            }
                        }
                        try
                        {
                            string ExePath = Path.Combine(Dir, "7z.exe");
                            string DllPath = Path.Combine(Dir, "7z.dll");
                            Info($"Распаковка {ExePath} [TEMP]");
                            File.WriteAllBytes(ExePath, Properties.Resources.EXE);
                            Info($"Распаковка {DllPath} [TEMP]");
                            File.WriteAllBytes(DllPath, Properties.Resources.DLL);
                            Process process = new Process();
                            process.StartInfo.FileName = ExePath;
                            process.StartInfo.Arguments = $@"x ""{Command[1]}"" -o""{Dir}""";
                            process.Start();
                            Info($"Запущен процесс 7z.exe [{process.Id}]... Попытка распаковки {Command[1]} в {Dir}");
                            process.WaitForExit();
                            process.Dispose();
                            string FilePath = null;
                            if (File.Exists(Path.Combine(Dir, Path.GetFileNameWithoutExtension(Command[1]))))
                                FilePath = Path.Combine(Dir, Path.GetFileNameWithoutExtension(Command[1]));
                            else if (File.Exists(Path.Combine(Dir, $"{Command[1]}~")))
                                FilePath = Path.Combine(Dir, $"{Command[1]}~");
                            else
                            {
                                Error("Странно, но не удалось найти файл который можно распаковать!");
                                Info($"Удаление {ExePath} [TEMP]");
                                File.Delete(ExePath);
                                Info($"Удаление {DllPath} [TEMP]");
                                File.Delete(DllPath);
                                Info($"Удаление {Dir} [TEMP]");
                                Directory.Delete(Dir, true);
                                return;
                            }
                            Info($"Удаление {ExePath} [TEMP]");
                            File.Delete(ExePath);
                            Info($"Удаление {DllPath} [TEMP]");
                            File.Delete(DllPath);
                            Info($"Попытка распаковки {FilePath} в {Path.GetDirectoryName(Dir)}");
                            if (Extract(FilePath, Path.GetDirectoryName(Dir)) == 0)
                            {
                                Completed($"{Path.GetFileName(Command[1])} успешно распакован в {Path.GetDirectoryName(Dir)}\\{Path.GetFileName(Dir)}!");
                                Info($"Удаление {Dir} [TEMP]");
                                Directory.Delete(Dir, true);
                            }
                            else
                            {
                                Error("Возможно произошла ошибка при распаковке .GMA файла [Возможна ложная тревога]");
                                Info($"Удаление {Dir} [TEMP]");
                                Directory.Delete(Dir, true);
                            }
                        }
                        catch
                        {
                            Error($"Не удалось распаковать {Command[1]} в {Dir}, возможно этот файл не является сжатым GMA аддоном или картой!");
                        }
                    }
                    else
                        Error(@"Пропущен обязательный параметр 'Path' [Путь к файлу .bin], пример использования данной команды: extractGmaBin D:\CompressedAddon.bin; [НЕ ПОДДЕРЖИВАЮТСЯ ПРОБЕЛЫ В ПУТЯХ!]");
                    break;
                case "7z?":
                    Completed("7-Zip - это архиватор разработанный российским программистом Игорем Викторовичем Павловым, официальный сайт: https://www.7-zip.org/, WIKI: https://ru.wikipedia.org/wiki/7-Zip");
                    break;
            }
        }
        private static void WriteLine(ConsoleColor Color, string Text)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void Error(string Text) => WriteLine(ConsoleColor.Red, Text);
        private static void Info(string Text) => WriteLine(ConsoleColor.Blue, Text);
        private static void Completed(string Text) => WriteLine(ConsoleColor.Green, Text);
        private static int Extract(string gmaFile, string outputDir)
        {
            string ShortName = Path.GetFileName(gmaFile);
            Info($"Чтение файла {ShortName}");
            FileStream fileStream = new FileStream(gmaFile, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream, Encoding.GetEncoding(1252));
            uint num = binaryReader.ReadUInt32();
            int result;
            if (num != 1145130311u)
            {
                binaryReader.Close();
                fileStream.Close();
                result = 1;
            }
            else
            {
                fileStream.Seek(18L, SeekOrigin.Current);
                Info($"Чтение названия аддона в файле {ShortName}");
                string text = ReadString(binaryReader);
                Info($"Чтение описания аддона в файле {ShortName}");
                string text2 = ReadString(binaryReader);
                Info($"Чтение автора аддона в файле {ShortName}");
                string text3 = ReadString(binaryReader);
                fileStream.Seek(4L, SeekOrigin.Current);
                List<object[]> list = new List<object[]>();
                for (;;)
                {
                    uint num2 = binaryReader.ReadUInt32();
                    if (num2 == 0u)
                        break;
                    list.Add(new object[] { ReadString(binaryReader), binaryReader.ReadUInt32() } );
                    fileStream.Seek(8L, SeekOrigin.Current);
                }
                if (list.Count == 0)
                {
                    binaryReader.Close();
                    fileStream.Close();
                    result = 2;
                }
                else
                {
                    Info("Создание папки распаковки");
                    string text4 = Path.Combine(outputDir, ScrubFileName(text));
                    if (!Directory.Exists(text4))
                        Directory.CreateDirectory(text4);
                    Info($"Чтение и запись данных аддона из файла {ShortName}");
                    foreach (object[] array in list)
                    {
                        byte[] bytes = binaryReader.ReadBytes(Convert.ToInt32(array[1]));
                        string text5 = Path.Combine(text4, Path.GetDirectoryName((string)array[0]));
                        string fileName = Path.GetFileName((string)array[0]);
                        if (!Directory.Exists(text5))
                            Directory.CreateDirectory(text5);
                        File.WriteAllBytes(Path.Combine(text5, fileName), bytes);
                    }
                    string path = Path.Combine(text4, "addon.json");
                    Info($"Запись информации об аддоне из файла {ShortName}");
                    File.WriteAllText(path, string.Concat(new string[] { "\"AddonInfo\"\r\n{\r\n\t\"name\" \"", text, "\"\r\n\t\"author_name\" \"", text3, "\"\r\n\t\"info\" \"", text2, "\"\r\n}" } ));
                    Info($"Освобождение ресурсов после работы");
                    binaryReader.Close();
                    fileStream.Close();
                    result = 0;
                }
            }
            return result;
        }
        private static string ReadString(BinaryReader reader)
        {
            string text = string.Empty;
            for (;;)
            {
                char c = reader.ReadChar();
                if (c == '\0')
                    break;
                text += c;
            }
            return text;
        }
        private static string ScrubFileName(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value);
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidFileNameChars)
                stringBuilder.Replace(c.ToString(), "");
            return stringBuilder.ToString();
        }
    }
}