// Venue: Physical location
Table Venue {
  id          long     [pk]
  Name        string   [not null]
}

// Lane: A specific lane at a venue
Table Lane {
  id          long     [pk]
  VenueId     long     [ref: > Venue.id]
  LaneNumber  int      [not null, note: "Unique per venue"]
}

// Session: A series of games on a lane
Table Session {
  id          long     [pk]
  LaneId      long     [ref: > Lane.id]
  StartTime   datetime [not null]
  EndTime     datetime
}

// Game: A single game within a session
Table Game {
  id           long     [pk]
  SessionId    long     [ref: > Session.id]
  StartTime    datetime [not null]
  EndTime      datetime
  TargetRounds int      [note: "Optional: e.g., 4–8 for PoC"]
}

// Round: One curling round in a game (Round 1, Round 2, ...)
Table Round {
  id                 long [pk]
  GameId             long [ref: > Game.id]
  Number             int  [not null, note: "1-based per game"]
  StartsFirstTeamId  long [ref: > Team.id, note: "Team that throws first this round"]
  StartTime          datetime
  EndTime            datetime
}

// Team: Represents one of two teams in a game
Table Team {
  id            long     [pk]
  GameId        long     [ref: > Game.id]
  Name          string   [not null]
  Colour        string   [not null, note: "One of 6 preset colours"]
  HasFirstRound bool     [not null, note: "True if this team starts Round 1"]
}

// Player: Ephemeral roster entry scoped to a single game (no identity beyond this game)
Table Player {
  id        long   [pk]
  TeamId    long   [ref: > Team.id]
  Name      string [not null]
}

// TeamScore: the teams score for a specific round
Table TeamScore {
  id        long [pk]
  RoundId   long [ref: > Round.id]
  TeamId    long [ref: > Team.id]
  Value     int  [not null, note: "Points this team earned this round"]
}

// Achievement: Defines an achievement in the game/session
Table Achievement {
  id          long   [pk]
  Name        string [not null]
  TriggerType string [not null, note: "round | session"]
  Description string
}

// TeamAchievement: Tracks achievements earned by a team (session derivable via Game)
Table TeamAchievement {
  id            long     [pk]
  AchievementId long     [ref: > Achievement.id]
  TeamId        long     [ref: > Team.id]
  GameId        long     [ref: > Game.id]
  AchievedAt    datetime [not null]
}
