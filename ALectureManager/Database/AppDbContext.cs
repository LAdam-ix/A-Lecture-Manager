
using System.Threading.Tasks;
using ALectureManager.Encoder;
using ALectureManager.Library;
using ALectureManager.Main;
using Microsoft.EntityFrameworkCore;


namespace ALectureManager.Models;

public class AppDbContext : DbContext
{
    private string _connectionString;

    public DbSet<EncoderProcessData> EncoderProcessesData { get; set; }
    public DbSet<CodecOption> CodecOption { get; set; }
    public DbSet<LibraryEntry> LibraryEntries { get; set; }

    public AppDbContext(SettingsViewModel settings)
    {
        _connectionString = settings.DatabaseConectionString;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }

    public async Task ReplaceEncodeProcesses(IEnumerable<EncoderProcessData> newEncodeProcessesData)
    {
        var existingIds = await EncoderProcessesData.Select(e => e.Id).ToListAsync();
        // this probably isnt the best but it was the only way to stop this from crashing.
        // maybe other way would be not storing the encode inside the json setting but 
        // only in this db and load it there but im not sure if that would work
        // and would take too much time to try.
        EncoderProcessesData.RemoveRange(EncoderProcessesData);
        CodecOption.RemoveRange(CodecOption);
        foreach (var newData in newEncodeProcessesData)
        {
            if (existingIds.Contains(newData.Id))
            {
                Entry(newData).State = EntityState.Modified;
            }
            else
            {
                EncoderProcessesData.Add(newData);
            }
        }

        await SaveChangesAsync();
    }
}