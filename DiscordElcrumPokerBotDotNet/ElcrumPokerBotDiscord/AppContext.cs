using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using ElcrumPokerBotDiscord.Models;
using Microsoft.EntityFrameworkCore;

namespace ElcrumPokerBotDiscord
{
    public class AppContext : DbContext
    {

        public DbSet<Participant> Users => Set<Participant>();
        public AppContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=helloapp.db");
        }

    }
}
