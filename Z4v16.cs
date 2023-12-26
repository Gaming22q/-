using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        List<Dish> dishes = new List<Dish>
        {
            new Dish { Name = "Суп", Type = "Первое", Calories = 150 },
            new Dish { Name = "Котлета", Type = "Второе", Calories = 500 },
            new Dish { Name = "Салат", Type = "Второе", Calories = 200 },
            new Dish { Name = "Омлет", Type = "Третье", Calories = 300 },
            new Dish { Name = "Компот", Type = "Первое", Calories = 100 },
            new Dish { Name = "Пирожное", Type = "Третье", Calories = 400 }
        };

        var averageCaloriesByType = dishes
            .GroupBy(dish => dish.Type)
            .Select(group => new
            {
                Type = group.Key,
                AverageCalories = group.Average(dish => dish.Calories)
            });

        foreach (var item in averageCaloriesByType)
        {
            Console.WriteLine($"Тип блюда: {item.Type}, Средняя калорийность: {item.AverageCalories}");
        }
    }
}

class Dish
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Calories { get; set; }
}