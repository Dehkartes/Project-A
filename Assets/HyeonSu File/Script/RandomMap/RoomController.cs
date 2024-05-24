using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : Singleton<RoomController>
{
	public string globalRoomTitle = "Basement";

	public RoomInfo currentLoadRoomData;
	public Room currRoom;

	public List<Room> loadedRooms = new List<Room>();

	public Material DefaultBackground;
	public Material VisitedBack;
	public Material currMaterial;

	private GameObject Player;
	public bool isLoadingRoom = false;

    private void Start()
    {
		Player = GameObject.FindWithTag("Player");

		Player.transform.position = new Vector3(100, -30, 0);
    }

    public void CreatedRoom()
	{
		isLoadingRoom = false;

		for(int i=0; i < transform.childCount; i++)
			Destroy(transform.GetChild(i).gameObject);
		 
		loadedRooms.Clear();
		
		DungeonCrawlerController.Instance.CreatedRoom();
		SetRoomPath();
	}

	void SetRoomPath()
	{
		if (isLoadingRoom)
			return;

		if (loadedRooms.Count > 0)
		{
			foreach (Room room in loadedRooms)
			{
				room.RemoveUnconnectedWalls();
			}
			isLoadingRoom = true;
		}
	}

	public void LoadRoom(RoomInfo settingRoom)
	{
		if (DoesRoomExist(settingRoom.center_Position.x, settingRoom.center_Position.y, settingRoom.center_Position.z))
		{
			return;
		}

		string roomPreName = "";

		if (settingRoom.roomName == "Single")
		{
			switch (Random.Range(0, 4))
			{
				case 0:
					roomPreName = "Single";
					break;
				case 1:
					roomPreName = "Single1";
					break;
				case 2:
					roomPreName = "Single2";
					break;
				case 3:
					roomPreName = "Single3";
					break;
			}
		}
		else
		{
			roomPreName = settingRoom.roomName;
		}

		if (settingRoom.roomName == "Boss" && DungeonCrawlerController.Instance.bossroomname != null)
		{
			roomPreName = DungeonCrawlerController.Instance.bossroomname;
		}

		//유니티 방 객체 생성
		//RoomPrefabsSet: 인게임 GameManager 객체로부터 사전에 방정보를 등록하는데 사용된 스크립트 roomPrefabs 딕셔너리에 키: 프리팹 네임, 벨류: 해당되는 맵 프리팹이 있음
		//				  (Prefabs\BossMap\BossRoom.prefab, Prefabs\NormalMap\NormalRoom.prefab)
		if (roomPreName == null || roomPreName == "")
		{
			return;
		}
		GameObject room = Instantiate(RoomPrefabsSet.Instance.roomPrefabs[roomPreName]);
		//방의 물리적 위치를 설정, 단위 방의 크기가 10*10 일 때, 방의 상대 좌표(posArr상의 좌표)가 5,5라면 절대 좌표(유니티 상의 좌표)는 50,50
		room.transform.position = new Vector3(
					settingRoom.center_Position.x * room.transform.GetComponent<Room>().Width,
					settingRoom.center_Position.y * room.transform.GetComponent<Room>().Height,
					settingRoom.center_Position.z
		);

		room.transform.localScale = new Vector3(
					room.transform.GetComponent<Room>().Width/10,
					room.transform.GetComponent<Room>().Height / 10,
					1
		);

		//settingRoom에 있는 방 정보를 가져와서 유니티 객체인 Room에 적용

		room.transform.GetComponent<Room>().center_Position = settingRoom.center_Position;
		room.name = globalRoomTitle + "-" + settingRoom.roomName + " " + settingRoom.center_Position.x + ", " + settingRoom.center_Position.y;

		room.transform.GetComponent<Room>().roomName                = settingRoom.roomName;
		room.transform.GetComponent<Room>().roomType                = settingRoom.roomType;
		room.transform.GetComponent<Room>().roomId                  = settingRoom.roomID;
		room.transform.GetComponent<Room>().parent_Position         = settingRoom.parent_Position;
		room.transform.GetComponent<Room>().mergeCenter_Position    = settingRoom.mergeCenter_Position;
		room.transform.GetComponent<Room>().distance                = settingRoom.distance;

		room.transform.parent = transform;

		loadedRooms.Add(room.GetComponent<Room>());
	}
		
	// 빈 데이터 혹은 삭제된 방이 있을 경우를 위한 예외처리
	public bool DoesRoomExist(int x, int y, int z)
	{
		return loadedRooms.Find(item => item.center_Position.x == x && item.center_Position.y == y && item.center_Position.z == z) != null;
	}

	//    
	public Room FindRoom(int x, int y, int z)
	{
		// List.Find : item 변수 조건에 맞는 Room을 찾아 반환
		return loadedRooms.Find(item => item.center_Position.x == x && item.center_Position.y == y && item.center_Position.z == z);
	}

	// 해당 Room에서 Player가 있는 방을 반환
	public void OnPlayerEnterRoom(Room room) 
	{
		CameraFollow.Instance.currRoom = room;
		
		currRoom = room;

		for (int i = 0; i < loadedRooms.Count; i++)
		{
			if (room.parent_Position == loadedRooms[i].parent_Position)
				loadedRooms[i].childRooms.minimapUpdate();
		}
	}

}
