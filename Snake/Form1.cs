using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            //Load settings and set to default
            new Settings();

            //Sets game speed and start timer
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Starts new game
            StartGame();
        }

        /// <summary>
        /// StartGame() runs on game start, resets Settings and the Snake
        /// </summary>
        private void StartGame()
        {
            labelGameOver.Visible = false;

            //Sets settings to default, again
            new Settings();

            //Create new snake object
            Snake.Clear();
            Circle head = new Circle { X = Settings.Width / 2, Y = Settings.Height / 2 };
            Snake.Add(head);

            labelScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        /// <summary>
        /// GenerateFood() places food on the board
        /// </summary>
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle { X = random.Next(0, maxXPos), Y = random.Next(0, maxYPos) };
        }

        /// <summary>
        /// UpdateScreen() allows restarting, runs MovePlayer(), and turns the player character. 
        /// Also makes sure the player cannot turn the opposite direction of which it is facing.
        /// </summary>
        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver == true)
            {
                //Check if Enter is pressed to restart.
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                //Turns the snake if it is not facing the opposite direction.
                if (Input.KeyPressed(Keys.Right) && Settings.Direction != Direction.Left)
                    {Settings.Direction = Direction.Right;}
                else if (Input.KeyPressed(Keys.Left) && Settings.Direction != Direction.Right)
                    {Settings.Direction = Direction.Left;}
                else if (Input.KeyPressed(Keys.Up) && Settings.Direction != Direction.Down)
                    {Settings.Direction = Direction.Up;}
                else if (Input.KeyPressed(Keys.Down) && Settings.Direction != Direction.Up)
                    {Settings.Direction = Direction.Down;}

                MovePlayer();
            }

            pbCanvas.Invalidate();

        }

        /// <summary>
        /// Draws objects on screen. Triggers on updatetick.
        /// </summary>
        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;    //Colour to head
                    else
                    {
                        snakeColour = Brushes.Green;    //Colour to rest of Body
                    }

                    //Draw snake
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    //Draw food
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                                      food.Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                }
            }
            //If gameOver = true a label is updated to display the player score.
            else
            {
                string gameOver = "Game over! \nYour final score is: " + Settings.Score + "\nPress enter to try again";
                labelGameOver.Text = gameOver;
                labelGameOver.Visible = true;
            }
        }

        /// <summary>
        /// MovePlayer() "moves" the snake by moving the snake head in the direction the player wishes.
        /// Also detects collision with game borders, own body, and food.
        /// Each body part behind the head is "moved" by setting its position to that of the piece in front of it.
        /// </summary>
        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.Direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    //Get maximum X and Y pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    //Detect colliosion with the game borders
                    if (Snake[i].X < 0 || Snake[i].Y < 0 || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }

                    //Detect collision with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }
                }
                else
                {
                    //Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        /// <summary>
        /// If Die() is run, gameOver is set to true and the game resets.
        /// </summary>
        private void Die()
        {
            Settings.GameOver = true;
        }

        /// <summary>
        /// If Eat() is run, a new part is added to the circle, the score is updated, and a new foodpiece is generated.
        /// </summary>
        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update score
            Settings.Score += Settings.Points;
            labelScore.Text = Settings.Score.ToString();

            //Generate new food
            GenerateFood();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}