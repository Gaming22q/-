using System;
using System.Collections.Generic;

public class Hero<TArmor, TWeapon>
    where TArmor : Armor
    where TWeapon : Weapon
{
    public List<TArmor> Armor { get; set; }
    public List<TWeapon> Weapons { get; set; }

    public Hero(List<TArmor> armor, List<TWeapon> weapons)
    {
        Armor = armor;
        Weapons = weapons;
    }

    public void PrintEquipment()
    {
        Console.WriteLine("Hero's equipment:");
        Console.WriteLine("Armor:");
        foreach (var armor in Armor)
        {
            Console.WriteLine(armor.Name);
        }

        Console.WriteLine("Weapons:");
        foreach (var weapon in Weapons)
        {
            Console.WriteLine(weapon.Name);
        }
    }
}

public class Armor
{
    public string Name { get; set; }
}

public class Weapon
{
    public string Name { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Создание экземпляров доспехов и оружия
        var armor1 = new Armor { Name = "Plate Armor" };
        var armor2 = new Armor { Name = "Leather Armor" };
        var weapon1 = new Weapon { Name = "Sword" };
        var weapon2 = new Weapon { Name = "Bow" };

        // Создание экземпляра героя с передачей доспехов и оружия в конструктор
        var hero = new Hero<Armor, Weapon>(new List<Armor> { armor1, armor2 }, new List<Weapon> { weapon1, weapon2 });

        // Вывод списка вооружения героя
        hero.PrintEquipment();
    }
}
