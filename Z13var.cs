public abstract class SportsEquipment
{
public string LoadType { get; set; }
public decimal Mass { get; set; }
}

public interface IEnergyConsuming
{
decimal Voltage { get; set; }
}

public class Treadmill : SportsEquipment, IEnergyConsuming
{
public decimal Length { get; set; }
public List<string> Modes { get; set; }
public decimal Voltage { get; set; }
}

public class ExerciseBike : SportsEquipment, IEnergyConsuming
{
public List<string> Modes { get; set; }
public decimal Voltage { get; set; }
}

public class Bench : SportsEquipment
{
public decimal MaxUserWeight { get; set; }
public decimal Incline { get; set; }
}

public class RomanChair : SportsEquipment
{
public decimal Incline { get; set; }
public decimal Dimensions { get; set; }
}

public class PullUpBar : SportsEquipment
{
public bool HasBar { get; set; }
public bool HasParallelBars { get; set; }
}
