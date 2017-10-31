namespace Snake
{
    //Selection, direction can be specified with Settings.Direction."..."
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    class Settings
    {
        //Each setting variable
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int Speed { get; set; }
        public static int Score { get; set; }
        public static int Points { get; set; }
        public static bool GameOver { get; set; }
        public static Direction Direction { get; set; }

        //Default settings
        public Settings()
        {
            Width = 12;
            Height = 12;
            Speed = 12;
            Score = 0;
            Points = 10;
            GameOver = false;
            Direction = Direction.Down;
        }
    }
}