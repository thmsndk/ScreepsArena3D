using Assets.Scripts.ScreepsArenaApi.Responses;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.ScreepsArenaApi
{
    public class Http
    {
        /// <summary>
        /// This method needs to be called before you can call any other method, a cookie gets generated that is supplied in subsequent requests, as such, we need the ticket.
        /// </summary>
        /// <param name="ticket">A ticket from steam authentication</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator ScreepsArenaLogin(string ticket, Action<AuthLoginResponse> callback = null)
        {
            Debug.Log("ScreepsArenaLogin");
            Debug.Log(ticket);
            var json = "{\"ticket\":\"" + ticket + "\"}";
            Debug.Log(json);

            var url = "https://arena.screeps.com/api/auth/login";
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // https://forum.unity.com/threads/json-from-webrequest.384974/
                string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                //PlayerData player = JsonConvert.DeserializeObject<PlayerData>(response);
                //Debug.Log(response);
                //string s = www.GetResponseHeader("set-cookie");
                //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

                //StartCoroutine(ScreepsArenaArenaList());
                // basic ctf id = 606873c364da921cb49855f7

                if (callback != null)
                {
                    callback(JsonConvert.DeserializeObject<AuthLoginResponse>(response));
                }
                // use username for something, persist id as well



                //// get last games from arena GET https://arena.screeps.com/api/arena/606873c364da921cb49855f7/last-games
                //StartCoroutine(GetLastGames(ArenaCaptureTheFlagBasic));
            }
        }

        public IEnumerator GetArenaList(Action<ArenaListResponse> callback)
        {
            Debug.Log("ScreepsArenaArenaList");
            var www = UnityWebRequest.Get("https://arena.screeps.com/api/arena/list");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // https://forum.unity.com/threads/json-from-webrequest.384974/
                string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                Debug.Log(response);

                //string s = www.GetResponseHeader("set-cookie");
                //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];
                callback(JsonConvert.DeserializeObject<ArenaListResponse>(response));

            }
        }

        public IEnumerator GetLastGames(string arenaId, Action<ArenaLastGamesResponse> callback)
        {
            Debug.Log("GetLastGames");
            var www = UnityWebRequest.Get($"https://arena.screeps.com/api/arena/{arenaId}/last-games");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // https://forum.unity.com/threads/json-from-webrequest.384974/
                string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                //Debug.Log(response);
                //string s = www.GetResponseHeader("set-cookie");
                //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

                // pick the latest game
                callback(JsonConvert.DeserializeObject<ArenaLastGamesResponse>(response));
                ////var latestGames = JsonUtility.FromJson<ArenaLastGamesResponse>(response);
                //latestGame = latestGames.games[0]; // TODO: games is null? why won't that deseralize?

                //// GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3 HTTP/1.1
                //StartCoroutine(GetGame(latestGame.game._id));

            }
        }

        public IEnumerator GetGame(string gameId, Action<GameResponse, string> callback)
        {
            Debug.Log("GetGame");
            var www = UnityWebRequest.Get($"https://arena.screeps.com/api/game/{gameId}");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // https://forum.unity.com/threads/json-from-webrequest.384974/
                string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                //Debug.Log(response);
                //string s = www.GetResponseHeader("set-cookie");
                //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

                // TODO: GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3 HTTP/1.1
                callback(JsonConvert.DeserializeObject<GameResponse>(response), response);

                //// TODO: we now have a game, and we could start fetching replay data
                ////StartCoroutine(FetchAllReplayData(gameResponse.game._id));

            }
        }

        public IEnumerator GetReplayChunk(string gameId, int chunkId, Action<ReplayChunkResponse, string> callback)
        {
            Debug.Log($"GetReplayChunk {gameId} {chunkId}");


            // TOOD: initialize all room objects
            // TODO: start a loop getting replay chunks, perhaps a "download" ability that buffers them all and then saves to disk or something.
            // TODO: get console `GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/log/100` we can add this later, lets skip it in the start.

            // TODO: a room player initializing all prefabs / roomobjects based on /replay/0
            // TODO: loop each tick and handle their next state compared to their previous, e.g. movement. shooting, healing, so forth
            // TODO: the tickrate should be able to be "controlled", stepping trough each state should be possible.

            // TODO: `GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/replay/0` for the initial tick `var REPlAY_CHUNK_LENGTH = 100;`
            //var ticks = gameResponse.game.meta.ticks;
            //var terrain = gameResponse.game.game.terrain;
            // meta.ticks contains amount of ticks keep requesting replay and console untill chunk length does not make sense.
            // game.terrain contains terrain


            // TODO: We now need to iterate all replay data and acquire it. later we might want some sort of stream, so we can fetch it on the go if missing tick chunks, should also seperate rendering from data fetching

            /*
             * https://community.playfab.com/questions/51765/libcurl-bug-in-unity-202113f1.html?page=1&pageSize=10&sort=oldest
             * The issue seems to be that from Unity 2021.1.3f1, 
             * UnityWebRequest no longer supports data compression. 
             * The error "Curl error 61: Unrecognized content encoding type. libcurl understands identity content encodings."
             */

            var www = UnityWebRequest.Get($"https://arena.screeps.com/api/game/{gameId}/replay/{chunkId}");
            //www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("Accept-Encoding", "identity");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //var response = DecompressGZIP(www.downloadHandler.data);

                // https://forum.unity.com/threads/json-from-webrequest.384974/
                //string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                var response = Decompress(www.downloadHandler.data);
                //Debug.Log(response);
                //string s = www.GetResponseHeader("set-cookie");
                //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

                callback(new ReplayChunkResponse { Ticks = JsonConvert.DeserializeObject<ReplayChunkTick[]>(response) }, response);
            }
        }

        /*
         TODO:
        GET /api/arena/606873c364da921cb49855f7
        response: {"ok":1,"arena":{"_id":"606873c364da921cb49855f7","name":"Capture the Flag","advanced":false,"rating":821,"rank":40,"games":627,"qualifying":false}}

        I assume this is called to get the currently running game.
        GET /api/arena/606873c364da921cb49855f7/current-game
        response: {"ok":1,"game":null}

        GET /api/arena/606873c364da921cb49855f7/rating-history?offset=0&limit=10
        response: {"ok":1,"history":[{"_id":"60a75ad6917e743d80a4c1a0","game":{"_id":"60a75ad659d8f98b59b4e24b","status":"finished","createdAt":"2021-05-21T07:01:42.755Z","result":{"status":"ok","winner":0},"usersCode":["60a75ad659d8f99866b4e24a","6089ab6fc235cb0cf77036a2"]},"codes":[{"_id":"6089ab6fc235cb0cf77036a2","user":"607c7ca7df54262f11077103","version":5},{"_id":"60a75ad659d8f99866b4e24a","user":"609419addf54262f11f5d844","version":31}],"users":[{"_id":"607c7ca7df54262f11077103","username":"artch"},{"_id":"609419addf54262f11f5d844","username":"thmsn"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":829,"rating":821,"previousRank":38,"rank":38,"calibrating":false}},{"_id":"60a2eb85d3a03242e223d783","game":{"_id":"60a2eb85b0e4af3a24e4b23c","status":"finished","createdAt":"2021-05-17T22:17:41.446Z","result":{"status":"ok","winner":0},"usersCode":["60a2eb5bb0e4af3b49e4b234","609c617f45774a8466fed114"]},"codes":[{"_id":"609c617f45774a8466fed114","user":"6094c487df54262f11758bf6","version":14},{"_id":"60a2eb5bb0e4af3b49e4b234","user":"609419addf54262f11f5d844","version":30}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"},{"_id":"6094c487df54262f11758bf6","username":"Dean"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":837,"rating":829,"previousRank":35,"rank":35,"calibrating":false}},{"_id":"609c6a2857cb1515f2957b32","game":{"_id":"609c6a2845774a275dfed186","status":"finished","createdAt":"2021-05-12T23:52:08.404Z","result":{"status":"ok","winner":0},"usersCode":["609c414345774acd1cfecfa0","609ab68f7224f74f8a4754d4"]},"codes":[{"_id":"609ab68f7224f74f8a4754d4","user":"6096ceb8df54262f11ce4b07","version":31},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"},{"_id":"6096ceb8df54262f11ce4b07","username":"tiggus"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":845,"rating":837,"previousRank":24,"rank":25,"calibrating":false}},{"_id":"609c6a14af8d51a6b871d8eb","game":{"_id":"609c6a1445774a6f58fed185","status":"finished","createdAt":"2021-05-12T23:51:48.260Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","6085b75129bb9429fcfeed98"]},"codes":[{"_id":"6085b75129bb9429fcfeed98","user":"607c7ca7df54262f11077103"},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"607c7ca7df54262f11077103","username":"artch"},{"_id":"609419addf54262f11f5d844","username":"thmsn"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":845,"rating":845,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c6a0739e640036b83380f","game":{"_id":"609c6a0645774afe1ffed184","status":"finished","createdAt":"2021-05-12T23:51:34.869Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","609458cc444a8b4a90359dbe"]},"codes":[{"_id":"609458cc444a8b4a90359dbe","user":"60942925df54262f1119c798","version":6},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"},{"_id":"60942925df54262f1119c798","username":"qnz"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":845,"rating":845,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c69fb6957c45e3b31eff7","game":{"_id":"609c69fb45774acceffed183","status":"finished","createdAt":"2021-05-12T23:51:23.442Z","result":{"status":"ok","winner":0},"usersCode":["609c414345774acd1cfecfa0","609458cc444a8b4a90359dbe"]},"codes":[{"_id":"609458cc444a8b4a90359dbe","user":"60942925df54262f1119c798","version":6},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"},{"_id":"60942925df54262f1119c798","username":"qnz"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":853,"rating":845,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c69ee895eac471983a889","game":{"_id":"609c69ed45774a255ffed182","status":"finished","createdAt":"2021-05-12T23:51:09.767Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","609b65967224f73d80475dc5"]},"codes":[{"_id":"609b65967224f73d80475dc5","user":"609b4af1df54262f118b9843","version":9},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"},{"_id":"609b4af1df54262f118b9843","username":"Frop"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":853,"rating":853,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c69e19418682ef05d1fdb","game":{"_id":"609c69e045774a9889fed181","status":"finished","createdAt":"2021-05-12T23:50:56.799Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","609485a0444a8b341035a010"]},"codes":[{"_id":"609485a0444a8b341035a010","user":"609419addf54262f11f5d844","version":12},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":853,"rating":853,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c69d37fc93f3180536e28","game":{"_id":"609c69d345774a4ac2fed180","status":"finished","createdAt":"2021-05-12T23:50:43.200Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","6089ab6fc235cb0cf77036a2"]},"codes":[{"_id":"6089ab6fc235cb0cf77036a2","user":"607c7ca7df54262f11077103","version":5},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"607c7ca7df54262f11077103","username":"artch"},{"_id":"609419addf54262f11f5d844","username":"thmsn"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":853,"rating":853,"previousRank":24,"rank":24,"calibrating":false}},{"_id":"609c69c86957c4418431efcd","game":{"_id":"609c69c745774a0b44fed17f","status":"finished","createdAt":"2021-05-12T23:50:31.820Z","result":{"status":"ok","winner":0.5},"usersCode":["609c414345774acd1cfecfa0","609485a0444a8b341035a010"]},"codes":[{"_id":"609485a0444a8b341035a010","user":"609419addf54262f11f5d844","version":12},{"_id":"609c414345774acd1cfecfa0","user":"609419addf54262f11f5d844","version":29}],"users":[{"_id":"609419addf54262f11f5d844","username":"thmsn"}],"user":"609419addf54262f11f5d844","arena":"606873c364da921cb49855f7","ratingHistory":{"previousRating":853,"rating":853,"previousRank":24,"rank":24,"calibrating":false}}],"meta":{"length":628}}

        this is called when you enter the rating history
        GET /api/game/60a75ad659d8f98b59b4e24b

        this is called when you enter "rank"
        GET /api/arena/606873c364da921cb49855f7/leaderboard?offset=0&limit=50
        response: {"ok":1,"page":[{"_id":"60941f34df54262f111619cb","username":"Tigga","rank":1,"rating":2690,"games":804,"maxVersion":120},{"_id":"60943277df54262f11fba63f","username":"slowmotionghost","rank":2,"rating":2423,"games":845,"maxVersion":64},{"_id":"60941820df54262f11a652ce","username":"Q13214","rank":3,"rating":2252,"games":457,"maxVersion":98},{"_id":"609ae91fdf54262f118aff0f","username":"Ceggindeggar","rank":4,"rating":2244,"games":1083,"maxVersion":243},{"_id":"60948f65df54262f11c8e927","username":"christaylor801","rank":5,"rating":2230,"games":589,"maxVersion":28},{"_id":"60941a11df54262f11092626","username":"Geir1983","rank":6,"rating":2221,"games":522,"maxVersion":69},{"_id":"6094176ddf54262f11819efe","username":"Qzar","rank":7,"rating":2098,"games":508,"maxVersion":24},{"_id":"6094362ddf54262f11ba53bd","username":"MaikeruKonare","rank":8,"rating":2007,"games":682,"maxVersion":150},{"_id":"6095c550df54262f11539342","username":"admon84","rank":9,"rating":1938,"games":8012,"maxVersion":27},{"_id":"609ecddadf54262f119b10e1","username":"Green","rank":10,"rating":1880,"games":892,"maxVersion":55},{"_id":"6094c487df54262f11758bf6","username":"Dean","rank":11,"rating":1804,"games":392,"maxVersion":89},{"_id":"60958debdf54262f118282da","username":"Robalian","rank":12,"rating":1707,"games":456,"maxVersion":42},{"_id":"609437c0df54262f110d0c3b","username":"thinic","rank":13,"rating":1640,"games":479,"maxVersion":74},{"_id":"6096ceb8df54262f11ce4b07","username":"tiggus","rank":14,"rating":1639,"games":741,"maxVersion":83},{"_id":"60a208f7df54262f110ec23a","username":"BrainWart","rank":15,"rating":1581,"games":5,"maxVersion":2},{"_id":"609f4edfdf54262f110a996a","username":"Random 5th","rank":16,"rating":1566,"games":5,"maxVersion":3},{"_id":"60a034f0df54262f11154d6a","username":"MrGonzo","rank":17,"rating":1512,"games":10,"maxVersion":3},{"_id":"60a13f3cdf54262f118020fa","username":"Shadow","rank":18,"rating":1508,"games":214,"maxVersion":19},{"_id":"6099b2e5df54262f110739e4","username":"RGBKnights","rank":19,"rating":1506,"games":382,"maxVersion":30},{"_id":"609ee335df54262f11f7c300","username":"Silten","rank":20,"rating":1465,"games":83,"maxVersion":22},{"_id":"609a2384df54262f11c2db74","username":"Murphyslaw","rank":21,"rating":1443,"games":589,"maxVersion":69},{"_id":"60ad8db4df54262f116f01ed","username":"Dignissi","rank":22,"rating":1387,"games":185,"maxVersion":12},{"_id":"60840bfedf54262f11152f1d","username":"pyosik","rank":23,"rating":1364,"games":252,"maxVersion":7},{"_id":"60942925df54262f1119c798","username":"qnz","rank":24,"rating":1353,"games":45,"maxVersion":9},{"_id":"60a531f1df54262f111766d3","username":"nsatter","rank":25,"rating":1324,"games":103,"maxVersion":38},{"_id":"609ac26ddf54262f11f4ab36","username":"friiky2","rank":26,"rating":1300,"games":205,"maxVersion":31},{"_id":"60997f43df54262f113015dc","username":"TehFiend","rank":27,"rating":1232,"games":149,"maxVersion":18},{"_id":"609417ccdf54262f11950ef6","username":"ags131","rank":28,"rating":1202,"games":107,"maxVersion":56},{"_id":"609ac0f2df54262f11aaa73f","username":"dFour","rank":29,"rating":1198,"games":179,"maxVersion":8},{"_id":"609993c3df54262f11179ae3","username":"Poc Boy","rank":30,"rating":1157,"games":5,"maxVersion":1},{"_id":"607c7ca7df54262f11077103","username":"artch","rank":31,"rating":1153,"games":206,"maxVersion":22},{"_id":"60a28f4adf54262f11eb7c26","username":"Disconnect","rank":32,"rating":1142,"games":17,"maxVersion":10},{"_id":"6098864bdf54262f1196e68c","username":"mike.spille","rank":33,"rating":1092,"games":220,"maxVersion":41},{"_id":"60943ecadf54262f117af03a","username":"Dizzy","rank":34,"rating":1049,"games":37,"maxVersion":20},{"_id":"60971167df54262f113d9a79","username":"Jacudibu","rank":35,"rating":1031,"games":6,"maxVersion":4},{"_id":"60abaa46df54262f1115bb3b","username":"Ureium","rank":36,"rating":1003,"games":5,"maxVersion":1},{"_id":"6099d578df54262f11a0c3bb","username":"disor","rank":37,"rating":896,"games":58,"maxVersion":15},{"_id":"609a8490df54262f11d19936","username":"RaymondKevin","rank":38,"rating":867,"games":7,"maxVersion":7},{"_id":"6099a77edf54262f11d9165d","username":"MarkDey","rank":39,"rating":839,"games":111,"maxVersion":14},{"_id":"609419addf54262f11f5d844","username":"thmsn","rank":40,"rating":821,"games":627,"maxVersion":31},{"_id":"60a1631cdf54262f117df30c","username":"kostronor","rank":41,"rating":812,"games":21,"maxVersion":9},{"_id":"609b78eadf54262f117003af","username":"Justitia","rank":42,"rating":811,"games":6,"maxVersion":2},{"_id":"6099ae7fdf54262f1130eac1","username":"ConRonJohnson","rank":43,"rating":808,"games":228,"maxVersion":61},{"_id":"609df5ffdf54262f116556b3","username":"deft_code","rank":44,"rating":787,"games":12,"maxVersion":6},{"_id":"609a6bb8df54262f11f2bb64","username":"ssandboxx","rank":45,"rating":760,"games":83,"maxVersion":25},{"_id":"609be700df54262f11b9df7c","username":"THC R4wizard","rank":46,"rating":750,"games":5,"maxVersion":2},{"_id":"607c7022df54262f116e083b","username":"dmitriyff","rank":47,"rating":735,"games":17,"maxVersion":4},{"_id":"609b4af1df54262f118b9843","username":"Frop","rank":48,"rating":717,"games":22,"maxVersion":19},{"_id":"609c65addf54262f11b25d26","username":"Kokuyo","rank":49,"rating":707,"games":239,"maxVersion":83},{"_id":"60acfeaddf54262f11f2d68e","username":"RexTheCapt","rank":50,"rating":689,"games":109,"maxVersion":13}],"meta":{"length":69}}
         */

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?redirectedfrom=MSDN&view=net-5.0
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        private static string Decompress(byte[] bytes)
        {
            // TODO: rework this.
            using (Stream memoryStream = new MemoryStream(bytes))
            {
                using (Stream decompressedMemoryStream = new MemoryStream())
                {
                    using (GZipStream decompressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedMemoryStream);
                        decompressedMemoryStream.Position = 0;

                        using (var sr = new StreamReader(decompressedMemoryStream, Encoding.UTF8))
                        {
                            var result = sr.ReadToEnd(); // TODO: async
                            return result;
                        }
                    }
                }
            }
        }
    }
}
