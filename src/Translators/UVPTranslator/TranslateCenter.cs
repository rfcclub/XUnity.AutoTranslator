﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using IniParser;
using IniParser.Model;
using XUnity.Common.Logging;

namespace UVPTranslator
{
    public static class TranslateCenter
    {
        private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");

        private static readonly Regex vnRegex = new Regex(@"[ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]+");
        private const string EMPTY = "";
        private const string TWO_SPACES = @"  ";
        private const string SPACE = @" ";

        public static Dictionary<string, string> ChinesePhienAmWords
        {
            get; set;
        } = new Dictionary<string, string>();

        public static Dictionary<string, string> Names
        {
            get;
            set;
        } = new Dictionary<string, string>();
        public static Dictionary<string, string> ThanhNgu
        {
            get;
            set;
        } = new Dictionary<string, string>();

        public static Dictionary<string, string> VietPhrase
        {
            get;
            set;
        } = new Dictionary<string, string>();

        public static Dictionary<string, string> LuatNhan
        {
            get;
            set;
        } = new Dictionary<string, string>();
        public static string VietPhrasePattern { get; set; } 
        public static string ChinesePhienAmPattern { get; set; }
        public static string NamesPattern { get; set; }
        public static bool UpperFirstLetter { get; private set; } = true;
        public static bool CleanTwoSpaces { get; private set; } = true;
        public static bool TrimSpaceBeforePunctuation { get; private set; } = true;
        public static bool TrimSpaceBeforeAfterBracket { get; private set; } = false;
        public static int NamesOrder { get; private set; } = 0;
        public static int VietPhraseOrder { get; private set; } = 1;

        public static int LuatNhanOrder { get; private set; } = 2;
        public static int ChinesePhienAmWordsOrder { get; private set; } = 3;

        public static bool WriteLog { get; private set; } = false;
        public static bool ConvertChinesePunctuation { get; private set; } = false;

        public static Dictionary<int, Dictionary<string, string>> translateMap = new Dictionary<int, Dictionary<string, string>>();
        public static bool CanWork { get; set; } = true;

        public static string AssemblyDirectory { get; set; }
        //{
        //    get
        //    {
        //        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        //UriBuilder uri = new UriBuilder(codeBase);
        //string path = Uri.UnescapeDataString(uri.Path);
        //        return Path.GetDirectoryName(path);
        //    }
        //}
        public static void Init()
        {
            ChinesePhienAmWords = new Dictionary<string, string>();
            Names = new Dictionary<string, string>();
            ThanhNgu = new Dictionary<string, string>();
            VietPhrase = new Dictionary<string, string>();
            LuatNhan = new Dictionary<string, string>();
            string dir = $"{AssemblyDirectory}\\Data";
            ReadInitFile($"{dir}\\TranslateTool.ini");
            if (CanWork)
            {
                ReadFileToList(ChinesePhienAmWords, $"{dir}\\ChinesePhienAmWords.txt");
                ReadFileToList(Names, $"{dir}\\Names.txt");
                ReadFileToList(ThanhNgu, $"{dir}\\ThanhNgu.txt");
                ReadFileToList(VietPhrase, $"{dir}\\VietPhrase.txt");
                ReadTextRegExToList(LuatNhan, $"{dir}\\LuatNhan.txt");
                translateMap[0] = Names;
                translateMap[1] = VietPhrase;
                translateMap[2] = LuatNhan;
                translateMap[3] = ChinesePhienAmWords;
            }
        }

        private static void ReadTextRegExToList(Dictionary<string, string> dictionary, string resourceName)
        {
            ReadTextRegExToList(dictionary, File.OpenRead(resourceName));
        }

