Console.WriteLine("INLÄMNINGSUPPGIFT: Prague Parking 1.0\n\nVälkommen! Hur kan vi hjälpa dig idag?\n");
string[] parking = new string[100];

while (true)
{
    Console.Write("\nMENY:" +
        "\n\t1. Ta emot fordon" +
        "\n\t2. Flytta fordon" +
        "\n\t3. Hämta ut fordon" +
        "\n\t4. Sök fordon" +
        "\n\t5. Avsluta" +
        "\n\nVälj ett alternativ (1-5): ");

    int userResponse = int.Parse(Console.ReadLine());

    switch (userResponse)
    {
        case 1: CheckInVehicle(); break;
        case 2: MoveVehicle(); break;
        case 3: CheckOutVehicle(); break;
        case 4: SearchVehicle(); break;
        case 5:
            Console.WriteLine("Avslutar programmet.\nTack för du använder Prague Parking. \nHej då!");
            return;
        default:
            Console.WriteLine("Ogiltigt val, försök igen.");
            break;
    }
}

void CheckInVehicle()
{
    Console.Write("Vänligen ange fordonstyp (CAR/MC): ");
    string vehicleType = Console.ReadLine().ToUpper();
    Console.Write("Vänligen ange registreringsnummer: ");
    string regNum = Console.ReadLine().ToUpper().Trim();

    if (FindVehicle(regNum) != -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} är redan parkerat!");
        return;
    }

    string vehicle = vehicleType + "#" + regNum;
    for (int i = 0; i < parking.Length; i++)
    {
        if (vehicleType == "CAR" && string.IsNullOrEmpty(parking[i]))
        {
            parking[i] = vehicle;
            Console.WriteLine($"{vehicle} är parkerad på parkeringsplats: {i + 1}.");
            return;
        }
        else if (vehicleType == "MC")
        {
            if (string.IsNullOrEmpty(parking[i]))
            {
                parking[i] = vehicle;
                Console.WriteLine($"{vehicle} är parkerad på parkeringsplats: {i + 1}.");
                return;
            }
            else if (parking[i].StartsWith("MC") && !parking[i].Contains("|"))
            {
                Console.WriteLine($"{vehicle} parkerad tillsammans med en annan {parking[i]} på plats: {i + 1}.");
                parking[i] += "|" + vehicle;
                return;
            }
        }
    }
    Console.WriteLine("Ingen ledig parkeringsplats tillgänglig.");
}

void MoveVehicle()
{
    Console.Write("Vänligen ange registreringsnummer: ");
    string regNum = Console.ReadLine().ToUpper().Trim();
    int fromIndex = FindVehicle(regNum);

    if (fromIndex == -1)
    {
        Console.WriteLine($"OBS! Fordon hittades inte.");
        return;
    }

    Console.WriteLine($"Fordon {regNum} hittades på plats {fromIndex + 1}.");
    Console.Write("Vänligen ange ny plats (1-100): ");
    int toIndex = int.Parse(Console.ReadLine()) - 1;

    bool isCar = parking[fromIndex].Contains("CAR#" + regNum);
    string toSpace = parking[toIndex];

    if (!string.IsNullOrEmpty(toSpace) && (isCar || !toSpace.StartsWith("MC") || toSpace.Contains("|")))
    {
        Console.WriteLine(isCar ? "OBS! Bilar kan endast parkera på tomma platser!" : "OBS! Platsen är inte tillgänglig för motorcykel!");
        return;
    }

    string vehicle = ExtractVehicle(fromIndex, regNum);
    if (string.IsNullOrEmpty(parking[toIndex]))
        parking[toIndex] = vehicle;
    else
        parking[toIndex] += "|" + vehicle;

    Console.WriteLine($"Fordon flyttat till plats {toIndex + 1}.");
}

void CheckOutVehicle()
{
    Console.WriteLine("Vänligen ange registreringsnummer på fordonet du vill hämta ut: ");
    string regNum = Console.ReadLine().ToUpper().Trim();
    int index = FindVehicle(regNum);

    if (index == -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} hittades inte.");
        return;
    }

    if (parking[index].Contains("|"))
    {
        string[] vehicles = parking[index].Split('|');
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (!vehicles[i].Contains("#" + regNum))
            {
                parking[index] = vehicles[i];
                break;
            }
        }
    }
    else
    {
        parking[index] = "";
    }

    Console.WriteLine($"Fordon {regNum} har hämtats ut från plats {index + 1}.");
}

void SearchVehicle()
{
    Console.WriteLine("Vänligen ange registreringsnummer att söka efter: ");
    string regNum = Console.ReadLine().ToUpper().Trim();
    int index = FindVehicle(regNum);

    if (index == -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} hittades inte.");
        return;
    }

    Console.WriteLine($"\nFORDONSINFORMATION:\n\tRegistreringsnummer: {regNum}\n\tParkeringsplats: {index + 1}");

    if (parking[index].StartsWith("CAR"))
    {
        Console.WriteLine("\tFordonstyp: Bil\n\tStatus: Parkerad ensam");
    }
    else if (parking[index].Contains("|"))
    {
        string[] motorcycles = parking[index].Split('|');
        string otherReg = "";
        for (int i = 0; i < motorcycles.Length; i++)
        {
            if (!motorcycles[i].Contains("#" + regNum))
            {
                otherReg = motorcycles[i].Split('#')[1];
                break;
            }
        }
        Console.WriteLine($"\tFordonstyp: Motorcykel\n\tStatus: Parkerad tillsammans med en annan motorcykel\n\tDelar plats med: {otherReg}");
    }
    else
    {
        Console.WriteLine("\tFordonstyp: Motorcykel\n\tStatus: Parkerad ensam");
    }
}

int FindVehicle(string regNum)
{
    for (int i = 0; i < parking.Length; i++)
    {
        if (!string.IsNullOrEmpty(parking[i]) && parking[i].Contains("#" + regNum))
            return i;
    }
    return -1;
}

string ExtractVehicle(int index, string regNum)
{
    if (parking[index].Contains("|"))
    {
        string[] vehicles = parking[index].Split('|');
        string vehicle = "";
        string remaining = "";

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].Contains("#" + regNum))
                vehicle = vehicles[i];
            else
                remaining = vehicles[i];
        }

        parking[index] = remaining;
        return vehicle;
    }

    string temp = parking[index];
    parking[index] = "";
    return temp;
}
