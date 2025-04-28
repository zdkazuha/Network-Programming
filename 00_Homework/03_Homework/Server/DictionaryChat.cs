using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DictionaryChat
    {
        public Dictionary<List<string>, List<string>> Messages { get; set; } = new Dictionary<List<string>, List<string>>();

        public DictionaryChat()
        {
            FillDictionary();
        }
        public void FillDictionary()
        {
            Messages.Add(new List<string> { "Привіт", "Хай", "привіт", }, new List<string> { "Привіт", "Привіт, як ти" });
            Messages.Add(new List<string> { "Добре", "Погано", "Так собі","Дуже добре", "Дуже погано" }, new List<string> { "Зрозуміло"});
            Messages.Add(new List<string> { "Як твої справи?" }, new List<string> { "Непогано а в тебе?", "Погано а в тебе?", "Добре а в тебе?" });
            Messages.Add(new List<string> { "Як твій настрій?" }, new List<string> { "Непогано а в тебе?", "Погано а в тебе?", "Добре а в тебе?" });
            Messages.Add(new List<string> { "Що сьогодні робив?" }, new List<string> { "Працював дуже втомивсья а ти що робив?", "Відпочивав та гарно відпочив ти що робив?", "Трішкі попрацював та відпочив а ти що робив?" });
            Messages.Add(new List<string> { "Як погода?" }, new List<string> { "Сонячно", "Хмарно", "Дождь", "Жарко" });
            Messages.Add(new List<string> { "Як температура на вулиці?" }, new List<string> { "На вулиці 12", "На вулиці 16", "На вулиці 21", "На вулиці 5" });
            Messages.Add(new List<string> { "Що їв сьогодні?" }, new List<string> { "Кашу, а ти?", "Бутерброди, а ти?", "Суп, а ти?" });
            Messages.Add(new List<string> { "Який твій улюблений фільм?" }, new List<string> { "Володар перснів", "Інтерстеллар", "Матриця" });
            Messages.Add(new List<string> { "Який твій улюблений фанр фільму?" }, new List<string> { "Фантастика", "Історичні", "Комедія","Бойовики" });
            Messages.Add(new List<string> { "Ти займаєшся спортом?" }, new List<string> { "Так, бігаю", "Так, займаюсь у спортзалі", "Ні, не займаюсь" });
            Messages.Add(new List<string> { "Який сьогодні день тижня?" }, new List<string> { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" });
            Messages.Add(new List<string> { "Тобі подобається музика?" }, new List<string> { "Так, дуже!", "Не дуже", "Люблю тільки інструментальну" });
            Messages.Add(new List<string> { "Граєш у відеоігри?" }, new List<string> { "Так, часто", "Рідко", "Не граю" });
            Messages.Add(new List<string> { "Чи подорожував ти колись?" }, new List<string> { "Так, люблю подорожі", "Ні, ще ні", "Планую подорожувати" });
            Messages.Add(new List<string> { "Яку мову хочеш вивчити?" }, new List<string> { "Англійську", "Німецьку", "Іспанську", "Японську" });
            Messages.Add(new List<string> { "Який улюблений колір?" }, new List<string> { "Синій", "Червоний", "Зелений" });
            Messages.Add(new List<string> { "Який улюблений напій?" }, new List<string> { "Чай", "Кава", "Сік", "Вода" });
            Messages.Add(new List<string> { "Який улюблений жанр музики?" }, new List<string> { "Рок", "Поп", "Класика", "Реп" });
            Messages.Add(new List<string> { "Як проходить твій день?" }, new List<string> { "Добре", "Середньо", "Погано" });
            Messages.Add(new List<string> { "Ти любиш тварин?" }, new List<string> { "Так, дуже", "Тільки котів", "Тільки собак" });
            Messages.Add(new List<string> { "Ти любиш читати?" }, new List<string> { "Так, дуже!", "Ні, не люблю", "Іноді читаю" });
            Messages.Add(new List<string> { "Ти любиш малювати?" }, new List<string> { "Так, люблю", "Не вмію", "Хочу навчитись" });
            Messages.Add(new List<string> { "Дякую","Дуже вдячний" }, new List<string> { "Нема за що","Звертайсья ще" });

        }
        private static Random random = new Random();

        public string GetResponse(string message)
        {
            foreach (var entry in Messages)
            {
                if (entry.Key.Contains(message))
                {
                    var responses = entry.Value;
                    return responses[random.Next(responses.Count)];
                }
            }
            return "На жаль я не можу відповисти на ваше запитання";
        }


    }
}
