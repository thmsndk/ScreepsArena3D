using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Responses
{
    public class GameResponse
    {
        public int ok;
        public GameResponseGame game;
    }

    public class GameResponseGame
    {
        public string _id;
        public GameResponseRatinghistory ratingHistory;
        public GameResponseCode[] codes;
        public GameResponseUser[] users;
        public string user;
        public string arena;
        public GameResponseGame1 game;
        public GameResponseMeta meta;
    }

    public class GameResponseRatinghistory
    {
        public int previousRating;
        public int rating;
        public int previousRank;
        public int rank;
        public bool calibrating;
    }

    public class GameResponseGame1
    {
        public string _id;
        public string status;
        public DateTime createdAt;
        public GameResponseResult result;
        public string terrain;
        public string[] playerColor;
        public string[] usersCode;
    }

    public class GameResponseResult
    {
        public string status;
        public float winner;
    }

    public class GameResponseMeta
    {
        public int ticks;
    }

    public class GameResponseCode
    {
        public string _id;
        public string user;
        public int version;
    }

    public class GameResponseUser
    {
        public string _id;
        public string username;
    }

}
