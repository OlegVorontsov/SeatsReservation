using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatsReservation.Domain.Entities.Events;
using SeatsReservation.Domain.Entities.Reservations;
using SeatsReservation.Domain.Entities.Users;
using SeatsReservation.Domain.Entities.Venues;
using SeatsReservation.Infrastructure.Postgres.Write;

namespace SeatsReservation.Infrastructure.Postgres.Seeding;

public class ReservationSeeder(
    ApplicationWriteDbContext dbContext,
    ILogger<ReservationSeeder> logger) : ISeeder
{
    // Константы для количества данных
    private const int USERS_COUNT = 500;
    private const int VENUES_COUNT = 500;
    private const int SEATS_PER_VENUE_MIN = 50;
    private const int SEATS_PER_VENUE_MAX = 500;
    private const int EVENTS_COUNT = 5000;

    private readonly Random _random = new();

    public async Task SeedAsync()
    {
        logger.LogInformation("ReservationSeeder started");

        try
        {
            await SeedData();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while reservation seeding");
            throw;
        }
        
        logger.LogInformation("Reservation seeding done");
    }

     private async Task SeedData()
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            // Очистка базы данных
            await ClearDatabase();

            // Сидирование данных батчами
            await SeedUsersBatched();
            await SeedVenuesAndSeatsBatched();
            await SeedEventsBatched();
            await SeedReservationsBatched();

            await transaction.CommitAsync();
            logger.LogInformation("Seeding completed successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task ClearDatabase()
    {
        logger.LogInformation("Clearing database...");

        // Удаляем в правильном порядке из-за внешних ключей
        dbContext.ReservationSeats.RemoveRange(dbContext.ReservationSeats);
        dbContext.Reservations.RemoveRange(dbContext.Reservations);
        dbContext.Events.RemoveRange(dbContext.Events);
        dbContext.Seats.RemoveRange(dbContext.Seats);
        dbContext.Venues.RemoveRange(dbContext.Venues);
        dbContext.Set<User>().RemoveRange(dbContext.Set<User>());

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Database cleared.");
    }

    private async Task SeedUsersBatched()
    {
        logger.LogInformation("Seeding users in batches...");

        const int batchSize = 1000;
        var firstNames = new[]
        {
            "Александр", "Елена", "Дмитрий", "Анна", "Михаил", "Ольга", "Сергей", "Наталья", "Владимир", "Татьяна"
        };
        var lastNames = new[]
        {
            "Иванов", "Петров", "Сидоров", "Козлов", "Новиков", "Морозов", "Петухов", "Обухов", "Калинин", "Лебедев"
        };

        // Отключаем отслеживание изменений для ускорения
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        var users = new List<User>();
        for (var i = 0; i < USERS_COUNT; i++)
        {
            var firstName = firstNames[_random.Next(firstNames.Length)];
            var lastName = lastNames[_random.Next(lastNames.Length)];

            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Details = new UserDetails()
                {
                    FIO = $"{firstName} {lastName}",
                    Description = $"Пользователь {firstName} {lastName}",
                    Socials = GenerateRandomSocials()
                }
            });

            if (users.Count < batchSize) continue;

            dbContext.Set<User>().AddRange(users);
            await dbContext.SaveChangesAsync();
            users.Clear();
        }

        if (users.Count != 0)
        {
            dbContext.Set<User>().AddRange(users);
            await dbContext.SaveChangesAsync();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        logger.LogInformation("Seeded {UsersCount} users.", USERS_COUNT);
    }

    private async Task SeedVenuesAndSeatsBatched()
    {
        logger.LogInformation("Seeding venues and seats in batches...");

        var venuePrefixes = new[]
        {
            "MSK", "SPB", "EKB", "NSK", "KZN", "NN", "CHE", "SMR", "UFA", "RND"
        };
        var venueNames = new[]
        {
            "Центральный зал", "Концертный холл", "Конференц-зал", "Театральный зал", "Большой зал", "Малый зал",
            "Летняя площадка", "Крытый павильон", "Амфитеатр", "Арена"
        };

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        // Генерируем все площадки
        var venues = new List<Venue>();
        for (var i = 0; i < VENUES_COUNT; i++)
        {
            var prefix = venuePrefixes[_random.Next(venuePrefixes.Length)];
            var name = venueNames[_random.Next(venueNames.Length)];
            var seatsLimit = _random.Next(SEATS_PER_VENUE_MIN, SEATS_PER_VENUE_MAX + 1);

            var venueResult = Venue.Create(name, prefix, seatsLimit);
            if (venueResult.IsSuccess) venues.Add(venueResult.Value);
        }

        dbContext.Venues.AddRange(venues);
        await dbContext.SaveChangesAsync();

        // Генерируем все места для всех площадок сразу
        var allSeats = new List<Seat>();
        const int seatBatchSize = 5000;

        foreach (var venue in venues)
        {
            var seatsCount = _random.Next(SEATS_PER_VENUE_MIN, Math.Min(venue.SeatsLimit, SEATS_PER_VENUE_MAX) + 1);
            var rows = Math.Max(1, seatsCount / 20);
            var seatsPerRow = Math.Max(1, seatsCount / rows);

            for (var row = 1; row <= rows; row++)
            {
                var seatsInThisRow = row == rows ? seatsCount - ((rows - 1) * seatsPerRow) : seatsPerRow;

                for (var seatNum = 1; seatNum <= seatsInThisRow; seatNum++)
                {
                    var seatResult = Seat.Create(venue.Id, seatNum, row);
                    if (!seatResult.IsSuccess) continue;

                    allSeats.Add(seatResult.Value);

                    if (allSeats.Count < seatBatchSize) continue;

                    dbContext.Seats.AddRange(allSeats);
                    await dbContext.SaveChangesAsync();
                    allSeats.Clear();
                }
            }
        }

        if (allSeats.Count != 0)
        {
            dbContext.Seats.AddRange(allSeats);
            await dbContext.SaveChangesAsync();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        logger.LogInformation($"Seeded {VENUES_COUNT} venues with seats.");
    }

    private async Task SeedEventsBatched()
    {
        logger.LogInformation("Seeding events in batches...");

        const int batchSize = 2000;
        var concertPerformers = new[]
        {
            "Би-2", "Сплин", "Мумий Тролль", "Земфира", "Ленинград", "Каста", "Noize MC", "Баста"
        };
        var conferenceTopics = new[]
        {
            "Технологии будущего", "ИИ в бизнесе", "Экология и устойчивое развитие", "Цифровая трансформация"
        };
        var conferenceSpeakers = new[]
        {
            "Иван Петров", "Мария Сидорова", "Алексей Козлов", "Екатерина Новикова"
        };
        var eventNames = new[]
        {
            "Большой концерт", "Весенний фестиваль", "Техническая конференция", "Онлайн-митап"
        };

        // Получаем ID площадок из базы
        var venueIds = await dbContext.Venues.Select(v => new
        {
            v.Id,
            v.SeatsLimit
        }).ToListAsync();

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        var events = new List<Event>();
        for (var i = 0; i < EVENTS_COUNT; i++)
        {
            var venue = venueIds[_random.Next(venueIds.Count)];
            var eventType = (EventType)_random.Next(0, 2);
            var eventDate = DateTime.UtcNow.AddDays(_random.Next(1, 365));
            var startDate = eventDate.AddHours(_random.Next(-2, 3));
            var endDate = startDate.AddHours(_random.Next(1, 8));
            var capacity = _random.Next(50, venue.SeatsLimit);
            var description = $"Описание события {eventNames[_random.Next(eventNames.Length)]}";
            var name = eventNames[_random.Next(eventNames.Length)];

            var eventResult = eventType switch
            {
                EventType.Concert => Event.Create(
                    EventType.Concert, venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    concertPerformers[_random.Next(concertPerformers.Length)], null, null, null),

                EventType.Conference => Event.Create(
                    EventType.Conference, venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    null,
                    conferenceSpeakers[_random.Next(conferenceSpeakers.Length)],
                    conferenceTopics[_random.Next(conferenceTopics.Length)], null),

                EventType.Online => Event.Create(
                    EventType.Online, venue.Id, name, eventDate, startDate, endDate, capacity, description,
                    null, null, null,
                    $"https://meet.example.com/room{_random.Next(1000, 9999)}"),

                _ => throw new ArgumentOutOfRangeException()
            };

            if (!eventResult.IsSuccess)
                continue;

            events.Add(eventResult.Value);

            if (events.Count < batchSize)
                continue;

            dbContext.Events.AddRange(events);
            await dbContext.SaveChangesAsync();
            events.Clear();
        }

        if (events.Count != 0)
        {
            dbContext.Events.AddRange(events);
            await dbContext.SaveChangesAsync();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        logger.LogInformation("Seeded {EventsCount} events.", EVENTS_COUNT);
    }

    private async Task SeedReservationsBatched()
    {
        logger.LogInformation("Seeding reservations in batches...");

        // Получаем все данные одним запросом
        var eventVenueData = await dbContext.Events.Select(e => new
        {
            e.Id,
            e.VenueId
        }).ToListAsync();
        var userIds = await dbContext.Set<User>().Select(u => u.Id).ToListAsync();
        var seatsByVenue = await dbContext.Seats
            .GroupBy(s => s.VenueId)
            .Select(g => new
            {
                VenueId = g.Key,
                SeatIds = g.Select(s => s.Id.Value).ToList()
            })
            .ToListAsync();

        var venueSeatsDict = seatsByVenue.ToDictionary(x => x.VenueId, x => x.SeatIds);

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        const int batchSize = 1000;
        var reservations = new List<Reservation>();

        foreach (var eventInfo in eventVenueData)
        {
            if (!venueSeatsDict.TryGetValue(eventInfo.VenueId, out var seatIds)) continue;

            var seatsToBook = seatIds.OrderBy(_ => _random.Next()).Take(seatIds.Count / 2).ToArray();

            for (var i = 0; i < seatsToBook.Length;)
            {
                var seatsInReservation = Math.Min(_random.Next(1, 5), seatsToBook.Length - i);
                var reservationSeats = seatsToBook.Skip(i).Take(seatsInReservation).ToList();

                var reservation = Reservation.Create(eventInfo.Id.Value, userIds[_random.Next(userIds.Count)], reservationSeats);
                if (reservation.IsSuccess)
                {
                    reservations.Add(reservation.Value);

                    if (reservations.Count >= batchSize)
                    {
                        dbContext.Reservations.AddRange(reservations);
                        await dbContext.SaveChangesAsync();
                        reservations.Clear();
                    }
                }

                i += seatsInReservation;
            }
        }

        if (reservations.Any())
        {
            dbContext.Reservations.AddRange(reservations);
            await dbContext.SaveChangesAsync();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        logger.LogInformation("Completed seeding reservations.");
    }

    private List<SocialNetwork> GenerateRandomSocials()
    {
        var socialNetworks = new[]
        {
            "VK", "Telegram", "Instagram", "Facebook", "Twitter"
        };
        var count = _random.Next(0, 3);
        var socials = new List<SocialNetwork>();

        for (var i = 0; i < count; i++)
        {
            var network = socialNetworks[_random.Next(socialNetworks.Length)];
            socials.Add(new SocialNetwork
            {
                Name = network,
                Link = $"https://{network.ToLower()}.com/user{_random.Next(1000, 9999)}"
            });
        }

        return socials;
    }
}