using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IDirectoryRepository : IRepository<Directory>
    {
        public Task<List<Directory>> GetByParentIdAsync(int id);
    }

    public class DirectoryRepository : Repository<Directory>, IDirectoryRepository
    {
        public DirectoryRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<Directory> GetByIdAsync(int id)
        {
            Directory directory = new Directory() {Id = 0, ParentId = -1};
            if (id != 0)
                directory = await _dbSet.Include(x => x.Tables)
                    .FirstOrDefaultAsync(x => x.Id == id);
            
            Stack<Directory> directories = new Stack<Directory>();
            directories.Push(directory);
            while (directories.Count > 0)
            {
                var d = directories.Pop();
                if (d == null) break;
                d.Childs = await GetByParentIdAsync(d.Id);
                foreach (var child in d.Childs)
                {
                    directories.Push(child);
                }
            }
            
            return directory;
        }

        public async Task<List<Directory>> GetByParentIdAsync(int id)
        {
            return await _dbSet.Where(x => x.ParentId == id)
                .Include(x => x.Tables).ToListAsync();
        }
    }
}