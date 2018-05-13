using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Amadues
{
    class ParticleCluster
    {
        public List<Particle> explosions;
        public int lifeSpan;
        public int timeAlive;
        public Texture2D texture;
        Random random;

        public ParticleCluster(int numberOfExplosions, double x, double y,
                                double radius, int minSpeed,
                                int maxSpeed, Texture2D aTexture, int aTimeToLive)
        {
            texture = aTexture;
            random = new Random();
            explosions = new List<Particle>();
            timeAlive = 0;
            lifeSpan = aTimeToLive;
            for (int i = 0; i < numberOfExplosions; i++)
            {
                explosions.Add(new Particle((random.Next(0, 100) * Math.PI) / 50, 
                                                random.Next(minSpeed,maxSpeed),
                                                (random.Next(0,100) * Math.PI)/50, 
                                                0.05, radius/2, 2, 
                                                (double)random.Next((int)(x - radius), (int)(x + radius)), 
                                                (double)random.Next((int)(y - radius), (int)(y + radius))));
            }
        }

        public void Update()
        {
            timeAlive++;
            for (int i = 0; i < explosions.Count(); i++)
            {
                explosions[i].Update();
            }
        }

        public Boolean IsExpired()
        {
            return timeAlive >= lifeSpan;
        }

        public byte Transparency()
        {
            if (timeAlive == 0)
            {
                return 255;
            }
            else
            {
                return (byte)(((float)lifeSpan / (float)timeAlive) * 255f);
            }
        }

    }
}