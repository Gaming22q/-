using System;
using System.Collections.Generic;

// Обобщенный класс-наследник обобщенной коллекции указанного типа
public class CustomList<T> : List<T>
{
    // Метод, вычисляющий суммарную длину строк в списке
    public int TotalLengthOfStrings()
    {
        int totalLength = 0;
        foreach (var item in this)
        {
            if (item is string str)
            {
                totalLength += str.Length;
            }
        }
        return totalLength;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создание экземпляра обобщенного класса
        CustomList<string> stringList = new CustomList<string>();

        // Заполнение списка пятью элементами типа string
        stringList.Add("Hello");
        stringList.Add("World");
        stringList.Add("C#");

        // Вызов метода для вычисления суммарной длины строк в списке
        int totalLength = stringList.TotalLengthOfStrings();

        // Вывод результата в консоль
        Console.WriteLine($"Суммарная длина строк в списке: {totalLength}");
    }
}
