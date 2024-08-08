using TransactionApi.Model.Entity;

namespace TransactionApi.Test.Helpers;

public static class TestDataHelper
{
    public static List<TransactionEntity> CreateTestData()
    {
       return new List<TransactionEntity>
        {
            new()
            {
                TransactionId = "T1",
                Name = "John Doe",
                Email = "john.doe@example.com",
                Amount = 100.0m,
                TransactionDate = DateTime.Parse("2023-12-30 23:50:00"),
                ClientLocation = "40.712776, -74.005974" // New York
            },
            new ()
            {
                TransactionId = "T2",
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Amount = 200.0m,
                TransactionDate = DateTime.Parse("2023-12-31 00:00:00"),
                ClientLocation = "34.052235, -118.243683" // Los Angeles
            },
            new ()
            {
                TransactionId = "T3",
                Name = "Alice Johnson",
                Email = "alice.johnson@example.com",
                Amount = 150.0m,
                TransactionDate = DateTime.Parse("2024-01-01 00:00:00"),
                ClientLocation = "51.507351, -0.127758" // London
            },
            new ()
            {
                TransactionId = "T4",
                Name = "Bob Brown",
                Email = "bob.brown@example.com",
                Amount = 175.0m,
                TransactionDate = DateTime.Parse("2024-01-02 00:00:00"),
                ClientLocation = "48.856613, 2.352222" // Paris
            },
            new ()
            {
                TransactionId = "T5",
                Name = "Charlie Davis",
                Email = "charlie.davis@example.com",
                Amount = 300.0m,
                TransactionDate = DateTime.Parse("2024-01-02 12:00:00"),
                ClientLocation = "35.689487, 139.691711" // Tokyo
            },
            new ()
            {
                TransactionId = "T6",
                Name = "Diana Evans",
                Email = "diana.evans@example.com",
                Amount = 400.0m,
                TransactionDate = DateTime.Parse("2024-01-03 00:00:00"),
                ClientLocation = "55.755825, 37.617298" // Moscow
            },
            new ()
            {
                TransactionId = "T7",
                Name = "Evan Foster",
                Email = "evan.foster@example.com",
                Amount = 250.0m,
                TransactionDate = DateTime.Parse("2024-01-04 00:00:00"),
                ClientLocation = "40.730610, -73.935242" // New York
            },
            new ()
            {
                TransactionId = "T8",
                Name = "Fiona Green",
                Email = "fiona.green@example.com",
                Amount = 320.0m,
                TransactionDate = DateTime.Parse("2024-01-05 00:00:00"),
                ClientLocation = "34.052235, -118.243683" // Los Angeles
            },
            new ()
            {
                TransactionId = "T9",
                Name = "George Harris",
                Email = "george.harris@example.com",
                Amount = 150.0m,
                TransactionDate = DateTime.Parse("2024-01-06 00:00:00"),
                ClientLocation = "51.507351, -0.127758" // London
            },
            new ()
            {
                TransactionId = "T10",
                Name = "Hannah White",
                Email = "hannah.white@example.com",
                Amount = 280.0m,
                TransactionDate = DateTime.Parse("2024-01-30 12:30:00"),
                ClientLocation = "48.856613, 2.352222" // Paris
            },
            new ()
            {
                TransactionId = "T11",
                Name = "Ian King",
                Email = "ian.king@example.com",
                Amount = 350.0m,
                TransactionDate = DateTime.Parse("2024-01-30 23:30:00"),
                ClientLocation = "35.689487, 139.691711" // Tokyo
            },
            new ()
            {
                TransactionId = "T12",
                Name = "Jack Lee",
                Email = "jack.lee@example.com",
                Amount = 410.0m,
                TransactionDate = DateTime.Parse("2024-01-31 00:00:00"),
                ClientLocation = "55.755825, 37.617298" // Moscow
            },
            new ()
            {
                TransactionId = "T13",
                Name = "Karen Moore",
                Email = "karen.moore@example.com",
                Amount = 290.0m,
                TransactionDate = DateTime.Parse("2024-02-01 00:00:00"),
                ClientLocation = "40.730610, -73.935242" // New York
            },
            new ()
            {
                TransactionId = "T14",
                Name = "Liam Nelson",
                Email = "liam.nelson@example.com",
                Amount = 330.0m,
                TransactionDate = DateTime.Parse("2024-02-02 00:00:00"),
                ClientLocation = "34.052235, -118.243683" // Los Angeles
            },
            new ()
            {
                TransactionId = "T15",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-02-02 03:00:00"),
                ClientLocation = "51.507351, -0.127758" // London
            },
            new ()
            {
                TransactionId = "T16",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-05-02 03:00:00"),
                ClientLocation = "51.507351, -0.127758" 
            },
            new ()
            {
                TransactionId = "T17",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-05-19 03:00:00"),
                ClientLocation = "34.052235, -118.243683" 
            },
            new ()
            {
                TransactionId = "T18",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-06-12 03:00:00"),
                ClientLocation = "40.730610, -73.935242" 
            },
            new ()
            {
                TransactionId = "T19",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-06-30 03:00:00"),
                ClientLocation = "55.755825, 37.617298" 
            },
            new ()
            {
                TransactionId = "T20",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-06-27 03:00:00"),
                ClientLocation = "51.507351, -0.127758" 
            },
            new ()
            {
                TransactionId = "T21",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-05-12 03:00:00"),
                ClientLocation = "5.689487, 139.691711" 
            },
            new ()
            {
                TransactionId = "T22",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-07-21 03:00:00"),
                ClientLocation = "48.856613, 2.352222" 
            },
            new ()
            {
                TransactionId = "T23",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-08-15 03:00:00"),
                ClientLocation = "34.052235, -118.243683"
            },
            new ()
            {
                TransactionId = "T24",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-07-01 03:00:00"),
                ClientLocation = "55.755825, 37.617298"
            },
            new ()
            {
                TransactionId = "T25",
                Name = "Mia Parker",
                Email = "mia.parker@example.com",
                Amount = 180.0m,
                TransactionDate = DateTime.Parse("2024-06-11 03:00:00"),
                ClientLocation = "5.689487, 139.691711" 
            },
        };
    }
}