using Roguelike_Game.Core;
using RogueSharp;
using System.Linq;

namespace Roguelike_Game.Systems
{
    class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        // Constructing a new MapGenerator requires the dimensions of the maps it will create
        //as well as the sizes and maximum number of rooms 
        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        // Generate a new map that is a simple open floor with walls around the outside
        public DungeonMap CreateMap()
        {
            // Initialize every cell in the map by
            // setting walkable, transparency, and explored to true
            _map.Initialize(_width, _height);

            //To place as many rooms as the specified maxRooms
            for(int r= _maxRooms; r > 0; r--)
            {
                //Determine the the size and position of the room randomly
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                //Presenting all rooms as rectangles
                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                //To check if room rectangle intersects with other rooms
                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                //As long as it doesn't intersect, add it to the list of rooms
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }    
            }

            // call CreateRoom to make it
            foreach (Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            PlacePlayer();

            return _map;
        }

        private void CreateRoom(Rectangle room)
        {
            for(int x = room.Left + 1; x < room.Right; x++)
            {
                for(int y = room.Top +1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, true);
                }
            }
        }

        // Find the center of the first room that we created and place the Player there
        private void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }
    }
}
