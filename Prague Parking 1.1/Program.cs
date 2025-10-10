using System.Text.RegularExpressions;

Console.WriteLine("INLÄMNINGSUPPGIFT: Prague Parking 1.1\n\nVälkommen! Hur kan vi hjälpa dig idag?");
string[] parking = new string[100];

while (true)
{
    Console.Write("\nMENY:" +
        "\n\t1. Ta emot fordon" +
        "\n\t2. Flytta fordon" +
        "\n\t3. Hämta ut fordon" +
        "\n\t4. Sök fordon" +
        "\n\t5. Visa parkeringsplatser" +
        "\n\t6. Avsluta" +
        "\n\nVälj ett alternativ (1-6): ");

    int userResponse = int.Parse(Console.ReadLine());

    switch (userResponse)
    {
        case 1: CheckInVehicle(); break;
        case 2: MoveVehicle(); break;
        case 3: CheckOutVehicle(); break;
        case 4: SearchVehicle(); break;
        case 5: ViewParking(); break;
        case 6:
            Console.WriteLine("Avslutar programmet. \nTack för du använder Prague Parking. \nHej då!");
            return;
        default:
            Console.WriteLine("Ogiltigt val, försök igen.");
            break;
    }
}

void CheckInVehicle()
{
    string vehicleType = GetValidVehicleType();
    string regNum = GetValidRegistrationNumber("Vänligen ange registreringsnummer (max 10 tecken, inga mellanslag): ");

    if (FindVehicle(regNum) != -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} är redan parkerat!");
        return;
    }

    string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
    string vehicle = vehicleType + "#" + regNum + "#" + timestamp;

    for (int i = 0; i < parking.Length; i++)
    {
        if (vehicleType == "CAR" && string.IsNullOrEmpty(parking[i]))
        {
            parking[i] = vehicle;
            Console.WriteLine($"{vehicleType}#{regNum} parkerad på plats {i + 1} kl. {DateTime.Now:HH:mm}.");
            return;
        }
        else if (vehicleType == "MC")
        {
            if (string.IsNullOrEmpty(parking[i]))
            {
                parking[i] = vehicle;
                Console.WriteLine($"{vehicleType}#{regNum} parkerad på plats {i + 1} kl. {DateTime.Now:HH:mm}.");
                return;
            }
            else if (parking[i].StartsWith("MC") && !parking[i].Contains("|"))
            {
                Console.WriteLine($"{vehicleType}#{regNum} parkerad tillsammans med annan MC på plats {i + 1} kl. {DateTime.Now:HH:mm}.");
                parking[i] += "|" + vehicle;
                return;
            }
        }
    }
    Console.WriteLine("Ingen ledig parkeringsplats tillgänglig.");
}

void MoveVehicle()
{
    string regNum = GetValidRegistrationNumber("Vänligen ange registreringsnummer på fordonet du vill flytta: ");
    int fromIndex = FindVehicle(regNum);

    if (fromIndex == -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} hittades inte.");
        return;
    }

    Console.WriteLine($"Fordon {regNum} hittades på plats {fromIndex + 1}.");
    int toIndex = GetValidDestination() - 1;

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

    Console.WriteLine($"Fordon {regNum} har flyttats från plats {fromIndex + 1} till plats {toIndex + 1}.");
}

void CheckOutVehicle()
{
    string regNum = GetValidRegistrationNumber("Vänligen ange registreringsnummer på fordonet du vill hämta ut: ");
    int index = FindVehicle(regNum);

    if (index == -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} hittades inte.");
        return;
    }

    string vehicleData = "";
    if (parking[index].Contains("|"))
    {
        string[] vehicles = parking[index].Split('|');
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].Contains("#" + regNum + "#"))
            {
                vehicleData = vehicles[i];
                for (int j = 0; j < vehicles.Length; j++)
                {
                    if (j != i)
                    {
                        parking[index] = vehicles[j];
                        break;
                    }
                }
                break;
            }
        }
    }
    else
    {
        vehicleData = parking[index];
        parking[index] = "";
    }

    string[] parts = vehicleData.Split('#');
    DateTime entryTime = DateTime.Parse(parts[2]);
    DateTime exitTime = DateTime.Now;
    TimeSpan duration = exitTime - entryTime;

    Console.WriteLine($"\nFordon {regNum} har hämtats ut från plats {index + 1}.");
    Console.WriteLine($"Parkerad sedan: {entryTime:yyyy-MM-dd HH:mm}");
    Console.WriteLine($"Utcheckning: {exitTime:yyyy-MM-dd HH:mm}");
    Console.WriteLine($"Parkerad tid: {duration.Days} dagar, {duration.Hours} timmar, {duration.Minutes} minuter");
}

