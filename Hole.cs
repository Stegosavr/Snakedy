﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Snakedy
{
    public class Hole 
    {
        public Vector2 Position
        {
            get { return Bounds.Position; }
            set { Bounds.Position = value; AreaBounds.Position = value; }
        }
        public IShapeF Bounds { get; }
        public IShapeF AreaBounds { get; }

        List<IObstacle> Spawned;

        public Hole(Vector2 position = new Vector2())
        {
            Bounds = new CircleF(position, 37);
            AreaBounds = new CircleF(position, 57);
            SpawnHole();
            if (Position != Vector2.Zero)
            {
                Bounds.Position = position;
                Position = position;
            }
            Spawned = new List<IObstacle>();
        }

        public void SpawnHole()
        {
            Position = Obstacles.GetRandomPositionNotIntersecting(Bounds, Globals.Obstacles.Select(o=>o.Bounds));
        }

        public bool Check(Vector2 ballPosition)
        {
            var distance = (Position - ballPosition).Length();
            if (distance < 30)
                return true;
            return false;
        }

        public void DespawnObstacles()
        {
            Obstacles.RemoveObstacles(Spawned);
            Spawned.Clear();
        }

        public void Update(Vector2 ballPosition,Timer timer)
        {
            if (Check(ballPosition))
            {
                Globals.Ball.Velocity = 0.001f;

                Obstacles.RemoveObstacles(Spawned);
                Spawned = Obstacles.CreateRandomObstacles(5,(obs)=>RectangleObstacle.CreateRandomRectangle(obs),new List<IShapeF>() { AreaBounds});
                Spawned = Spawned.Concat(PitObstacle.CreateRandomPool(9,new List<IShapeF>() { AreaBounds })).ToList();

                SpawnHole();
                timer.AddTime(timer.DelayTime);
                Console.WriteLine("Score: " + ++Globals.Score);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 16, Color.Black, 37f);
        }

    }
}
