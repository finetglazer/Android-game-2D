using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Photon.SceneManager.Multi
{
    public class ModeSceneManager: MonoBehaviourPunCallbacks
    {
        public Button leaveButton;
        public Button easyButton;
        public Button mediumButton;
        public Button nightmareButton;

        private bool _isSceneLoading;
        private string _roomType;
        
        private void Start()
        {
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
            easyButton.onClick.AddListener(OnEasyButtonClicked);
            mediumButton.onClick.AddListener(OnMediumButtonClicked);
            nightmareButton.onClick.AddListener(OnNightmareButtonClicked);
           
        }
        
        void OnLeaveButtonClicked()
        {
            PhotonNetwork.LoadLevel("CreateRoomScene");
        }
        
        void OnEasyButtonClicked()
        {
            Create("easy");
            
        }
        
        void OnMediumButtonClicked()
        {
            Create("medium");
        }
        
        void OnNightmareButtonClicked()
        {
            Create("nightmare");
        }

        void Create(string type)
        {
            _roomType = type;
            RoomOptions roomOptions = new RoomOptions()
            {
                MaxPlayers = 4,
                IsVisible = true,
                IsOpen = true
            };
            
            string roomName = Random.Range(1000, 9999).ToString();
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            Debug.Log("Creating a multi mode room...");
            
           
        }
        
        public override void OnCreatedRoom()
        {
            
            Hashtable typeRoom = PhotonNetwork.CurrentRoom.CustomProperties;
            typeRoom["type"] = _roomType;
            PhotonNetwork.CurrentRoom.SetCustomProperties(typeRoom);
            
            base.OnCreatedRoom();
            Debug.Log("Solo mode room created successfully! Loading SoloLobbyScene...");
            if (!_isSceneLoading)
            {
                _isSceneLoading = true;
                PhotonNetwork.LoadLevel("MultiLobbyScene");
            
            }
        }
    }
}