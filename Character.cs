﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;


namespace Snakedy
{
    public class Character : ICollisionActor
    {
        public Texture2D Texture;
        public SoundEffect HitSound;

        public Vector2 Position;
        public float HitForce;

        public double Angle = 0;
        public float Velocity = 0;
        public Vector2 Force = Vector2.Zero;
        Vector2 HitDirection;
        float MinSpeed = 0.01f;

        bool Collided = false;
        public bool Holding = false;

        public IShapeF Bounds { get; }
        public ICollisionActor CollidedWith { get; private set; }
        public double CollidedTo { get; private set; }

        public Character(Vector2 position, float hitForce = 0.01f,float collisionSize = 20f)
        {
            Position = position;
            HitForce = hitForce;
            Bounds = new CircleF(position, collisionSize);
        }

        public void Move(GameTime gameTime)
        {
            Vector2 dir = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            Force = dir * Velocity;
            Position += Force * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Velocity > MinSpeed)
                Velocity -= MinSpeed;
            else if (Velocity > 0)
                Velocity = 0;

            Bounds.Position = Position;

            var kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.Space))
                Collided = false;
        }

        public void PreparingHit(bool pressed) 
        {
            if (pressed)
                HitCalculating(true);
            if (Holding = true && pressed == false)
                HitCalculating(false);
            Holding = pressed;
        }

        public void HitCalculating(bool prepare)
        {
            if (prepare)
            {
                var mousePos = Mouse.GetState().Position.ToVector2();
                HitDirection = Position - mousePos;
                //Console.WriteLine(HitDirection);
            }
            else if (HitDirection != Vector2.Zero)
            {
                Angle = Math.Atan2(HitDirection.Y, HitDirection.X);
                Velocity = HitDirection.Length() * HitForce;
                HitDirection = Vector2.Zero;
                CollidedWith = null;

                HitSound.Play();
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (CollidedWith != collisionInfo.Other)
            {
                CalculateCollision(collisionInfo);
                CollidedWith = collisionInfo.Other;
            }
            else
            {
                Vector2 dir = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
                Position += dir;
            }
        }

        public void CalculateCollision(CollisionEventArgs collisionInfo)
        {
            var n = collisionInfo.PenetrationVector;
            var surface = new Vector2(n.Y, -n.X);
            Console.WriteLine("surface:"+surface.ToString() + " ");
            var surfAngle = (Math.Atan2(surface.Y, surface.X) + Math.PI) % Math.PI;
            Angle = surfAngle + surfAngle - Angle;
            CollidedTo = Angle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 16, Color.Red, 3f);
            if (Holding)
                Effects.DrawArrow(Mouse.GetState().Position.ToVector2(), Position);
        }
    }
}