        private static void ReadTextRegExToList(Dictionary<string, string> dictionary, Stream resourceName)
        {
            using (StreamReader stream = new StreamReader(resourceName, Encoding.UTF8))
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    string[] lines = line.Split('=');
                    if (lines.Length != 2)
                    {
                        continue;
                    }
                    string key = lines[0].Trim();
                    var value = lines[1];

                    string[] values = value.Replace("{0}", "¤_¤").Split('¤');
                    StringBuilder replacement = new StringBuilder();
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Equals("_")) replacement.Append($"$1");
                        else replacement.Append(values[i]);
                    }
                    string[] keys = key.Replace("{0}", "¤_¤").Split('¤');
                    StringBuilder pattern = new StringBuilder();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i].Equals("_")) pattern.Append("(.+)");
                        else pattern.Append($"({keys[i]})");
                    }
                    dictionary[pattern.ToString()] = replacement.ToString();
                }
            }
        }
        private static void ReadInitFile(string iniFile)
        {
            XuaLogger.AutoTranslator.Info($"Load ini from {iniFile}");
            if (!File.Exists(iniFile))
            {
                CanWork = false;
                return;
            }
            using (StreamReader stream = new StreamReader(File.OpenRead(iniFile), Encoding.UTF8))
            {
                var parser = new FileIniDataParser();
                IniData ini = parser.ReadData(stream);
                UpperFirstLetter = ini["Main"]["UpperFirstLetter"]?.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? true;
                CleanTwoSpaces = ini["Main"]["CleanTwoSpaces"]?.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? true;
                TrimSpaceBeforePunctuation = ini["Main"]["TrimSpaceBeforePunctuation"]?.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? true;
                TrimSpaceBeforeAfterBracket = ini["Main"]["TrimSpaceBeforeAfterBracket"]?.Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false;
                NamesOrder = ini["Translate"]["Names"]?.ToInteger() ?? 0;
                VietPhraseOrder = ini["Translate"]["VietPhrase"]?.ToInteger() ?? 1;
                LuatNhanOrder = ini["Translate"]["LuatNhan"]?.ToInteger() ?? 2;
                ChinesePhienAmWordsOrder = ini["Translate"]["ChinesePhienAmWords"]?.ToInteger() ?? 3;
                WriteLog = ini["Main"]["WriteLog"]?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? false;
                ConvertChinesePunctuation = ini["Main"]["ConvertChinesePunctuation"]?.Equals("true", StringComparison.InvariantCultureIgnoreCase) ?? true;
                if (IsOK(NamesOrder, VietPhraseOrder, ChinesePhienAmWordsOrder, LuatNhanOrder))
                {
                    translateMap[NamesOrder] = Names;
                    translateMap[VietPhraseOrder] = VietPhrase;
                    translateMap[LuatNhanOrder] = LuatNhan;
                    translateMap[ChinesePhienAmWordsOrder] = ChinesePhienAmWords;
                }
            }
        }

        private static bool IsOK(params int[] intToChecks)
        {
            foreach(int intToCheck in intToChecks)
            {
                if (intToCheck < 0 || intToCheck > 2) return false;
            }
            return true;
        }

        private static void ReadFileToList(Dictionary<string, string> dictionary, string resourceName)
        {
            XuaLogger.AutoTranslator.Info($"Read dict from {resourceName}");
            ReadFileToList(dictionary, File.OpenRead(resourceName));
        }

        private static void ReadFileToList(Dictionary<string, string> dictionary, Stream resourceName)
        {
            using (StreamReader reader = new StreamReader(resourceName, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] lines = line.Split('=');
                    if (lines.Length != 2)
                    {
                        continue;
                    }
                    string key = lines[0].Trim();
                    if (!dictionary.ContainsKey(key))
                    {
                        // LAY VIETPHRASE 1 NGHIA
                        string[] vp2Nghia = lines[1].Split('/');
                        if (vp2Nghia.Length < 2)
                        {
                            string value = lines[1].Trim();
                            // Kiem tra coi co dau :
                            string[] tuDongNghia = value.ToString().Split(':');
                            if (tuDongNghia.Length < 2 || value.Length == 1)
                            {
                                dictionary.Add(key, value);
                            }
                            else
                            {
                                dictionary.Add(key, tuDongNghia[0]);
                            }
                        }
                        else
                        {
                            // Kiem tra coi co dau :
                            string[] tuDongNghia = vp2Nghia[0].Split(':');
                            if (tuDongNghia.Length < 2)
                            {
                                dictionary.Add(key, vp2Nghia[0]);
                            }
                            else
                            {
                                dictionary.Add(key, tuDongNghia[0]);
                            }
                        }
                    }
                }
            }
        }
        private static void BuildPattern()
        {
            ChinesePhienAmPattern = "(" + String.Join("|", ChinesePhienAmWords.Keys.ToArray()) + ")";
            VietPhrasePattern = "(" + String.Join("|", VietPhrase.Keys.ToArray()) + ")";
            NamesPattern = "(" + String.Join("|", Names.Keys.ToArray()) + ")";
        }

        public static string Translate(string translateContent, string prefix = @" ", string postfix = @" ", bool trim = false, bool isName = false)
        {
            if (!CanWork) return translateContent;
            if (ConvertChinesePunctuation)
            {
                StringBuilder result = new StringBuilder(translateContent);
                result = result.Replace('\uff0c', ',')
                           .Replace('\uff01', '!')
                           .Replace('\uff08', '(')
                           .Replace('\uff09', ')')
                           .Replace('\uff1a', ':')
                           .Replace('\uff1b', ';')
                           .Replace('\uff1f', '?')
                           .Replace('\uff5e', '~')
                           .Replace('\u2026', '.')
                           .Replace('「', '[')
                           .Replace('」', ']')
                           //.Replace('\u201c', '"')
                           //.Replace('\u201d', '"')
                           .Replace('\u203b', '*')
                           .Replace('\u3000', ' ')
                           .Replace('\u3001', ',')
                           .Replace('\u3002', '.')
                           .Replace('\u300a', '(')
                           .Replace('\u300b', ')');
                translateContent = result.ToString();
            }
            Regex regex1 = new Regex("({[a-z0-9_%|+-x/#]+}|&[a-z0-9_%|+-x/#]+&)");
            var matches = regex1.Matches(translateContent);
            var matched = matches.Count > 0;
            Dictionary<string, string> bkup = new Dictionary<string, string>();
            if (matched)
            {
                int count = 1;
                foreach(Match match in matches)
                {
                    bkup[$"<<<GROUP_{count}>>>"] = match.Value;
                    count += 1;
                }
                
                // replace string content with bkup key
                StringBuilder builder = new StringBuilder(translateContent);
                foreach(var key in bkup.Keys)
                {
                    builder.Replace(bkup[key], key);
                }
                translateContent = builder.ToString();
            }
            StringBuilder translateBuilder = new StringBuilder(translateContent);
            var orderedKeys = translateMap.Keys.OrderBy(s => s);
            bool translated = false;
            foreach (var orderKey in orderedKeys)
            {
                var list = translateMap[orderKey].AsEnumerable().ToList();
                int count = 0;
                foreach (var t in list)
                {
                    if (orderKey != 2)
                    {
                        translateBuilder = translateBuilder.Replace(t.Key, prefix + t.Value + postfix);
                    }
                    else
                    {
                        string val = translateBuilder.ToString();
                        Match match = Regex.Match(val, t.Key);
                        if (match.Success)
                        {
                            val = Regex.Replace(val, t.Key, t.Value);
                        }
                    }
                    count++;
                    if (count % 10 == 0
                        && !IsChinese(StripChineseChars(translateBuilder.ToString())))
                    {
                        translated = true;
                        break;
                    }
                }
                if (translated) break;
            }
            translateContent = translateBuilder.ToString();
            // recover old string
            if (matched)
            {
                StringBuilder builder = new StringBuilder(translateContent);
                foreach (var key in bkup.Keys)
                {
                    builder.Replace(key, bkup[key]);
                }
                translateContent = builder.ToString();
            }
            
            ThanhNgu.AsEnumerable().ToList().ForEach(t =>
                translateContent = translateContent.Replace(t.Key, prefix + t.Value + postfix));
            if (CleanTwoSpaces)
            {
                translateContent = Regex.Replace(translateContent, @"( )+", " ");
            }
            // trim space before .,?!
            if (TrimSpaceBeforePunctuation)
            {
                translateContent = translateContent.Replace(" .", ".").Replace(" ,", ",").Replace(" !", "!").Replace(" ?", "?");
            }
            if (TrimSpaceBeforeAfterBracket)
            {
                translateContent = translateContent.Replace("( ", "(").Replace(" )", ")");
            }
            translateContent = trim ? translateContent.Trim() : translateContent;
            // uppercase first letter
            if (translateContent.Length > 1 && UpperFirstLetter)
            {
                translateContent = translateContent.FirstUpper();
            }
            return translateContent;
        }
        public static bool IsChinese(string c)
        {
            return cjkCharRegex.IsMatch(c);
        }

        public static bool IsVietnamese(string c)
        {
            return vnRegex.IsMatch(c);
        }

        public static int GetCurrentTimestamp()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string StripChineseChars(string input)
        {
            StringBuilder result = new StringBuilder(input);
            result = result.Replace('\uff0c', ',')
                           .Replace('\uff01', '!')
                           .Replace('\uff08', '(')
                           .Replace('\uff09', ')')
                           .Replace('\uff1a', ':')
                           .Replace('\uff1b', ';')
                           .Replace('\uff1f', '?')
                           .Replace('\uff5e', '~')
                           .Replace('\u2026', '.')
                           //.Replace('\u201c', '"')
                           //.Replace('\u201d', '"')
                           .Replace('—', '-')
                           .Replace('鐾', ' ')
                           .Replace('\u203b', '*')
                           .Replace('\u3000', ' ')
                           .Replace('\u3001', ',')
                           .Replace('\u3002', '.')
                           .Replace('\u300a', '(')
                           .Replace('\u300b', ')')
                           .Replace('\u3010', '[')
                           .Replace('\u3011', ']');
            return result.ToString();
        }
    }
}
