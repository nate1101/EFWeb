using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface IExhibitorService
    {
        Task<Exhibitor> GetExhibitorById(int id);
        Task<List<Exhibitor>> GetExhibitorsByEventId(int eventId);
        Task<Exhibitor> SaveExhibitor(Exhibitor exhibitor);
        Task<bool> DeleteExhibitor(int exhibitorId);
        Task<int> GetExhibitorCountByEventId(int eventId);
    }
}
