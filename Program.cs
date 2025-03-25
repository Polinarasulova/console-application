using System;
using System.IO;

public delegate bool NameValidationHandler(string name);

// класс события, который передаёт информацию о пользователе
public class UserEventArgs : EventArgs
{
    public User User { get; }

    public UserEventArgs(User user)
    {
        User = user;
    }
}

// класс пользователя
public class User
{
    public event EventHandler<UserEventArgs> OnUserCreated;

    // Свойства пользователя
    public string Name { get; }
    public int Age { get; }
    public bool IsAdult => Age >= 18; 
    public User(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // вызывает событие, когда пользователь сзаписан
    public void CreateUser()
    {
        OnUserCreated?.Invoke(this, new UserEventArgs(this));
    }
}

// База
class Program
{
    static void Main()
    {
        Console.Write("Введите как к вам обращаться(ИМЯ): ");
        string name = Console.ReadLine();

        // Валидация имени через делегат
        NameValidationHandler validateName = s => !string.IsNullOrEmpty(s);
        if (!validateName(name))
        {
            Console.WriteLine("!!!!!!Ошибка!!!!!!:  (Вы осавили имя(первую строчку) пустым)");
            return;
        }

        Console.Write("Введите ваш настоящий возраст: ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age < 0)
        {
            Console.WriteLine("!!!!!!Ошибка!!!!!!: не разыгрывайте меня (возраст должен быть положительным числом)");
            return;
        }

        User user = new User(name, age);
        user.OnUserCreated += HandleUserCreated;
        user.CreateUser(); 
    }

    private static void HandleUserCreated(object sender, UserEventArgs e)
    {
        Console.WriteLine($"Привет, {e.User.Name}!");
        WriteUserDataToFile(e.User);
    }

    // Метод для записи данных пользователя в файл
    private static void WriteUserDataToFile(User user)
    {
        string filePath;
        if (user.IsAdult)
        {
            filePath = "Users.txt"; // 18+
        }
        else if (user.Age >= 14)
        {
            filePath = "teenagers.txt"; // подростки
        }
        else
        {
            filePath = "children.txt"; // дети
        }

        string record = $"{DateTime.Now}: Пользователь(солнышко) {user.Name}, возраст {user.Age} лет\n";

        // Запись данных в файл
        File.AppendAllText(filePath, record);
        Console.WriteLine($"Данные сохранены в файл: {filePath} (большое спасибо за взаимодействия)");
    }
}