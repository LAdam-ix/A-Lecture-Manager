using System.Linq;
using System.Threading.Tasks;
using ALectureManager.Encoder;
using ALectureManager.Library;
using ALectureManager.Main;
using Microsoft.EntityFrameworkCore;
using HanumanInstitute.FFmpeg;



namespace ALectureManager.Models;

public class AppDbContext : DbContext
{
    private string _connectionString;

    public DbSet<EncoderProcessData> EncoderProcessesData { get; set; }
    public DbSet<CodecOption> CodecOption { get; set; }
    private DbSet<SavableProgressStatus> _progress { get; set; }
    public DbSet<LibraryEntry> LibraryEntries { get; set; }
    public AppDbContext(SettingsViewModel settings)
    {
        
        _connectionString = settings.DatabaseConectionString;
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }

    public async Task ReplaceEncodeProcesses(IEnumerable<EncoderProcessData> newEncodeProcessesData)
    {
        var existingIds = await EncoderProcessesData.Select(e => e.Id).ToListAsync();
        EncoderProcessesData.RemoveRange(EncoderProcessesData);

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