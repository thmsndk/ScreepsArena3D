using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using Assets.Scripts;
using Assets.Scripts.ScreepsArenaApi.Responses;

// TODO: RTS camera https://www.youtube.com/watch?v=PsAbHoB85hM

// http://steamworks.github.io/gettingstarted/
public class AuthenticateSteam : MonoBehaviour
{
    
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

    private AuthLoginResponse loginResponse;
    private ArenaLastGamesResponseGame latestGame;
    private GameResponse gameResponse;


    public static string SteamTicket { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log("[Steam] Hello " + name);

            // https://github.com/rlabrecque/Steamworks.NET-Test/blob/master/Assets/Scripts/SteamUserTest.cs
            m_Ticket = new byte[1024];
            m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_Ticket, 1024, out m_pcbTicket);
            //print("SteamUser.GetAuthSessionTicket(Ticket, 1024, out pcbTicket) - " + m_HAuthTicket + " -- " + m_pcbTicket);
        }
    }
    


    // Update is called once per frame
    void Update()
    {
    //    // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
    //        m_NumberOfCurrentPlayers.Set(handle);
    //        Debug.Log("Called GetNumberOfCurrentPlayers()");
    //    }

    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        // https://github.com/rlabrecque/Steamworks.NET-Test/blob/master/Assets/Scripts/SteamUserTest.cs
    //        m_Ticket = new byte[1024];
    //        m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_Ticket, 1024, out m_pcbTicket);
    //        print("SteamUser.GetAuthSessionTicket(Ticket, 1024, out pcbTicket) - " + m_HAuthTicket + " -- " + m_pcbTicket);

    //        /*
    //         * if (m_HAuthTicket != HAuthTicket.Invalid && m_pcbTicket != 0) {
				//	EBeginAuthSessionResult ret = SteamUser.BeginAuthSession(m_Ticket, (int)m_pcbTicket, SteamUser.GetSteamID());
				//	print("SteamUser.BeginAuthSession(m_Ticket, " + (int)m_pcbTicket + ", " + SteamUser.GetSteamID() + ") - " + ret);
				//}
				//else {
				//	print("Call GetAuthSessionTicket first!");
				//}
    //         */
    //    }

    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        // This is the player logic after we have fetched the entire data structure of a replay, room objects should only represent their current state
    //    }
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
        //Debug.Log("[" + GetAuthSessionTicketResponse_t.k_iCallback + " - GetAuthSessionTicketResponse] - " + pCallback.m_hAuthTicket + " -- " + pCallback.m_eResult);
        // Convert to hex/string for use with PlayFab
        string hexEncodedTicket = "";
        for (int i = 0; i < m_Ticket.Length; i++)
        {
            hexEncodedTicket += String.Format("{0:X2}", m_Ticket[i]);
        }
        //Debug.Log("[STEAM] hexEncodedTicket == " + hexEncodedTicket); // Ensure it's not empty and looks "hexy"
        
        SteamTicket = hexEncodedTicket;

        print("[Steam] ticket acquired");
        //StartCoroutine(ScreepsArenaLogin(hexEncodedTicket));


    }
    //string sessionCookie = null;

    



    

    

    

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