void SearchVehicle()
{
    string regNum = GetValidRegistrationNumber("Vänligen ange registreringsnummer att söka efter: ");
    int index = FindVehicle(regNum);

    if (index == -1)
    {
        Console.WriteLine($"OBS! Fordon med registreringsnummer {regNum} hittades inte.");
        return;
    }

    string vehicleData = "";
    if (parking[index].Contains("|"))
    {
        string[] vehicles = parking[index].Split('|');
        foreach (string v in vehicles)
        {
            if (v.Contains("#" + regNum + "#"))
            {
                vehicleData = v;
                break;
            }
        }
    }
    else
    {
        vehicleData = parking[index];
    }

    string[] parts = vehicleData.Split('#');
    DateTime parkTime = DateTime.Parse(parts[2]);
    TimeSpan parkedDuration = DateTime.Now - parkTime;

    Console.WriteLine($"\nFORDONSINFORMATION:");
    Console.WriteLine($"\tRegistreringsnummer: {regNum}");
    Console.WriteLine($"\tParkeringsplats: {index + 1}");
    Console.WriteLine($"\tParkerad sedan: {parkTime:yyyy-MM-dd HH:mm}");
    Console.WriteLine($"\tParkerad tid: {parkedDuration.Days} dagar, {parkedDuration.Hours} timmar, {parkedDuration.Minutes} minuter");

    if (parking[index].StartsWith("CAR"))
    {
        Console.WriteLine("\tFordonstyp: Bil");
        Console.WriteLine("\tStatus: Parkerad ensam");
    }
    else if (parking[index].Contains("|"))
    {
        string[] motorcycles = parking[index].Split('|');
        string otherReg = "";
        for (int i = 0; i < motorcycles.Length; i++)
        {
            if (!motorcycles[i].Contains("#" + regNum + "#"))
            {
                otherReg = motorcycles[i].Split('#')[1];
                break;
            }
        }
        Console.WriteLine("\tFordonstyp: Motorcykel");
        Console.WriteLine("\tStatus: Parkerad tillsammans med en annan motorcykel");
        Console.WriteLine($"\tDelar plats med: {otherReg}");
    }
    else
    {
        Console.WriteLine("\tFordonstyp: Motorcykel");
        Console.WriteLine("\tStatus: Parkerad ensam");
    }
}

int FindVehicle(string regNum)
{
    for (int i = 0; i < parking.Length; i++)
    {
        if (!string.IsNullOrEmpty(parking[i]) && parking[i].Contains("#" + regNum + "#"))
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
            if (vehicles[i].Contains("#" + regNum + "#"))  
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

void ViewParking()
{
    Console.WriteLine("\nPARKERINGSÖVERSIKT:\n");

    int emptyCount = 0;
    int carCount = 0;
    int singleMcCount = 0;
    int doubleMcCount = 0;

    for (int i = 0; i < parking.Length; i++)
    {
        string space = parking[i];
        int spaceNumber = i + 1;
        string status;

        if (string.IsNullOrEmpty(space))
        {
            status = "Tom";
            emptyCount++;
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }
        else if (space.StartsWith("CAR"))
        {
            status = "BIL";
            carCount++;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }
        else if (space.Contains("|"))
        {
            status = "MC+MC";
            doubleMcCount++;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }
        else
        {
            status = "MC";
            singleMcCount++;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }

        Console.Write($"[{spaceNumber,3}:{status,-5}] ");
        Console.ResetColor();

        if (spaceNumber % 10 == 0)
            Console.WriteLine();
    }

    Console.WriteLine($"\n\nSTATUS:" +
        $"\n\tTomma platser: {emptyCount}" +
        $"\n\tBilar: {carCount}" +
        $"\n\tMotorcyklar (ensam): {singleMcCount}" +
        $"\n\tMotorcyklar (dubbel): {doubleMcCount}" +
        $"\n\tTotalt använda platser: {100 - emptyCount}/100");

    Console.WriteLine("\nFORDONS LISTA:");
    for (int i = 0; i < parking.Length; i++)
    {
        if (!string.IsNullOrEmpty(parking[i]))
        {
            string[] vehicles = parking[i].Split('|');
            for (int j = 0; j < vehicles.Length; j++)
            {
                string[] parts = vehicles[j].Split('#');
                string display = parts[0] + "#" + parts[1];
                DateTime parkTime = DateTime.Parse(parts[2]);
                Console.WriteLine($"\tPlats {i + 1,3}: {display} \t(Parkerad: {parkTime:yyyy-MM-dd HH:mm})");
            }
        }
    }
}

string GetValidRegistrationNumber(string prompt)
{
    Regex validInput = new Regex("^[a-zA-Z0-9äöåüÄÖÅÜ]+$");

    while (true)
    {
        Console.Write(prompt);
        string regNum = Console.ReadLine().ToUpper().Trim();

        if (regNum.Length > 10)
        {
            Console.WriteLine("OBS! Max 10 tecken!");
            continue;
        }

        if (!validInput.IsMatch(regNum))
        {
            Console.WriteLine("OBS! Får endast innehålla siffror och bokstäver A-Z samt ÅÄÖÜ!");
            continue;
        }

        return regNum;
    }
}

string GetValidVehicleType()
{
    while (true)
    {
        Console.Write("Vänligen ange fordonstyp (CAR/MC): ");
        string fordonsTyp = Console.ReadLine().ToUpper();

        if (fordonsTyp == "CAR" || fordonsTyp == "MC")
            return fordonsTyp;

        Console.WriteLine("OBS! Välj endast CAR eller MC!\n");
    }
}

int GetValidDestination()
{
    int destination = 0;
    while (destination < 1 || destination > 100)
    {
        Console.Write("Vänligen ange parkeringsplats att flytta till (1-100): ");
        destination = int.Parse(Console.ReadLine());

        if (destination < 1 || destination > 100)
            Console.WriteLine("OBS! Välj mellan 1-100.");
    }
    return destination;
}