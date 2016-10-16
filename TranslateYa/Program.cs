using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RestSharp;


namespace TranslateYa
{
    class Program
    {
        /// <summary>
        /// Ключ Яндрекс API
        /// </summary>
        private const string KEY = @"trnsl.1.1.20161013T093632Z.1bb500c65c946843.97d23279a8cc14b668481af3440c2247b87dcae8";
        private const string CLIENT = @"https://translate.yandex.net/";

        enum RequestType
        {
            Detect,
            Translate,
        }

        enum UserMessageEnum
        {
            /// <summary>
            /// Стартовое сообщение
            /// </summary>
            StartMessage,
            /// <summary>
            /// Выбор режима перевода: 
            /// 1)Выбираем язык с которого переводим и на который переводим
            /// 2)Выбираем язык на который переводим
            /// </summary>
            ChooseMode,
            /// <summary>
            /// Получили список языков и их коды
            /// </summary>
            ListLanguage,
            /// <summary>
            /// Выбираем язык с которого переводим
            /// </summary>
            ChooseStartLanguage,
            /// <summary>
            /// Выбираем язык на который переводим
            /// </summary>
            ChooseEndLanguage,
            /// <summary>
            /// Просим ввести текст для перевода
            /// </summary>
            WriteText,
            /// <summary>
            /// Сообщение о выводе переведенного текста
            /// </summary>
            TranslateText
        }

        struct Mode
        {
            public RequestType Type;
        }

        struct Message
        {
            public UserMessageEnum Code;
            public string Text;
        }

        static void Main(string[] args)
        {
            #region               
            Console.Write("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5} ", "Hello!", 
                "My application uses the yandex api to translate text.", 
                "Choose the mode:", 
                "1)Select language", 
                "2)Language auto-detection", 
                "Enter the number:");
            Mode _mode = new Mode();
            {
                _mode.Type = GetMode(Console.ReadKey());
            };
            Console.WriteLine("\r\nWrite your text:");
            string bstr = Console.ReadLine();
            Console.WriteLine("--------------");
            #endregion
            

            string parseText = XmlParse(CreateRequest(GetAction(_mode.Type), bstr, "ru"));
            Console.WriteLine(string.Format("{0}: {1}", "Translate text", parseText));
            Console.WriteLine("--------------");
        }
        static private string XmlParse(string xmlString)
        {
            var doc = new XmlDocument();
            {
                doc.LoadXml(xmlString);
                doc.SelectNodes("text");
                return doc.InnerText;
            }
            
        }
        static private string GetAction(RequestType type)
        {
            switch (type)
            {
                case RequestType.Detect:
                    return "detect";
                case RequestType.Translate:
                    return "translate";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static private string CreateRequest(string type, string text, string tlang)
        {
            var client = new RestClient(@CLIENT);
            RestRequest request = new RestRequest(@"/api/v1.5/tr/" + type + "?", Method.POST);
            {
                request.AddQueryParameter("key", KEY);
                request.AddQueryParameter("text", @text);
                request.AddQueryParameter("lang", @tlang);
            }

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            return content;
        }

        static private string CreateRequest(string type, string text, string blang, string tlang)
        {
            var client = new RestClient(@CLIENT);
            RestRequest request = new RestRequest(@"/api/v1.5/tr/" + type + "?", Method.POST);
            {
                request.AddQueryParameter("key", KEY);
                request.AddQueryParameter("text", @text);
                request.AddQueryParameter("lang", @blang+@"-"+@tlang);
            }

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            return content;
        }

        static private RequestType GetMode(ConsoleKeyInfo mode)
        {
            switch (mode.Key)
            {
                case ConsoleKey.D1:
                    return RequestType.Detect;
                case ConsoleKey.D2:
                    return RequestType.Translate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
