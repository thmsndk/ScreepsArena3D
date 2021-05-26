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

        //// TODO: get response
        //private IEnumerator GetArenaList(Action<ArenaLastGamesResponse> callback)
        //{
        //    Debug.Log("ScreepsArenaArenaList");
        //    var www = UnityWebRequest.Get("https://arena.screeps.com/api/arena/list");

        //    yield return www.SendWebRequest();

        //    if (www.isNetworkError || www.isHttpError)
        //    {
        //        Debug.Log(www.error);
        //    }
        //    else
        //    {
        //        // https://forum.unity.com/threads/json-from-webrequest.384974/
        //        string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
        //        Debug.Log(response);
        //        //string s = www.GetResponseHeader("set-cookie");
        //        //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];
        //        callback(JsonConvert.DeserializeObject<ArenaLastGamesResponse>(response));

        //    }
        //}

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
