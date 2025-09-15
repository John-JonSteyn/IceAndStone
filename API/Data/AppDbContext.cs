using IceAndStone.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IceAndStone.API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Venue> Venues => Set<Venue>();
		public DbSet<Lane> Lanes => Set<Lane>();
		public DbSet<Session> Sessions => Set<Session>();
		public DbSet<Game> Games => Set<Game>();
		public DbSet<Round> Rounds => Set<Round>();
		public DbSet<Team> Teams => Set<Team>();
		public DbSet<Player> Players => Set<Player>();
		public DbSet<TeamScore> TeamScores => Set<TeamScore>();
		public DbSet<Achievement> Achievements => Set<Achievement>();
		public DbSet<TeamAchievement> TeamAchievements => Set<TeamAchievement>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Lane>()
				.HasIndex(lane => new { lane.VenueId, lane.LaneNumber })
				.IsUnique();

			modelBuilder.Entity<Round>()
				.HasIndex(round => new { round.GameId, round.Number })
				.IsUnique();

			modelBuilder.Entity<Team>()
				.HasIndex(team => new { team.GameId, team.Colour })
				.IsUnique();

			modelBuilder.Entity<TeamScore>()
				.HasIndex(teamScore => new { teamScore.RoundId, teamScore.TeamId })
				.IsUnique();

			modelBuilder.Entity<Game>().HasIndex(game => game.SessionId);
			modelBuilder.Entity<Session>().HasIndex(session => new { session.LaneId, session.StartTime });
			modelBuilder.Entity<Player>().HasIndex(player => player.TeamId);
			modelBuilder.Entity<Team>().HasIndex(team => team.GameId);
			modelBuilder.Entity<Round>().HasIndex(round => round.GameId);

			modelBuilder.Entity<Venue>().Property(venue => venue.Name).IsRequired();
			modelBuilder.Entity<Team>().Property(team => team.Name).IsRequired();
			modelBuilder.Entity<Team>().Property(team => team.Colour).IsRequired();
			modelBuilder.Entity<Player>().Property(player => player.Name).IsRequired();

			modelBuilder.Entity<Lane>()
				.HasOne<Venue>()
				.WithMany()
				.HasForeignKey(lane => lane.VenueId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Venue>().HasData(
				new Venue { Id = 1, Name = "Midgard Curling Yard" },
				new Venue { Id = 2, Name = "Frostfang Arena" }
			);

			modelBuilder.Entity<Lane>().HasData(
				new Lane { Id = 1, VenueId = 1, LaneNumber = 1 },
				new Lane { Id = 2, VenueId = 1, LaneNumber = 2 },
				new Lane { Id = 3, VenueId = 2, LaneNumber = 1 },
				new Lane { Id = 4, VenueId = 2, LaneNumber = 2 }
			);

			modelBuilder.Entity<Session>()
				.HasOne<Lane>()
				.WithMany()
				.HasForeignKey(session => session.LaneId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Game>()
				.HasOne<Session>()
				.WithMany()
				.HasForeignKey(game => game.SessionId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Round>()
				.HasOne<Game>()
				.WithMany()
				.HasForeignKey(round => round.GameId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Team>()
				.HasOne<Game>()
				.WithMany()
				.HasForeignKey(team => team.GameId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Player>()
				.HasOne<Team>()
				.WithMany()
				.HasForeignKey(player => player.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamScore>()
				.HasOne<Round>()
				.WithMany()
				.HasForeignKey(teamScore => teamScore.RoundId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamScore>()
				.HasOne<Team>()
				.WithMany()
				.HasForeignKey(teamScore => teamScore.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamAchievement>()
				.HasOne<Achievement>()
				.WithMany()
				.HasForeignKey(teamAchievement => teamAchievement.AchievementId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamAchievement>()
				.HasOne<Team>()
				.WithMany()
				.HasForeignKey(teamAchievement => teamAchievement.TeamId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TeamAchievement>()
				.HasOne<Game>()
				.WithMany()
				.HasForeignKey(teamAchievement => teamAchievement.GameId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Achievement>().HasData(
				new Achievement
				{
					Id = 1,
					Name = "Icebreaker",
					TriggerType = "round",
					Description = "Team scores first points in a game"
				},
				new Achievement
				{
					Id = 2,
					Name = "Mighty Mjolnir",
					TriggerType = "round",
					Description = "Team scores maximum points in a round"
				},
				new Achievement
				{
					Id = 3,
					Name = "Lucky Loki",
					TriggerType = "round",
					Description = "Team scores max points in 3 consecutive rounds"
				},
				new Achievement
				{
					Id = 4,
					Name = "Looking for Leif",
					TriggerType = "session",
					Description = "Team completes 5+ games in a session"
				},
				new Achievement {
					Id = 5,
					Name = "Niflheim’s Touch",
					TriggerType = "session",
					Description = "Team scores zero in a game"
				},
				new Achievement
				{
					Id = 6,
					Name = "Odin’s Offspring",
					TriggerType = "session",
					Description = "Win all games in a session with at least 5 games"
				},
				new Achievement
				{
					Id = 7,
					Name = "Appointed Housecarl",
					TriggerType = "session",
					Description = "Win 1 game"
				},
				new Achievement
				{
					Id = 8,
					Name = "Dubbed Thane",
					TriggerType = "session",
					Description = "Win 2 consecutive games"
				},
				new Achievement
				{
					Id = 9,
					Name = "Promoted to Jarl",
					TriggerType = "session",
					Description = "Win 3 consecutive games"
				},
				new Achievement
				{
					Id = 10,
					Name = "Coronated King",
					TriggerType = "session",
					Description = "Win 5 consecutive games"
				}
			);
		}
	}
}
