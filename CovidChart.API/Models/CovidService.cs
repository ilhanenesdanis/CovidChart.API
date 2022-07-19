using CovidChart.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidChart.API.Models
{
    public class CovidService
    {
        private readonly Context _context;
        private readonly IHubContext<CovidHub> _hubContext;
        public CovidService(Context context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public IQueryable<Covid> GetAllList()
        {
            return _context.Covids.AsQueryable();
        }
        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("RevieveCovidList",GetCovidList());
        }
        public List<CovidChart> GetCovidList()
        {
            List<CovidChart> covids = new();

            using (var command=_context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText  = "select tarih,[1],[2],[3],[4],[5] from (select[City],[Count]," +
                    " CAST([CovidDate] as date) as tarih from Covids) " +
                    "as covid PIVOT(SUM(Count) For City IN([1],[2],[3],[4],[5])) " +
                    " as PTable order by tarih asc";
                command.CommandType = System.Data.CommandType.Text;
                _context.Database.OpenConnection();
                using (var reader=command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart c = new CovidChart();
                        c.CovidDate = reader.GetDateTime(0).ToShortDateString();
                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                c.Counts.Add(0);
                            }
                            else
                            {
                                c.Counts.Add(reader.GetInt32(x));
                            }
                        });
                        covids.Add(c);


                    }

                }
            }

            _context.Database.CloseConnection();
            return covids;
        }
    }
}
