using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class ExhibitorService : IExhibitorService
    {
        private readonly IExhibitorRepository _exhibitorRepository;
        public ExhibitorService(
            IExhibitorRepository exhibitorRepository
            )
        {
            _exhibitorRepository = exhibitorRepository;
        }

        public async Task<Exhibitor> GetExhibitorById(int id)
        {
            return await _exhibitorRepository.GetExhibitorById(id);
        }

        public async Task<List<Exhibitor>> GetExhibitorsByEventId(int eventId)
        {
            return await _exhibitorRepository.GetExhibitorsByEventId(eventId);
        }

        public async Task<Exhibitor> SaveExhibitor(Exhibitor exhibitor)
        {
            return await _exhibitorRepository.SaveExhibitor(exhibitor);
        }

        public async Task<bool> DeleteExhibitor(int exhibitorId)
        {
            return await _exhibitorRepository.DeleteExhibitor(exhibitorId);
        }

        public async Task<int> GetExhibitorCountByEventId(int eventId)
        {
            return await _exhibitorRepository.GetExhibitorCountByEventId(eventId);
        }
    }
}
