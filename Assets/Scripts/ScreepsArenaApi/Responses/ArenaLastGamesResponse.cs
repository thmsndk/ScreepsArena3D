using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScreepsArenaApi.Responses
{
    public class ArenaLastGamesResponse
    {
        public int ok;
        public ArenaLastGamesResponseGame[] games;
    }

    public class ArenaLastGamesResponseGame
    {
        public string _id;
        public ArenaLastGamesResponseGame1 game;
        public ArenaLastGamesResponseCode[] codes;
        public ArenaLastGamesResponseUser[] users;
        public string user;
        public string arena;
        public ArenaLastGamesResponseRatinghistory ratingHistory;
    }

    public class ArenaLastGamesResponseGame1
    {
        public string _id;
        public string status;
        public DateTime createdAt;
        public ArenaLastGamesResponseResult result;
        public string[] usersCode;
    }

    public class ArenaLastGamesResponseResult
    {
        public string status;
        public float winner;
    }

    public class ArenaLastGamesResponseRatinghistory
    {
        public int previousRating;
        public int rating;
        public int previousRank;
        public int rank;
        public bool calibrating;
    }

    public class ArenaLastGamesResponseCode
    {
        public string _id;
        public string user;
        public int version;
    }

    public class ArenaLastGamesResponseUser
    {
        public string _id;
        public string username;
    }

}
