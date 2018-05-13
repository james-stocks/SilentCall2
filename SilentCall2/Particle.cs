using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    class Particle
    {
        public double direction, speed, rotation, rotationspeed, scale, scalespeed, x, y;
        
        public Particle(double aD, double aS, double aR, 
                        double aRSpeed, double aScale, 
                        double aScaleSpeed, double aX, double aY)
        {
            direction = aD;
            speed = aS;
            rotation = aR;
            rotationspeed = aRSpeed;
            scale = aScale;
            scalespeed = aScaleSpeed;
            x = aX;
            y = aY;
        }

        public void Update()
        {
            x += speed * Math.Cos(direction);
            y += speed * Math.Sin(direction);
            rotation += rotationspeed;
            scale += scalespeed;
        }

    }
}