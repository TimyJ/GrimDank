using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System;

namespace GrimDank
{
    class Lerp2D
    {
        public Point StartPosition { get; private set; }
        public Point EndPosition { get; private set; }


        private double _currentX;
        public int CurrentX
        {
            get => (int)Math.Round(_currentX, MidpointRounding.AwayFromZero);
        }

        private double _currentY;
        public int CurrentY
        {
            get => (int)Math.Round(_currentY, MidpointRounding.AwayFromZero);
        }

        public Point CurrentPosition { get => new Point(CurrentX, CurrentY); }

        // Time set for the lerping, in seconds
        public double TimeToLerp { get; private set; }

        public bool IsFinished { get => CurrentPosition == EndPosition; }

        private double dxPerSecond;
        private double dyPerSecond;

        public Lerp2D(Point startPosition, Point endPosition, double timeInSeconds)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;

            _currentX = startPosition.X;
            _currentY = startPosition.Y;
            TimeToLerp = timeInSeconds;

            var deltaDistance = EndPosition - StartPosition;
            dxPerSecond = deltaDistance.X / timeInSeconds;
            dyPerSecond = deltaDistance.Y / timeInSeconds;
        }

        public Lerp2D(Point endPosition, double timeInSeconds)
            : this(new Point(0, 0), endPosition, timeInSeconds) { }

        public Lerp2D(int endX, int endY, double timeInSeconds)
            : this(new Point(endX, endY), timeInSeconds) { }

        public Lerp2D(int startX, int startY, int endX, int endY, double timeInSeconds)
            : this(new Point(startX, startY), new Point(endX, endY), timeInSeconds) { }

        // Do the lerping.
        public void Update(GameTime deltaTime)
        {
            // Lerp
            var deltaSeconds = deltaTime.ElapsedGameTime.TotalSeconds;
            _currentX += dxPerSecond * deltaSeconds;
            _currentY += dyPerSecond * deltaSeconds;

            // Clamp to end position to avoid overshoot
            _currentX = Math.Min(_currentX, EndPosition.X);
            _currentY = Math.Min(_currentY, EndPosition.Y);
        }
    }
}
