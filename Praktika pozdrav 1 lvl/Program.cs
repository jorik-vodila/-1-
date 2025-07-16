using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;



class Program
{
    static readonly string Filesave = "save_file.json";

    static void save_in_file(List<Person> people)
    {
        var json = JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Filesave, json);
        Console.WriteLine("Список сохранён в файл.");
    }

    static List<Person> load_in_file()
    {
        if (!File.Exists(Filesave))
            return new List<Person>();

        var json = File.ReadAllText(Filesave);
        return JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
    }

    static Person createPerson()
    {
        Console.Write("Введите имя: ");
        string name = Console.ReadLine();

        Console.Write("Введите дату рождения (dd.MM.yyyy): ");
        DateTime birthDate;
        while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
        {
            Console.Write("Неверный формат, попробуйте ещё раз: ");
        }

        Console.Write("Введите контактные данные пользователя (email/телефон): ");
        string place = Console.ReadLine();

        return new Person { Name = name, BirthDate = birthDate, address = place };
    }

    static void output(List<Person> people)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }

        int count = 1;
        Console.WriteLine("Дни рождения:");
        foreach (var p in people)
        {
            Console.WriteLine($"{count}. {p}");
            count++;
        }
    }

    static void deletePerson(List<Person> people)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст, удалять нечего.");
            return;
        }

        output(people);
        Console.Write("Выберите номер записи для удаления: ");
        if (!int.TryParse(Console.ReadLine(), out int c) || c < 1 || c > people.Count)
        {
            Console.WriteLine("Такой записи нет, операция отменена.");
            return;
        }

        people.RemoveAt(c - 1);
        Console.WriteLine("Запись удалена.");
    }

    static void changePeople(List<Person> people)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст, менять нечего.");
            return;
        }

        output(people);
        Console.Write("Выберите номер записи для изменения: ");
        if (!int.TryParse(Console.ReadLine(), out int c) || c < 1 || c > people.Count)
        {
            Console.WriteLine("Такого человека нет, операция отменена.");
            return;
        }

        bool editing = true;
        while (editing)
        {
            Console.WriteLine($"Текущая информация: {people[c - 1]}");
            Console.WriteLine("Что хотите изменить?\n1. Имя\n2. Дату рождения\n3. Контакты\n4. Закончить");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Новое имя: ");
                    people[c - 1].Name = Console.ReadLine();
                    break;
                case "2":
                    Console.Write("Новая дата рождения (dd.MM.yyyy): ");
                    DateTime newBirthDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out newBirthDate))
                        Console.Write("Неверный формат, попробуйте ещё раз: ");
                    people[c - 1].BirthDate = newBirthDate;
                    break;
                case "3":
                    Console.Write("Новые контактные данные: ");
                    people[c - 1].address = Console.ReadLine();
                    break;
                case "4":
                    editing = false;
                    break;
                default:
                    Console.WriteLine("Такой команды нет, попробуйте ещё.");
                    break;
            }
        }
    }

    static void CurrentBirthDays(List<Person> people)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }

        int count = 0;
        var today = DateTime.Today;

        Console.WriteLine("Дни рождения в этом месяце, которые ещё впереди:");
        foreach (var p in people)
        {
            if (p.Check_BirthDate_today())
            {
                Console.WriteLine($"Сегодня день рождения у {p.Name}! Ему исполняется {today.Year - p.BirthDate.Year}!");
                count++;
            }
            else if (p.BirthDate.Month == today.Month && p.BirthDate.Day > today.Day)
            {
                Console.WriteLine(p);
                count++;
            }
        }

        if (count == 0)
            Console.WriteLine("В этом месяце нет предстоящих дней рождения.");
    }

    static void sort(List<Person> people)
    {
        people.Sort((a, b) =>
        {
            int res = a.BirthDate.Month.CompareTo(b.BirthDate.Month);
            if (res == 0)
                res = a.BirthDate.Day.CompareTo(b.BirthDate.Day);
            return res;
        });
        Console.WriteLine("Список отсортирован по дате рождения.");
    }

    static void menu_output(List<Person> people)
    {
        if (people.Count == 0)
        {
            Console.WriteLine("Список пуст.");
            return;
        }

        const int pageSize = 5;
        int totalPages = (people.Count + pageSize - 1) / pageSize;
        int currentPage = 1;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Страница {currentPage} из {totalPages}");
            Console.WriteLine(new string('-', 65));
            Console.WriteLine($"{"Имя",-15} | {"Дата рождения",-12} | {"Контакты",-20} | Статус |");
            Console.WriteLine(new string('-', 65));

            int start = (currentPage - 1) * pageSize;
            int end = Math.Min(start + pageSize, people.Count);

            for (int i = start; i < end; i++)
            {
                var p = people[i];
                Console.WriteLine($"{p.Name,-15} | {p.BirthDate:dd.MM.yyyy} | {p.address,-23} | {p.Check_BirthDate()} |");
            }

            Console.WriteLine(new string('-', 65));
            Console.WriteLine("n — next | b — back | q — quit");
            Console.Write("Выбор: ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "n" && currentPage < totalPages)
                currentPage++;
            else if (input == "b" && currentPage > 1)
                currentPage--;
            else if (input == "q")
                break;
        }
    }
    class Person
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string address { get; set; }

    public override string ToString()
    {
        return $"{Name} — {BirthDate:dd.MM.yyyy} - {address}";
    }

    public string Check_BirthDate()
    {
        var today = DateTime.Today;

        if (BirthDate.Month < today.Month || (BirthDate.Month == today.Month && BirthDate.Day < today.Day))
            return "уже было";
        if (BirthDate.Month == today.Month && BirthDate.Day == today.Day)
            return "сегодня";

         return "наступит";
    }

    public bool Check_BirthDate_today()
    {
        var today = DateTime.Today;
        return BirthDate.Month == today.Month && BirthDate.Day == today.Day;
    }
}
    static void Main()
    {
        List<Person> people = load_in_file();
        CurrentBirthDays(people);
        Console.WriteLine();

        bool programm = true;

        while (programm)
        {
            Console.WriteLine("-------------Меню-------------");
            Console.WriteLine("1. Добавить запись");
            Console.WriteLine("2. Вывести список");
            Console.WriteLine("3. Изменить запись");
            Console.WriteLine("4. Удалить запись");
            Console.WriteLine("5. Сортировка списка");
            Console.WriteLine("6. Ближайшие дни рождения");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "0":
                    save_in_file(people);
                    programm = false;
                    break;
                case "1":
                    var person = createPerson();
                    people.Add(person);
                    break;
                case "2":
                    menu_output(people);
                    break;
                case "3":
                    changePeople(people);
                    break;
                case "4":
                    deletePerson(people);
                    break;
                case "5":
                    sort(people);
                    break;
                case "6":
                    CurrentBirthDays(people);
                    break;
                default:
                    Console.WriteLine("Неверный пункт, попробуйте снова.");
                    break;
            }
            Console.WriteLine();
        }
    }
}
