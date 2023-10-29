using System;
using System.Collections.Generic;
using System.Linq;


// // ����������� ������� ����� "����������"
public class Ingredient
{
public string Name { get; set; }
public double CostPer100g { get; set; }
public double Quantity { get; set; }

public Ingredient(string name, double costPer100g, double quantity)
{
Name = name;
CostPer100g = costPer100g;
Quantity = quantity;
}

public double GetCost()
{
return CostPer100g * Quantity / 100;
}
}

// ����� ����� ��������� �� �����������
public class Salad // 
{
private List<Ingredient> Ingredients { get; set; }

public Salad()
{
Ingredients = new List<Ingredient>
{
new Ingredient("Tomato", 1.5, 200), // �����
new Ingredient("Cucumber", 1.0, 150), // ������
new Ingredient("Onion", 0.5, 50), // ���
new Ingredient("Olive Oil", 2.0, 30), // ������
new Ingredient("Salt", 0.1, 5) // ����
};
}

public double GetCost()
{
return Ingredients.Sum(i => i.GetCost());
}

public override string ToString()
{
var ingredients = string.Join(", ", Ingredients.Select(i => i.Name));
return $"{ingredients}, {GetCost()}";
}
}