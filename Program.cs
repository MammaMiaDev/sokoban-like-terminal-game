namespace CommandLineGame;

public static class Program {
    private const int Rows = 10;
    private const int Cols = 30;
    private static char[,] _grid = new char[Rows, Cols];
    private static int _coins;

    private const char ObstacleChar = '#';
    private const char CoinChar = '*';
    private const char PlayerChar = 'x';
    private const char WallChar = '0';
    private const char TileChar = '_';

    public static void Main() {
        Console.Clear();
        while (Run()) { }
        Console.WriteLine("\nBBye!");
    }

    private static bool Run() {
        var player = (X: 1, Y: 1);
        
        _grid = new char[Rows, Cols];
        _coins = 0;

        // Populate grid
        for (var x = 0; x < Cols; x++) {
            for (var y = 0; y < Rows; y++) {
                if (x == 0 || x == Cols - 1 || y == 0 || y == Rows - 1) {
                    _grid[y, x] = WallChar; continue;
                }
                _grid[y, x] = TileChar;
            }
        }

        // Place boxes and coins
        var rnd = new Random();
        var boxes = Enumerable.Range(0, 10).Select(_ => (X: rnd.Next(2, Cols-2), Y: rnd.Next(2,Rows-2))).ToArray();
        foreach (var box in boxes) { _grid[box.Y, box.X] = ObstacleChar; }
        var coinsPositions = Enumerable.Range(0, 5).Select(_ => (X: rnd.Next(1, Cols-1), Y: rnd.Next(1, Rows-1))).ToArray();
        foreach (var coin in coinsPositions) { _grid[coin.Y, coin.X] = CoinChar; }

        // Place the player
        _grid[player.Y, player.X] = PlayerChar;

        while (true) {
            Console.Clear();
            Console.WriteLine($"Welcome to this anonymous Sokoban-like game!\n\nCoins: {_coins}\n");

            // Update the grid
            for (var i = 0; i < _grid.GetLength(0); i++) {
                for (var j = 0; j < _grid.GetLength(1); j++) {
                    Console.Write(_grid[i, j]);
                }
                Console.Write("\n");
            }

            // Check winning condition
            if (_coins == coinsPositions.Length) {
                Console.WriteLine("\nYOU WIN!\nPRESS R TO RESTART OR ESC TO QUIT.");
                while (true)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.R) return true;
                    if (key.Key == ConsoleKey.Escape) return false;
                }
            }
            
            // elaborate user input
            Console.WriteLine("\nArrow keys => move the player\nEscape => exit.");
            var input = Console.ReadKey();
            switch (input.Key) {
                case ConsoleKey.Escape: return false;
                case ConsoleKey.UpArrow: Move(0, -1, ref player); break;
                case ConsoleKey.DownArrow: Move(0, 1, ref player); break;
                case ConsoleKey.LeftArrow: Move(-1, 0, ref player); break;
                case ConsoleKey.RightArrow: Move(1, 0, ref player); break;
                case ConsoleKey.R: return true;
            }
        }
    }
    
    private static void Move(int x, int y, ref (int X, int Y) player) {
        var to = new { X = player.X + x, Y = player.Y + y };
        if (to.X <= 0 || to.Y <= 0 || to.X >= Cols - 1 || to.Y >= Rows - 1) return;

        if (_grid[to.Y, to.X] == ObstacleChar)
        {
            var obstacleTo = new { X = to.X + x, Y = to.Y + y };
            if (obstacleTo.X <= 0 || obstacleTo.Y <= 0 || 
                obstacleTo.X >= Cols - 1 || obstacleTo.Y >= Rows - 1 ||
                _grid[obstacleTo.Y, obstacleTo.X] == ObstacleChar ||
                _grid[obstacleTo.Y, obstacleTo.X] == CoinChar) return;
            _grid[obstacleTo.Y, obstacleTo.X] = ObstacleChar;
        }

        if (_grid[to.Y, to.X] == CoinChar) _coins++;

        _grid[player.Y, player.X] = TileChar;
        _grid[to.Y, to.X] = PlayerChar;

        player.X = to.X; player.Y =to.Y;
    }
}
