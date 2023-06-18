using DistributedLockApp.Data;
using DistributedLockApp.Data.Model;
using Medallion.Threading.Postgres;

namespace DistributedLockApp
{
    internal class Program
    {
        /*
         * PosgreSQL Distributed Example using Medallion Postgres Package
         * First, please apply the database migration.
         * 
         * run command: dotnet run --project DistrubutedLockApp
         */
        static async Task Main(string[] args)
        {
            var dbContext = new AppDbContext();

            // initialize seed data
            if (dbContext.Users.Any() == false)
            {
                dbContext.Users.Add(new User() { Balance = 50, Name = "Bob" });
                dbContext.Users.Add(new User() { Balance = 100, Name = "Alice" });
                await dbContext.SaveChangesAsync();
            }


        Lock_User:
            await Console.Out.WriteLineAsync("Which user you want to lock if available ?  \nUsers : Bob, Alice");
            var lockUser = Console.ReadLine();

            var user = dbContext.Users.FirstOrDefault(x => x.Name.Equals(lockUser));

            var lockId = $"User_{user.Id}";
            var @lock = new PostgresDistributedLock(new PostgresAdvisoryLockKey(lockId, allowHashing: true), AppDbContext.CONN_STR);

            Console.WriteLine("Press ESC to restart app");
            while ((Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) == false)
            {
                await using (var handleLock = await @lock.TryAcquireAsync())
                {
                    // is locked ?
                    if (handleLock is not null)
                        Console.WriteLine($"Locked :{lockId}");
                    else
                        Console.WriteLine($"Opps... I'm Locked, Do not any change on me ! : {lockId}");

                    await Task.Delay(1000);
                }
            }

            goto Lock_User;
        }
    }
}