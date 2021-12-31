using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelBuilder : MonoBehaviour
{
	// Public variables for the generator
	public Room startRoomPrefab, endRoomPrefab;
	public List<Room> roomPrefabs = new List<Room> ();
	public List<Room> secretPrefabs = new List<Room> ();
	public List<Room> roundingPrefabs = new List<Room> ();
	public Vector2 iterationRange = new Vector2 (3, 5);
	public int secretDivisor = 10;
	public int memoryMultiplier = 1;
	public int memoryFactor = 1;
	public int difficulty;

	// Non public variables for auxillary storage and reference
	StartRoom startRoom;
	EndRoom endRoom;
	LayerMask roomLayerMask;
	List<List<Room>> memoryMatrix = new List<List<Room>> ();
	List<Doorway> availableDoorways = new List<Doorway> ();
	List<Room> placedRooms = new List<Room> ();
	List<Room> placedSecrets = new List<Room> ();
	List<Room> lastPlacedSecrets = new List<Room> ();

	// Onvalidate method for makeshift boolean buttons for resetting the generator during runtime
	public bool resetBool = false;
	public bool hardResetBool = false;

	public NavMeshSurface surface;
	public GameObject playerPrefab;
	public HealthBar healthBar;
	public GameObject enemyPrefab;
	public GameObject mimicPrefab;
	private Scoring _scoring;
	private GameObject GM;

	GameObject player;
	private int playerHealth;

	List<GameObject> enemies = new List<GameObject>();
    private void OnValidate (){
        if (resetBool) {
            ResetLevelGenerator();
            resetBool = false;
        }

		if (hardResetBool) {
            HardResetLevelGenerator();
            hardResetBool = false;
        }
    }

	void Start (){
		GM = GameObject.Find("GM");
		difficulty = GM.GetComponent<DifficultyController>().difficulty;
		if(GM.GetComponent<DifficultyController>().islevelCompleted){
			GM.GetComponent<DifficultyController>().islevelCompleted = false;
			iterationRange.x += difficulty;
			iterationRange.y += difficulty;
		}
		roomLayerMask = LayerMask.GetMask ("Room");
		StartCoroutine ("GenerateLevel");
		_scoring = GameObject.Find("Score").GetComponent<Scoring>();
		_scoring.addScore(0);
		player = GameObject.FindWithTag("Player");
		if(player){
			playerHealth = player.GetComponent<HealthManager>().GetHealth();
			healthBar.SetMaxHealth(playerHealth);
		}
		
	}

	void Update(){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}else{
			playerHealth = player.GetComponent<HealthManager>().GetHealth();
			healthBar.SetHealth(playerHealth);
		}
	}

	IEnumerator GenerateLevel (){
		// We use waitforfixedupdate in order to make sure between room placments we are wating for the next fixed frame.
		// This is done because we need make sure that the physics and colliders have been updated before placement.
		// If we dont do this its possible that we're not able to detect overlaps using the layermask.
		WaitForSeconds startup = new WaitForSeconds (1);
		WaitForFixedUpdate interval = new WaitForFixedUpdate ();
		
		// If we have memories of previous levels we update the room ratios here
		if (memoryMatrix.Count != 0){
			updateRatioByMemories();
		}

		// Update ratios for the secret rooms
		if (lastPlacedSecrets.Count != 0){
			updateSecretRatio();
		}
		else {
			lastPlacedSecrets.Clear();
		}
		
		yield return startup;

		// Place start room
		PlaceStartRoom ();
		yield return interval;

		// Number of iterations based on the range we give
		int iterations = Random.Range ((int)iterationRange.x, (int)iterationRange.y);

		// Place prefab prefab from list
		for (int i = 0; i < iterations; i++) {
			PlaceRoom (ref roomPrefabs);
			yield return interval;
		}

		// Place secret prefab from list based on ceiling of our division
		for (int i = 0; i < Mathf.Ceil(iterations / secretDivisor) + 1; i++){
			PlaceRoom(ref secretPrefabs);
			yield return interval;
		}

		// Place end room
		PlaceEndRoom ();
		yield return interval;

		// Round the level until we are unable
		while (RoundLevel()){
			yield return interval;
		}

		// Update the memory of our placement outcomes
		memoryMatrix.Add(placedRooms);
		AddSecretToList(ref placedSecrets);

		// bake navmesh
		yield return interval;
		surface.BuildNavMesh();

		// Spawn Enemies 
		// Debug.Log(currentRoom.gameObject);
		foreach(Transform room in transform){
			foreach(Transform child in room){
				if(child.CompareTag("EnemySpawnPoint")){
					Vector3 enemyPos = new Vector3(child.transform.position.x, -1.9f , child.transform.position.z);
					GameObject enemyClone = Instantiate(enemyPrefab, enemyPos, child.transform.rotation);
					enemies.Add(enemyClone);
				}else if(child.CompareTag("MimicSpawnPoint")){
					Vector3 mimicPos = new Vector3(child.transform.position.x,child.transform.position.y , child.transform.position.z);
					GameObject enemyClone = Instantiate(mimicPrefab, mimicPos, child.transform.rotation);
					enemies.Add(enemyClone);
				}
			}
		}

		// Level generation finished
		StopCoroutine ("GenerateLevel");
	}

	void updateRatioByMemories (){
		int totalRatio = sumRatio(roomPrefabs);

		// We check how the selection of each prefab ratio expected result compares to actual
		foreach (Room prefab in roomPrefabs){
			foreach (List<Room> memory in memoryMatrix){
				float expectedNumber = memory.Count * (prefab.ratio / totalRatio);
				int count = 0;
				foreach (Room room in memory){
					if (room == prefab){
						count++;
					}
				}

				// Based on our comparison between expected and actual count we update ratios
				if (count < expectedNumber){
					prefab.ratio += memoryMultiplier;
				}

				if (count > expectedNumber && prefab.ratio > memoryMultiplier){
					prefab.ratio -= memoryMultiplier;
				}

				// More recent memories have a greater impact on the ratio manipulation as loop progresses
				memoryMultiplier += memoryFactor;
			}
		}
	}

	void updateSecretRatio(){
		// If we got unlucky and got no treasure then we increase likelyhood, and vice versa
		foreach (Room room in secretPrefabs){
			if (!lastPlacedSecrets.Contains(room)){
				room.ratio += 1;
			}
		}
	}

	int SelectByRatio(List<Room> rooms) {
		// Method that intregrates room ratios with Random to return list index
		int totalRatio = sumRatio(rooms);
		int randomValue = (int) Random.Range(0, totalRatio);

		for (int i = 0; i < rooms.Count; i++){
			if ((randomValue -= rooms[i].ratio) < 0){
				return i;
			}
		}
		return 0;
	}

	int sumRatio(List<Room> rooms){
		// Simple summation method for ratios
		int sum = 0;
		foreach(Room room in rooms){
			sum += room.ratio;
		}
		return sum;
	}

	void PlaceStartRoom (){
		// Instantiate room
		startRoom = Instantiate (startRoomPrefab) as StartRoom;
		startRoom.transform.parent = this.transform;

		// Spawn player
		Vector3 playerPos = new Vector3(startRoom.transform.position.x, -1.416669f, startRoom.transform.position.z);
		player = Instantiate(playerPrefab, playerPos, startRoom.transform.rotation);
		healthBar.SetMaxHealth(player.GetComponent<HealthManager>().startingHealth);
		// Add room dorways to all doorways list
		AddDoorwaysToList (startRoom, ref availableDoorways);

		// Position room at origin
		startRoom.transform.position = Vector3.zero;
		startRoom.transform.rotation = Quaternion.identity;
	}

	void PlaceRoom (ref List<Room> prefabs)
	{
		// Instantiate room
		Room currentRoom = Instantiate (prefabs[SelectByRatio(prefabs)]) as Room;
		currentRoom.transform.parent = this.transform;

		// Create doorway lists for loop
		List<Doorway> allAvailableDoorways = new List<Doorway> (availableDoorways);
		
		Shuffle(allAvailableDoorways);

		List<Doorway> currentRoomDoorways = new List<Doorway> ();
		AddDoorwaysToList (currentRoom, ref currentRoomDoorways);

		// Get doorways from current room and add them to list of available doorways
		AddDoorwaysToList (currentRoom, ref availableDoorways);

		bool roomPlaced = false;

		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways) {
			// Try all available doorways in current room
			foreach (Doorway currentDoorway in currentRoomDoorways) {
				// Position room
				PositionRoomAtDoorway (ref currentRoom, currentDoorway, availableDoorway);

				// Check room overlaps
				if (CheckRoomOverlap (currentRoom)) {
					continue;
				}

				roomPlaced = true;

				// Add room to list depending on type
				if (prefabs == roomPrefabs){
					placedRooms.Add (currentRoom);
				} else {
					placedSecrets.Add (currentRoom);
				}

				// Remove occupied doorways
				currentDoorway.gameObject.SetActive (false);
				availableDoorways.Remove (currentDoorway);

				availableDoorway.gameObject.SetActive (false);
				availableDoorways.Remove (availableDoorway);

				// Exit loop if room has been placed
				break;
			}

			// Exit loop if room has been placed
			if (roomPlaced) {
				break;
			}
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced) {
			Destroy (currentRoom.gameObject);
			ResetLevelGenerator ();
		}
	}
	void PlaceEndRoom ()
	{
		// Instantiate room
		endRoom = Instantiate (endRoomPrefab) as EndRoom;
		endRoom.transform.parent = this.transform;

		// Create doorway lists to loop over
		List<Doorway> allAvailableDoorways = new List<Doorway> (availableDoorways);
		Doorway doorway = endRoom.doorways [0];

		bool roomPlaced = false;

		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways) {
			// Position room
			Room room = (Room) endRoom;
			PositionRoomAtDoorway (ref room, doorway, availableDoorway);

			// Check room overlaps
			if (CheckRoomOverlap (endRoom)) {
				continue;
			}

			roomPlaced = true;

			// Remove occupied doorways
			doorway.gameObject.SetActive (false);
			availableDoorways.Remove (doorway);

			availableDoorway.gameObject.SetActive (false);
			availableDoorways.Remove (availableDoorway);

			// Exit loop if room has been placed
			break;
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced) {
			ResetLevelGenerator ();
		}
	}

	bool RoundLevel(){
		// Method is similar to PlaceRoom() except it is refactored to be used inside while loop
		Room currentRoom = Instantiate (roundingPrefabs[SelectByRatio(roundingPrefabs)]) as Room;
		currentRoom.transform.parent = this.transform;

		List<Doorway> allAvailableDoorways = new List<Doorway> (availableDoorways);
		Shuffle(allAvailableDoorways);

		List<Doorway> currentRoomDoorways = new List<Doorway> ();
		AddDoorwaysToList (currentRoom, ref currentRoomDoorways);

		// Get doorways from current room and add them randomly to the list of available doorways
		AddDoorwaysToList (currentRoom, ref availableDoorways);

		foreach (Doorway availableDoorway in allAvailableDoorways){
			foreach (Doorway currentDoorway in currentRoomDoorways) {

				PositionRoomAtDoorway (ref currentRoom, currentDoorway, availableDoorway);

				// Check room overlaps
				if (CheckRoomOverlap (currentRoom)) {
					continue;
				}

				// Add room to list depending on type
				placedRooms.Add (currentRoom);

				// Remove occupied doorways
				currentDoorway.gameObject.SetActive (false);
				availableDoorways.Remove (currentDoorway);

				availableDoorway.gameObject.SetActive (false);
				availableDoorways.Remove (availableDoorway);

				// We return true if rounding is sucessfull
				return true;
			}
		}

		// If we can no longer round the level we destroy and end loop
		Destroy (currentRoom.gameObject);
		return false;
	}

	void AddDoorwaysToList (Room room, ref List<Doorway> list){
		// Doorways are inserted randomly into list
		foreach (Doorway doorway in room.doorways) {
			int r = Random.Range (0, list.Count);
			list.Insert (r, doorway);
		}
	}

	void AddSecretToList(ref List<Room> list){
		// Simple memory of secrets placed in last level
		foreach (Room room in list) {
			lastPlacedSecrets.Add(room);
		}
	}

	void PositionRoomAtDoorway (ref Room room, Doorway roomDoorway, Doorway targetDoorway)
	{
		// Set our rotatin to a default in order to make maths consistant 
		room.transform.position = Vector3.zero;
		room.transform.rotation = Quaternion.identity;

		// Rotate room to match the target doorway orientation
		Vector3 targetDoorwayEuler = targetDoorway.transform.eulerAngles;
		Vector3 roomDoorwayEuler = roomDoorway.transform.eulerAngles;

		// Get the delta angle between the room and target doorways
		float deltaAngle = Mathf.DeltaAngle (roomDoorwayEuler.y, targetDoorwayEuler.y);

		// Get the amount of rotation we need to rotate the room
		Quaternion targetRotation = Quaternion.AngleAxis (deltaAngle, Vector3.up);

		// Rotate the room based on that Quanternion
		room.transform.rotation = targetRotation * Quaternion.Euler (0, 180f, 0);

		// Position room back in correct location
		Vector3 roomPositionOffset = roomDoorway.transform.position - room.transform.position;
		room.transform.position = targetDoorway.transform.position - roomPositionOffset;
	}

	bool CheckRoomOverlap (Room room)
	{
		// First get the bounds of the room
		Bounds bounds = room.RoomBounds;

		// We slightly shrink the bounds this give some leeway to allow for decoration walls to overlap
		bounds.Expand (-0.01f);
		bounds.center = room.transform.position; 

		// Get colliders and check each room
		Collider[] colliders = Physics.OverlapBox (bounds.center, bounds.size / 2, room.transform.rotation, roomLayerMask);
		if (colliders.Length > 0) {
			foreach (Collider c in colliders) {
				// Ignore current room
				if (c.transform.parent.gameObject.Equals (room.gameObject)) {
					continue;
				} else {
					// We have an overlap
					return true;
				}
			}
		}
		return false;
	}

	List<Doorway> Shuffle (List<Doorway> ts) {
		// Shuffle method for lists
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
		return ts;
    }

	void ResetLevelGenerator (){
		// Reset the generation by destorying all game objects and clearing lists (except memory)
		Debug.LogError ("Reset level generator");

		StopCoroutine ("GenerateLevel");

		// Delete all rooms
		if (startRoom) {
			Destroy (startRoom.gameObject);
		}

		if (endRoom) {
			Destroy (endRoom.gameObject);
		}

		foreach (Room room in placedRooms) {
			Destroy (room.gameObject);
		}

		foreach (Room room in placedSecrets) {
			Destroy (room.gameObject);
		}

		// Clear lists
		placedRooms.Clear ();
		availableDoorways.Clear ();
		placedSecrets.Clear();
		foreach(GameObject enemy in enemies){
			Destroy(enemy);
		}
		Destroy(player);

		// Reset coroutine
		StartCoroutine ("GenerateLevel");
	}

	void HardResetLevelGenerator (){
		// Regular reset with memory clear
		Debug.LogError ("Hard Reset level generator");

		ResetLevelGenerator();

		memoryMatrix.Clear();
		lastPlacedSecrets.Clear();

		StopCoroutine ("GenerateLevel");

		// Reset coroutine
		StartCoroutine ("GenerateLevel");
	}

	public void CompleteLevel(){
		GM.GetComponent<DifficultyController>().AddDifficulty();
		GM.GetComponent<DifficultyController>().islevelCompleted = true;
	}
}
