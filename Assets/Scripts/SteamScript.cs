using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Steamworks;
using System;
using UnityEngine.Networking;
using System.Text;
using Assets.Scripts.Responses;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using Assets.Scripts;

// TODO: RTS camera https://www.youtube.com/watch?v=PsAbHoB85hM

// http://steamworks.github.io/gettingstarted/
public class SteamScript : MonoBehaviour
{
    private const string ArenaCaptureTheFlagBasic = "606873c364da921cb49855f7";
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

    private byte[] m_Ticket;
    private uint m_pcbTicket;
    private HAuthTicket m_HAuthTicket;

    protected Callback<ValidateAuthTicketResponse_t> m_ValidateAuthTicketResponse;
    protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;
    protected Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse;
    protected Callback<GameWebCallback_t> m_GameWebCallback;

    private CallResult<EncryptedAppTicketResponse_t> OnEncryptedAppTicketResponseCallResult;
    private CallResult<StoreAuthURLResponse_t> OnStoreAuthURLResponseCallResult;

    public GameObject creepSpherePrefb = default;

    private AuthLoginResponse loginResponse;
    private ArenaLastGamesResponseGame latestGame;
    private GameResponse gameResponse;

    private ReplayDataProvider replayDataProvider = new ReplayDataProvider();

    // object id and a reference to the game object.
    private Dictionary<string, GameObject> gameState = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }

        StartCoroutine(RenderTicks(10f));
    }

    private IEnumerator RenderTicks(float ticksPerScond)
    {
        var response = replayDataProvider.GetReplayChunk(0);

        // initialize room, gameTime 0
        foreach (var tick in response.Ticks)
        {
            RenderTick(tick);

            yield return new WaitForSecondsRealtime(1 / ticksPerScond);
        }

        var chunk100 = replayDataProvider.GetReplayChunk(100);
        foreach (var tick in chunk100.Ticks)
        {
            RenderTick(tick);

            yield return new WaitForSecondsRealtime(1f / ticksPerScond);
        }


    }

    private void RenderTick(ReplayChunkTick tick)
    {
        var users = tick.users; // TODO: handle more than two users in the future.
                                //var me = tick.users.player1.username == gameResponse.game.users.Single(u => u._id == gameResponse.game.user).username
        var remainingObjects = gameState.Keys.ToList();
        foreach (var roomObject in tick.objects)
        {
            if (remainingObjects.Contains(roomObject._id))
            {
                remainingObjects.Remove(roomObject._id);
            }
            else
            {
                // An object has spawned...
            }

            if (roomObject.type == "creep")
            {
                gameState.TryGetValue(roomObject._id, out var creep);

                if (tick.gameTime == 0)
                {
                    var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;
                    ColorUtility.TryParseHtmlString(ownerColorHex, out var color);

                    creep = Instantiate(creepSpherePrefb);
                    var renderer = creep.GetComponent<Renderer>();
                    renderer.material.SetColor("_BaseColor", color);
                    gameState.Add(roomObject._id, creep);
                }

                creep.transform.position = new Vector3(roomObject.x, 0f, roomObject.y);
            }
        }

        // theese objects where not in this tick. does that mean they died?
        foreach (var id in remainingObjects)
        {
            gameState.TryGetValue(id, out var gameObject);
            if (gameObject)
            {
                Destroy(gameObject);
                gameState.Remove(id);
            }
        }


    }


    // Update is called once per frame
    void Update()
    {
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
            m_NumberOfCurrentPlayers.Set(handle);
            Debug.Log("Called GetNumberOfCurrentPlayers()");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // https://github.com/rlabrecque/Steamworks.NET-Test/blob/master/Assets/Scripts/SteamUserTest.cs
            m_Ticket = new byte[1024];
            m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_Ticket, 1024, out m_pcbTicket);
            print("SteamUser.GetAuthSessionTicket(Ticket, 1024, out pcbTicket) - " + m_HAuthTicket + " -- " + m_pcbTicket);

            /*
             * if (m_HAuthTicket != HAuthTicket.Invalid && m_pcbTicket != 0) {
					EBeginAuthSessionResult ret = SteamUser.BeginAuthSession(m_Ticket, (int)m_pcbTicket, SteamUser.GetSteamID());
					print("SteamUser.BeginAuthSession(m_Ticket, " + (int)m_pcbTicket + ", " + SteamUser.GetSteamID() + ") - " + ret);
				}
				else {
					print("Call GetAuthSessionTicket first!");
				}
             */
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // This is the player logic after we have fetched the entire data structure of a replay, room objects should only represent their current state
        }
    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);

            m_ValidateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.Create(OnValidateAuthTicketResponse);
            m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
            m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
            m_GameWebCallback = Callback<GameWebCallback_t>.Create(OnGameWebCallback);

            OnEncryptedAppTicketResponseCallResult = CallResult<EncryptedAppTicketResponse_t>.Create(OnEncryptedAppTicketResponse);
            OnStoreAuthURLResponseCallResult = CallResult<StoreAuthURLResponse_t>.Create(OnStoreAuthURLResponse);
            //OnMarketEligibilityResponseCallResult = CallResult<MarketEligibilityResponse_t>.Create(OnMarketEligibilityResponse);
            //OnDurationControlCallResult = CallResult<DurationControl_t>.Create(OnDurationControl);
        }
    }
    void OnValidateAuthTicketResponse(ValidateAuthTicketResponse_t pCallback)
    {
        Debug.Log("[" + ValidateAuthTicketResponse_t.k_iCallback + " - ValidateAuthTicketResponse] - " + pCallback.m_SteamID + " -- " + pCallback.m_eAuthSessionResponse + " -- " + pCallback.m_OwnerSteamID);
    }
    void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
    {
        Debug.Log("[" + MicroTxnAuthorizationResponse_t.k_iCallback + " - MicroTxnAuthorizationResponse] - " + pCallback.m_unAppID + " -- " + pCallback.m_ulOrderID + " -- " + pCallback.m_bAuthorized);
    }
    void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
    {
        Debug.Log("[" + GetAuthSessionTicketResponse_t.k_iCallback + " - GetAuthSessionTicketResponse] - " + pCallback.m_hAuthTicket + " -- " + pCallback.m_eResult);
        // Convert to hex/string for use with PlayFab
        string hexEncodedTicket = "";
        for (int i = 0; i < m_Ticket.Length; i++)
        {
            hexEncodedTicket += String.Format("{0:X2}", m_Ticket[i]);
        }
        Debug.Log("[STEAM] hexEncodedTicket == " + hexEncodedTicket); // Ensure it's not empty and looks "hexy"

        //StartCoroutine(ScreepsArenaLogin(hexEncodedTicket));


    }
    //string sessionCookie = null;

    private IEnumerator ScreepsArenaLogin(string ticket)
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
            Debug.Log(response);
            //string s = www.GetResponseHeader("set-cookie");
            //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

            //StartCoroutine(ScreepsArenaArenaList());
            // basic ctf id = 606873c364da921cb49855f7

            loginResponse = JsonConvert.DeserializeObject<AuthLoginResponse>(response);
            // use username for something, persist id as well



            // get last games from arena GET https://arena.screeps.com/api/arena/606873c364da921cb49855f7/last-games
            StartCoroutine(GetLastGames(ArenaCaptureTheFlagBasic));
        }
    }

    private IEnumerator GetLastGames(string arenaId)
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
            Debug.Log(response);
            //string s = www.GetResponseHeader("set-cookie");
            //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

            // pick the latest game
            var latestGames = JsonConvert.DeserializeObject<ArenaLastGamesResponse>(response);
            //var latestGames = JsonUtility.FromJson<ArenaLastGamesResponse>(response);
            latestGame = latestGames.games[0]; // TODO: games is null? why won't that deseralize?

            // GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3 HTTP/1.1
            StartCoroutine(GetGame(latestGame.game._id));

        }
    }

    private IEnumerator GetGame(string gameId)
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
            Debug.Log(response);
            //string s = www.GetResponseHeader("set-cookie");
            //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

            // TODO: GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3 HTTP/1.1
            gameResponse = JsonConvert.DeserializeObject<GameResponse>(response);

            // TODO: we now have a game, and we could start fetching replay data
            StartCoroutine(FetchAllReplayData(gameResponse.game._id));

        }
    }

    private IEnumerator FetchAllReplayData(string gameId)
    {
        Debug.Log("FetchReplayData");


        // TOOD: initialize all room objects
        // TODO: start a loop getting replay chunks, perhaps a "download" ability that buffers them all and then saves to disk or something.
        // TODO: get console `GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/log/100` we can add this later, lets skip it in the start.

        // TODO: a room player initializing all prefabs / roomobjects based on /replay/0
        // TODO: loop each tick and handle their next state compared to their previous, e.g. movement. shooting, healing, so forth
        // TODO: the tickrate should be able to be "controlled", stepping trough each state should be possible.

        // TODO: `GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/replay/0` for the initial tick `var REPlAY_CHUNK_LENGTH = 100;`
        var ticks = gameResponse.game.meta.ticks;
        //var terrain = gameResponse.game.game.terrain;
        // meta.ticks contains amount of ticks keep requesting replay and console untill chunk length does not make sense.
        // game.terrain contains terrain


        // TODO: We now need to iterate all replay data and acquire it. later we might want some sort of stream, so we can fetch it on the go if missing tick chunks, should also seperate rendering from data fetching

        var chunkId = 0;

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
            Debug.Log(response);
            //string s = www.GetResponseHeader("set-cookie");
            //sessionCookie = s.Substring(s.LastIndexOf("sessionID")).Split(';')[0];

            var replayChunkResponse = new ReplayChunkResponse { Ticks = JsonConvert.DeserializeObject<ReplayChunkTick[]>(response) };
            Debug.Log(replayChunkResponse);

            if (chunkId == 0)
            {
                // initialize room, gameTime 0
                foreach (var tick in replayChunkResponse.Ticks)
                {
                    var users = tick.users; // TODO: handle more than two users in the future.
                    //var me = tick.users.player1.username == gameResponse.game.users.Single(u => u._id == gameResponse.game.user).username
                    foreach (var roomObject in tick.objects)
                    {
                        var ownerColorHex = users.player1._id == roomObject.user ? users.player1.color : users.player2.color;

                        ColorUtility.TryParseHtmlString(ownerColorHex, out var color);
                        if (roomObject.type == "creep")
                        {

                            var creep = Instantiate(creepSpherePrefb);
                            creep.transform.position = new Vector3(roomObject.x, 0f, roomObject.y);
                            var renderer = creep.GetComponent<Renderer>();
                            renderer.material.SetColor("_BaseColor", color);
                        }
                    }
                }
            }


        }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?redirectedfrom=MSDN&view=net-5.0
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// 
    public static string Decompress(byte[] bytes)
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

    ////private static string DecompressGZIP(string strData)
    ////{
    ////    var data = Encoding.UTF8.GetBytes(strData);
    ////    var output = new MemoryStream();
    ////    using (var compressedStream = new MemoryStream(data))
    ////    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
    ////    {
    ////        zipStream.CopyTo(output);
    ////        zipStream.Close();
    ////        output.Position = 0;
    ////        return output.ToString();
    ////    }

    ////    throw new Exception("Failed to decompress gzip");
    ////}



    /*
     * function getReplayRangeByTick(tick, ticks) {
        tick = tick <= 0 ? 0 : tick;
        if (tick === 0) {
            // Initial Replay Data;
            return [0, 0];
        }
        if (tick >= ticks) {
            tick = ticks;
        }
        var min = Math.floor((tick - 1) / REPlAY_CHUNK_LENGTH) * 100;
        var max = Math.min(min + 100, ticks);
        return [min + 1, max];
    }
     */

    private IEnumerator ScreepsArenaArenaList()
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


        }
    }

    void OnGameWebCallback(GameWebCallback_t pCallback)
    {
        Debug.Log("[" + GameWebCallback_t.k_iCallback + " - GameWebCallback] - " + pCallback.m_szURL);
    }

    void OnEncryptedAppTicketResponse(EncryptedAppTicketResponse_t pCallback, bool bIOFailure)
    {
        Debug.Log("[" + EncryptedAppTicketResponse_t.k_iCallback + " - EncryptedAppTicketResponse] - " + pCallback.m_eResult);

        // This code is taken directly from SteamworksExample/SpaceWar
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            byte[] rgubTicket = new byte[1024];
            uint cubTicket;
            SteamUser.GetEncryptedAppTicket(rgubTicket, 1024, out cubTicket);

            // normally at this point you transmit the encrypted ticket to the service that knows the decryption key
            // this code is just to demonstrate the ticket cracking library

            // included is the "secret" key for spacewar. normally this is secret
            byte[] rgubKey = new byte[32] { 0xed, 0x93, 0x86, 0x07, 0x36, 0x47, 0xce, 0xa5, 0x8b, 0x77, 0x21, 0x49, 0x0d, 0x59, 0xed, 0x44, 0x57, 0x23, 0xf0, 0xf6, 0x6e, 0x74, 0x14, 0xe1, 0x53, 0x3b, 0xa3, 0x3c, 0xd8, 0x03, 0xbd, 0xbd };

            byte[] rgubDecrypted = new byte[1024];
            uint cubDecrypted = 1024;
            if (!SteamEncryptedAppTicket.BDecryptTicket(rgubTicket, cubTicket, rgubDecrypted, ref cubDecrypted, rgubKey, rgubKey.Length))
            {
                Debug.Log("Ticket failed to decrypt");
                return;
            }

            if (!SteamEncryptedAppTicket.BIsTicketForApp(rgubDecrypted, cubDecrypted, SteamUtils.GetAppID()))
            {
                Debug.Log("Ticket for wrong app id");
            }

            CSteamID steamIDFromTicket;
            SteamEncryptedAppTicket.GetTicketSteamID(rgubDecrypted, cubDecrypted, out steamIDFromTicket);
            if (steamIDFromTicket != SteamUser.GetSteamID())
            {
                Debug.Log("Ticket for wrong user");
            }

            uint cubData;
            byte[] punSecretData = SteamEncryptedAppTicket.GetUserVariableData(rgubDecrypted, cubDecrypted, out cubData);
            if (cubData != sizeof(uint))
            {
                Debug.Log("Secret data size is wrong.");
            }
            Debug.Log(punSecretData.Length);
            Debug.Log(System.BitConverter.ToUInt32(punSecretData, 0));
            if (System.BitConverter.ToUInt32(punSecretData, 0) != 0x5444)
            {
                Debug.Log("Failed to retrieve secret data");
                return;
            }

            Debug.Log("Successfully retrieved Encrypted App Ticket");
        }
    }

    void OnStoreAuthURLResponse(StoreAuthURLResponse_t pCallback, bool bIOFailure)
    {
        Debug.Log("[" + StoreAuthURLResponse_t.k_iCallback + " - StoreAuthURLResponse] - " + pCallback.m_szURL);
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");
            // One popular and recommended use case for the GameOverlayActivated Callback is to pause the game when the overlay opens.
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
        }
        else
        {
            Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers);
        }
    }


}
